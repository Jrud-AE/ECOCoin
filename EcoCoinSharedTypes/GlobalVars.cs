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
    }

    public enum EnvironmentType
    {
        Production = 0,
        Test = 1
    }
}
