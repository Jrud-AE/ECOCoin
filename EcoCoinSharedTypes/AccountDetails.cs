namespace EcoCoinSharedTypes
{
    public class AccountDetails
    {
        private Guid gAccountID;
        private string sAccountName;
        private List<KeyPair> lApprovedKeys;

        private bool bAutomateEarthAccountRecoveryProtectionEnabled = false;

        private AccountDetails()
        {

        }

        /// <summary>
        /// loads account from file
        /// </summary>
        /// <param name="AccountID"></param>
        public AccountDetails(Guid AccountID, string MostRecentVersionHash = "")
        {
            this.gAccountID = AccountID;

            //if we have a local copy of the file, then use it
            string FilePath = GlobalVars.AccountStoragePath + gAccountID.ToString() + ".acc";
            if (System.IO.File.Exists(FilePath) && (MostRecentVersionHash == GlobalFunctions.HashFileAsString(FilePath) || true))
            { 
                System.IO.FileStream FS = new FileStream(GlobalVars.AccountStoragePath + gAccountID.ToString() + ".acc", FileMode.Open);

                byte[] Data = new byte[FS.Length];

                FS.Read(Data, 0, Data.Length);

                AccountDetails AccountFile = System.Text.Json.JsonSerializer.Deserialize<AccountDetails>(Data);

                this.sAccountName = AccountFile.sAccountName;
                this.lApprovedKeys = AccountFile.lApprovedKeys;
            }
            else //if we don't have a copy of the file locally or it's out of date, then request it from the Automate Earth server
            {

            }
        }

        public void SaveAccountToFile()
        {
            System.IO.FileStream FS = new FileStream(GlobalVars.AccountStoragePath + gAccountID.ToString() + ".acc", FileMode.Create);

            byte[] buffer = GlobalFunctions.SerializeObjectToByteArray(this);

            FS.Write(buffer, 0, buffer.Length);
        }

        public static AccountDetails CreateAccount(string AccountName)
        {
            AccountDetails AD = new AccountDetails();
            
            AD.gAccountID = Guid.NewGuid();
            AD.sAccountName = AccountName;
            AD.ApprovedKeys = new List<KeyPair>();

            AD.ApprovedKeys.Add(KeyPair.CreateKey());

            AD.ApprovedKeys[0].Permissions.AlterAccountSettingsPermission = true;
            AD.ApprovedKeys[0].Permissions.KeyCreationPermission = true;
            AD.ApprovedKeys[0].Permissions.KeyDeletePermission = true;
            AD.ApprovedKeys[0].Permissions.KeyPermissionModifyPermission = true;

            return AD;
        }

        public Guid AccountID
        {
            get { return gAccountID; }
            set { gAccountID = value; }
        }

        public string AccountName
        {
            get { return sAccountName; }
            set { sAccountName = value; }
        }

        public List<KeyPair> ApprovedKeys
        {
            get { return lApprovedKeys; }
            set { lApprovedKeys = value; }
        }

        public bool AutomateEarthAccountRecoveryProtectionEnabled
        {
            get { return bAutomateEarthAccountRecoveryProtectionEnabled; }
            set { bAutomateEarthAccountRecoveryProtectionEnabled = value; }
        }
    }
}
