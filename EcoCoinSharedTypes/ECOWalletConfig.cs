using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class ECOWalletConfig
    {
        private List<AccountDetails> lAccounts;

        public ECOWalletConfig()
        {
            lAccounts = new List<AccountDetails>();
        }

        public List<AccountDetails> Accounts
        {
            get 
            { 
                return lAccounts; 
            }
            set
            {
                lAccounts = value;
            }
        }

        public void SaveToFile()
        {
            using (System.IO.FileStream FS = new FileStream(GlobalVars.ECORootStoragePath + "ECOWalletConfig.json", FileMode.Create))
            {
                byte[] buffer = GlobalFunctions.SerializeObjectToByteArray(this);

                FS.Write(buffer, 0, buffer.Length);
            }
        }

        public static ECOWalletConfig LoadFromFile()
        {
            using (System.IO.FileStream FS = new FileStream(GlobalVars.ECORootStoragePath + "ECOWalletConfig.json", FileMode.Open))
            {
                byte[] Data = new byte[FS.Length];

                FS.Read(Data, 0, Data.Length);

                ECOWalletConfig WC = System.Text.Json.JsonSerializer.Deserialize<ECOWalletConfig>(Data);

                return WC;
            }
        }
    }
}
