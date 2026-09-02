using EcoCoinSharedTypes;
using EcoCoinValidator.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinValidator
{
    internal class RequestValidationRouter
    {
        internal static TransactionValidationResponse ValidateTransaction(TransactionRequest TranReq, byte[] EnvelopeSignature)
        {
            TransactionValidationResponse Approval = new TransactionValidationResponse() { Approved = false };

            if (ValidateEnvelopeSignature(TranReq, EnvelopeSignature))
            {
                try
                {
                    RetrieveRelevantFiles(TranReq);
                }
                catch (Exception ex)
                {
                    Approval.Approved = false;
                    Approval.DenyReason = "Failed to retrieve relevant files for the transaction request.";
                }

                if (Approval.DenyReason == "")
                {
                    if (GetMostRecentSignerFile(TranReq))
                    {
                        if (ValidateSignerKeyEnabled(TranReq))
                        {
                            if (ValidateTransactionRequestSignature(TranReq))
                            {
                                if (VerifyNOnceOrder(TranReq))
                                {
                                    switch (TranReq.RequestType)
                                    {
                                        case RequestType.CreateAccount:
                                            Approval = AccountCreation.Validate(TranReq, EnvelopeSignature);
                                            break;
                                        case RequestType.AddKey:
                                            Approval = AddKey.Validate(TranReq, EnvelopeSignature);
                                            break;
                                    }
                                }
                                else
                                {
                                    Approval.Approved = false;
                                    Approval.DenyReason = "The NOnce value of the transaction request is not greater than the last used NOnce value for the signing account.";
                                }
                            }
                            else
                            {
                                Approval.Approved = false;
                                Approval.DenyReason = "The transaction request was not signed by the signing key.";
                            }
                        }
                        else
                        {
                            Approval.Approved = false;
                            Approval.DenyReason = "The signing key is disabled and cannot be used to sign transactions.";
                        }
                    }
                    else
                    {
                        Approval.Approved = false;
                        Approval.DenyReason = "Failed to retrieve the most recent signer account file for validation.";
                    }
                }
            }
            else
            {
                Approval.Approved = false;
                Approval.DenyReason = "The transaction request envelope was not signed by the Automate Earth server, or the request data sent does not match the signature.";
            }

            return Approval;
        }

        /// <summary>
        /// Validate that the envelope was signed by the Automate Earth server, and that the request data sent matches the signature.
        /// </summary>
        /// <param name="TranReq"></param>
        /// <param name="EnvelopeSignature"></param>
        /// <returns></returns>
        private static bool ValidateEnvelopeSignature(TransactionRequest TranReq, byte[] EnvelopeSignature)
        {
            bool Approval = false;

            //AE Server account signs all transaction requests
            AccountDetails SignerAccount = new AccountDetails(GlobalVars.AEOfficialServerAccount);

            KeyPair ValidKeyPair = null;

            using (RSA rsa = RSA.Create())
            {
                foreach (KeyPair KP in SignerAccount.ApprovedKeys)
                {
                    rsa.ImportFromPem(KP.PublicKey);

                    if (rsa.VerifyData(GlobalFunctions.SerializeObjectToByteArray(TranReq), EnvelopeSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                    {
                        ValidKeyPair = KP;
                    }
                }
            }

            if (ValidKeyPair != null)
            {
                Approval = true;
            }

            return Approval;
        }
        private static bool ValidateTransactionRequestSignature(TransactionRequest TranReq)
        {
            bool Approval = false;
            AccountDetails SignerAccount = new AccountDetails(TranReq.TransactionSignerID);
            KeyPair ValidKeyPair = SignerAccount.ApprovedKeys[TranReq.TransactionSignerKeyID];
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(ValidKeyPair.PublicKey);
                string TransactionSignature = TranReq.TransactionSignature;
                TranReq.TransactionSignature = "";
                if (rsa.VerifyData(GlobalFunctions.SerializeObjectToByteArray(TranReq), Convert.FromHexString(TransactionSignature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {
                    Approval = true;
                }
            }
            return Approval;
        }
        private static bool GetMostRecentSignerFile(TransactionRequest TranReq)
        {
            bool Valid = false;

            try
            {
                if (!System.IO.File.Exists(GlobalVars.AccountStoragePath + TranReq.TransactionSignerID + ".acc") || GlobalFunctions.HashFileAsString(GlobalVars.AccountStoragePath + TranReq.TransactionSignerID + ".acc") != TranReq.TransactionSignerAccountVersionHash)
                {
                    AccountDetails.DownloadAccountFile(TranReq.TransactionSignerID);

                    if (GlobalFunctions.HashFileAsString(GlobalVars.AccountStoragePath + TranReq.TransactionSignerID + ".acc") == TranReq.TransactionSignerAccountVersionHash)
                    {
                        Valid = true;
                    }
                }
                else
                {
                    Valid = true;
                }
            }
            catch (Exception ex)
            {

            }

            return Valid;
        }
        private static void RetrieveRelevantFiles(TransactionRequest TranReq)
        {
            foreach (EcoFileStub File in TranReq.InvolvedFiles)
            {
                if (File.FileName.Contains(".acc"))
                {
                    if (!System.IO.File.Exists(GlobalVars.AccountStoragePath + File.FileName) || GlobalFunctions.HashFile(GlobalVars.AccountStoragePath + File.FileName) != File.FileHash)
                    {
                        AccountDetails.DownloadAccountFile(Guid.Parse(File.FileName.Replace(".acc", "")));
                    }
                }
            }
        }
        private static bool ValidateSignerKeyEnabled(TransactionRequest TranReq)
        {
            bool Valid = false;
            AccountDetails SignerAccount = new AccountDetails(TranReq.TransactionSignerID);
            KeyPair SigningKey = SignerAccount.ApprovedKeys[TranReq.TransactionSignerKeyID];
            if (!SigningKey.Disabled)
            {
                Valid = true;
            }
            return Valid;
        }
        private static bool VerifyNOnceOrder(TransactionRequest TranReq)
        {
            bool Valid = false;
            AccountDetails SignerAccount = new AccountDetails(TranReq.TransactionSignerID);
            if (TranReq.NOnce > SignerAccount.LastUsedNOnce)
            {
                Valid = true;
            }
            return Valid;
        }
    }
}
