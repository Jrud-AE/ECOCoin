using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinValidator.Account
{
    public class AddKey
    {
        public static TransactionValidationResponse Validate(TransactionRequest TranReq, byte[] TransactionSignature)
        {
            TransactionValidationResponse Approval = new TransactionValidationResponse() { Approved = false };
            
            AccountDetails SignerAccount = new AccountDetails(TranReq.TransactionSignerID);

            KeyPair ValidKeyPair = SignerAccount.ApprovedKeys[TranReq.TransactionSignerKeyID];


            //CHECK 1: Verify that the signing key has permission to create keys
            if (ValidKeyPair.Permissions.KeyCreationPermission)
            {
                //CHECK 2: Is the key already on the account?
                if (!IsKeyAlreadyOnAccount(SignerAccount, TranReq.NewPublicKey))
                {
                    //CHECK 3: Is the key malformed somehow?
                    if (IsKeyValid(TranReq.NewPublicKey))
                    {
                        Approval.Approved = true;
                    }
                    else
                    {
                        Approval.DenyReason = "The key is malformed or invalid.";
                    }
                }
                else
                {
                    Approval.DenyReason = "The key is already on the account.";
                }
            }
            else
            {
                Approval.DenyReason = "The signing key does not have permission to create keys.";
            }

            return Approval;
        }

        public static bool IsKeyAlreadyOnAccount(AccountDetails SignerAccount, string PublicKey)
        {
            bool KeyAlreadyExists = false;
            foreach (KeyPair KP in SignerAccount.ApprovedKeys)
            {
                if (KP.PublicKey == PublicKey)
                {
                    KeyAlreadyExists = true;
                    break;
                }
            }
            return KeyAlreadyExists;
        }
        public static bool IsKeyValid(string pemKey)
        {
            bool Valid = false;

            if (!string.IsNullOrWhiteSpace(pemKey))
            {

                try
                {
                    // Create an ephemeral RSA instance
                    using var rsa = RSA.Create();

                    // This parses both standard SubjectPublicKeyInfo and PKCS#1 RSA public keys
                    rsa.ImportFromPem(pemKey.AsSpan());

                    // If it imports without throwing, the key structure is valid
                    Valid = true;
                }
                catch (Exception)
                {
                    // The key is corrupted, malformed, or mathematically invalid
                    return false;
                }
            }

            return Valid;
        }
    }
}
