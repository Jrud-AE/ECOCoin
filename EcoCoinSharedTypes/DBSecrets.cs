using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class DBSecrets
    {
        private string sUsername;
        private string sPassword;
        private string sServerAddress;
        private string sDatabase;

        public string Username
        {
            get
            {
                return sUsername;
            }
            set
            {
                sUsername = value;
            }
        }
        public string Password
        {
            get
            {
                return sPassword;
            }
            set
            {
                sPassword = value;
            }
        }
        public string ServerAddress
        {
            get
            {
                return sServerAddress;
            }
            set
            {
                sServerAddress = value;
            }
        }
        public string Database
        {
            get
            {
                return sDatabase;
            }
            set
            {
                sDatabase = value;
            }
        }
        public void SaveDBSecrets()
        {
            System.IO.FileStream FS = new FileStream(GlobalVars.ECORootStoragePath + "DBSecrets.config", FileMode.Create);

            byte[] buffer = GlobalFunctions.SerializeObjectToByteArray(this);

            FS.Write(buffer, 0, buffer.Length);
        }

        public void LoadSecrets()
        {
            using (System.IO.FileStream FS = new FileStream(GlobalVars.ECORootStoragePath + "DBSecrets.config", FileMode.Open))
            {
                byte[] Data = new byte[FS.Length];

                FS.Read(Data, 0, Data.Length);

                DBSecrets DBS = System.Text.Json.JsonSerializer.Deserialize<DBSecrets>(Data);

                this.Username = DBS.Username;
                this.Password = DBS.Password;
                this.ServerAddress = DBS.ServerAddress;
                this.Database = DBS.Database;
            }
        }
    }
}
