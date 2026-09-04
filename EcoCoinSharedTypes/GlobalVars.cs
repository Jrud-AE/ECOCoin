namespace EcoCoinSharedTypes
{
    public class GlobalVars
    {
        public static EnvironmentType EnvironmentType;
        public static string ECORootStoragePath;
        public static string AccountStoragePath;
        public static Guid AEOfficialServerAccount;
        public static Guid AEAccountCreationAccount;
        public static int LocalSigningKeyID;
        public static ECOWalletConfig ECOWalletConfiguration;
        private static object Conch = new object();

        private static GenericDataAccessClassCore.DBMC oDB;

        internal static GenericDataAccessClassCore.DBMC DB
        {
            get
            {
                if (oDB == null)
                {
                    lock (Conch)
                    {
                        if (oDB == null)
                        {
                            EcoCoinSharedTypes.DBSecrets Secrets = new EcoCoinSharedTypes.DBSecrets();
                            Secrets.LoadSecrets();

                            oDB = new GenericDataAccessClassCore.DBMC("Server=" + Secrets.ServerAddress + ";Database=" + Secrets.Database + ";User Id=" + Secrets.Username + ";Password=" + Secrets.Password + ";", "ValidatorSocketServer", true);
                        }
                    }
                }

                return oDB;
            }
        }
    }

    public enum EnvironmentType
    {
        Production = 0,
        Test = 1
    }
}
