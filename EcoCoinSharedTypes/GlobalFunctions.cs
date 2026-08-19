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
    }
}
