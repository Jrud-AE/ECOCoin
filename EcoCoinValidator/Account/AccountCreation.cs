using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EcoCoinValidator.Account
{
    public class AccountCreation
    {
        public static TransactionValidationResponse Validate(TransactionRequest TranReq, byte[] TransactionSignature)
        {
            TransactionValidationResponse Approval = new TransactionValidationResponse();
            Approval.Approved = false;

            //CHECK 1: only the Automate Earth account creation account can create accounts.
            if (TranReq.TransactionSignerID == GlobalVars.AEAccountCreationAccount)
            {
                bool VerifyResult = false;
                AccountDetails SignerAccount = new AccountDetails(TranReq.TransactionSignerID);

                using (RSA rsa = RSA.Create())
                {
                    foreach (KeyPair KP in SignerAccount.ApprovedKeys)
                    {
                        rsa.ImportFromPem(KP.PublicKey);

                        if (rsa.VerifyData(GlobalFunctions.SerializeObjectToByteArray(TranReq), TransactionSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                        {
                            VerifyResult = true;
                        }
                    }
                }
                //CHECK 2: Verify that the Automate Earth account creation account is the one that signed the request.
                if (VerifyResult)
                {
                    //CHECK 3: Verify that the account name is shorter than 50 characters
                    if (TranReq.AccountName.Length <= 50)
                    {
                        //CHECK 4: Verify that the account name is at least 1 characters
                        if (TranReq.AccountName.Length > 0)
                        {
                            //CHECK 5: Verify that the account name only contains approved characters
                            string ApprovedChars = "^[a-zA-Z0-9]+$";

                            if (Regex.IsMatch(TranReq.AccountName, ApprovedChars))
                            {
                                //CHECK 6: Verify that the initial public key is valid
                                if (AddKey.IsKeyValid(TranReq.InitialPublicKey))
                                {
                                    //CHECK 7: Verify that the initial public key is not already on the account
                                    if (!AddKey.IsKeyAlreadyOnAccount(SignerAccount, TranReq.InitialPublicKey))
                                    {
                                        //CHECK 8: Verify that NOnce is 0 on initial account creation
                                        if (TranReq.NOnce == 0)
                                        {
                                            Approval.Approved = true;
                                        }
                                        else
                                        {
                                            Approval.DenyReason = "The NOnce must be 0 on initial account creation.";
                                        }
                                    }
                                    else
                                    {
                                        Approval.DenyReason = "The initial public key is already on the account.";
                                    }
                                }
                                else
                                {
                                    Approval.DenyReason = "The initial public key is malformed or invalid.";
                                }                                    
                            }
                            else
                            {
                                Approval.DenyReason = "The account name can only contain letters and numbers.";
                            }
                        }
                        else
                        {
                            Approval.DenyReason = "The account name must be at least 1 character long.";
                        }
                    }                       
                    else
                    {
                        Approval.DenyReason = "The account name must be 50 characters long or less.";
                    }
                }
                else
                {
                    Approval.DenyReason = "The request data sent does not match the signature.";
                }
            }
            else
            {
                Approval.DenyReason = "Only the Automate Earth account creation account can create accounts.";
            }

            return Approval;
        }
    }
}
