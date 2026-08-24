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
        internal static bool ValidateTransaction(TransactionRequest TranReq, byte[] TransactionSignature)
        {
            bool Approval = false;

            if (ValidateSigner(TranReq, TransactionSignature))
            {
                switch (TranReq.RequestType)
                {
                    case RequestType.CreateAccount:
                        Approval = AccountCreation.Validate(TranReq, TransactionSignature);
                        break;
                    case RequestType.AddKey:
                        Approval = AddKey.Validate(TranReq, TransactionSignature);
                        break;
                }
            }

            return Approval;
        }

        /// <summary>
        /// Validate that the request was signed by the suggested signer, and that the request data sent matches the signature.
        /// </summary>
        /// <param name="TranReq"></param>
        /// <param name="TransactionSignature"></param>
        /// <returns></returns>
        private static bool ValidateSigner(TransactionRequest TranReq, byte[] TransactionSignature)
        {
            bool Approval = false;

            AccountDetails SignerAccount = new AccountDetails(TranReq.TransactionSignerID);

            KeyPair ValidKeyPair = null;

            using (RSA rsa = RSA.Create())
            {
                foreach (KeyPair KP in SignerAccount.ApprovedKeys)
                {
                    rsa.ImportFromPem(KP.PublicKey);

                    if (rsa.VerifyData(GlobalFunctions.SerializeObjectToByteArray(TranReq), TransactionSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
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
    }
}
