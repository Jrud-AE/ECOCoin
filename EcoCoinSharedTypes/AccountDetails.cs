using System.Diagnostics.Tracing;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace EcoCoinSharedTypes
{
    public class AccountDetails
    {
        private Guid gAccountID;
        private string sAccountName;
        private List<KeyPair> lApprovedKeys;

        private bool bAutomateEarthAccountRecoveryProtectionEnabled = false;
        private bool bInheritanceLegalHoldActive = false;

        private decimal dPrimaryBalance = 0;
        private List<BalanceHold> lBalanceHolds = new List<BalanceHold>();

        private List<VerifiedMember> lVerifiedMembers = new List<VerifiedMember>();

        public AccountDetails()
        {

        }

        /// <summary>
        /// loads account from file
        /// </summary>
        /// <param name="AccountID"></param>
        public AccountDetails(Guid AccountID, string MostRecentVersionHash = "")
        {
            this.gAccountID = AccountID;

            AccountDetails AccountFile;

            //if we have a local copy of the file, then use it
            string FilePath = GlobalVars.AccountStoragePath + gAccountID.ToString() + ".acc";
            if (System.IO.File.Exists(FilePath) && (MostRecentVersionHash == GlobalFunctions.HashFileAsString(FilePath) || MostRecentVersionHash == ""))
            {
                using (System.IO.FileStream FS = new FileStream(GlobalVars.AccountStoragePath + gAccountID.ToString() + ".acc", FileMode.Open))
                { 
                    byte[] Data = new byte[FS.Length];

                    FS.Read(Data, 0, Data.Length);

                    AccountFile = System.Text.Json.JsonSerializer.Deserialize<AccountDetails>(Data);
                }
            }
            else //if we don't have a copy of the file locally or it's out of date, then request it from the Automate Earth server
            {
                AccountFile = DownloadAccountFile(gAccountID);
            }

            this.sAccountName = AccountFile.sAccountName;
            this.lApprovedKeys = AccountFile.lApprovedKeys;

            this.bAutomateEarthAccountRecoveryProtectionEnabled = AccountFile.bAutomateEarthAccountRecoveryProtectionEnabled;
            this.bInheritanceLegalHoldActive = AccountFile.bInheritanceLegalHoldActive;

            this.dPrimaryBalance = AccountFile.dPrimaryBalance;
            this.lBalanceHolds = AccountFile.lBalanceHolds;
        }

        public static AccountDetails DownloadAccountFile(Guid gAccountID)
        {
            AccountDetails AccountFile;

            WebRequest WR = WebRequest.Create("https://ecocoinapi.automateearth.com/api/Account/AccountRetrieve?AccountId=" + gAccountID.ToString());

            using (WebResponse Response = WR.GetResponse())
            {
                using (StreamReader SR = new StreamReader(Response.GetResponseStream()))
                {
                    string JSON = SR.ReadToEnd();
                    AccountFile = System.Text.Json.JsonSerializer.Deserialize<AccountDetails>(JSON);

                    AccountFile.SaveAccountToFile();
                }
            }

            return AccountFile;
        }

        public void SaveAccountToFile()
        {
            System.IO.FileStream FS = new FileStream(GlobalVars.AccountStoragePath + gAccountID.ToString() + ".acc", FileMode.Create);

            byte[] buffer = GlobalFunctions.SerializeObjectToByteArray(this);

            FS.Write(buffer, 0, buffer.Length);
        }

        public static AccountDetails CreateAccount(string AccountName, string InitialPublicKey)
        {
            AccountDetails AD = new AccountDetails();

            AD.gAccountID = Guid.NewGuid();
            AD.sAccountName = AccountName;
            AD.ApprovedKeys = new List<KeyPair>();

            KeyPair FirstKey = new KeyPair();

            FirstKey.PublicKey = InitialPublicKey;

            AD.ApprovedKeys.Add(FirstKey);

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
        public bool InheritanceLegalHoldActive
        {
            get { return this.bInheritanceLegalHoldActive; }
            set { this.bInheritanceLegalHoldActive = value; }
        }

        public decimal PrimaryBalance
        {
            get { return dPrimaryBalance; }
            set { dPrimaryBalance = value; }
        }

        public List<BalanceHold> BalanceHolds
        {
            get { return lBalanceHolds; }
            set { lBalanceHolds = value; }
        }

        public long LastUsedNOnce
        {
            get
            {
                string[] Lines = System.IO.File.ReadAllLines(EcoCoinSharedTypes.GlobalVars.AccountStoragePath + gAccountID.ToString() + ".lat");

                int i = Lines.Length - 1;
                long NOnce = -1;

                while (!Lines[i].Contains("{"))
                {
                    if (Lines[i].Contains("NOnce="))
                    {
                        NOnce = long.Parse(Lines[i].Replace("NOnce=", "").Replace(",", ""));
                    }
                    i--;
                }

                return NOnce;
            }
        }
        public List<VerifiedMember> VerifiedMembers
        {
            get { return lVerifiedMembers; }
            set { lVerifiedMembers = value; }
        }
    }

    public class AccountDetailsEnvelope
    {
        private AccountDetails adAccountDetails;
        private byte[] bSignature;

        public AccountDetailsEnvelope(AccountDetails AccountDetails, byte[] Signature)
        {
            adAccountDetails = AccountDetails;
            bSignature = Signature;
        }
        public AccountDetails AccountDetails
        {
            get { return adAccountDetails; }
            set { adAccountDetails = value; }
        }
        public byte[] Signature
        {
            get { return bSignature; }
            set { bSignature = value; }
        }
    }
}
