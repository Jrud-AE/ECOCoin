using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class GlobalFunctions
    {
        public static byte[] SerializeObjectToByteArray(object obj)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        public static byte[] HashFile(string FilePath)
        {
            byte[] hashBytes;

            using (FileStream stream = File.OpenRead(FilePath)) 
            {
                hashBytes = SHA256.HashData(stream);
            }

            return hashBytes;
        }

        public static string HashFileAsString(string FilePath)
        {
            return Convert.ToHexString(HashFile(FilePath));
        }

        public static byte[] GenerateCryptoHashForObject(object obj, Guid SigningAccount = default, int AccountKeyID = -1)
        {
            RSA TKey = RSA.Create();

            if (SigningAccount == default)
            {
                SigningAccount = GlobalVars.AEOfficialServerAccount;
                AccountKeyID = GlobalVars.LocalSigningKeyID;
            }

            AccountDetails SigningAccountDetails = new AccountDetails(SigningAccount);

            TKey.ImportFromPem(SigningAccountDetails.ApprovedKeys[AccountKeyID].PrivateKey.ToCharArray());

            return TKey.SignData(GlobalFunctions.SerializeObjectToByteArray(obj), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public static string GenerateCryptoHashForObjectAsString(object obj, Guid SigningAccount = default, int AccountKeyID = -1)
        {
            return Convert.ToHexString(GenerateCryptoHashForObject(obj, SigningAccount, AccountKeyID));
        }
    }
}
