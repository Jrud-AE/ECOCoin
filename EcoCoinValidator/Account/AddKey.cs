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
        public static bool Validate(TransactionRequest TranReq, byte[] TransactionSignature)
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

            //CHECK 1: Verify that the signing key has permission to create keys
            if (ValidKeyPair.Permissions.KeyCreationPermission)
            {
                Approval = true;
            }

            return Approval;
        }
    }
}
