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
        public static bool Validate(TransactionRequest TranReq, byte[] TransactionSignature)
        {
            bool Approval = false;

            //CHECK 1: only the Automate Earth account creation account can create accounts.
            if (TranReq.TransactionSignerID == Guid.Parse("a6479df0-445a-4376-b3ed-6dd89fc51cf9"))
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
                    if (TranReq.AccountName.Length < 50)
                    {
                        //CHECK 4: Verify that the account name is at least 1 characters
                        if (TranReq.AccountName.Length > 0)
                        {
                            //CHECK 5: Verify that the account name only contains approved characters
                            string ApprovedChars = "^[a-zA-Z0-9]+$";

                            if (Regex.IsMatch(TranReq.AccountName, ApprovedChars))
                            {
                                Approval = true;
                            }
                        }
                    }                        
                }
                
            }

            return Approval;
        }
    }
}
