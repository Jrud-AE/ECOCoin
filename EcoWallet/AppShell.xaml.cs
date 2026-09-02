using EcoCoinSharedTypes;

namespace EcoWallet
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("CreateAccountPage", typeof(CreateAccountPage));

            GlobalVars.ECORootStoragePath = "G:/EcoCoinDataTest/";

            if (!System.IO.File.Exists(GlobalVars.ECORootStoragePath + "ECOWalletConfig.json"))
            {
                ECOWalletConfig WC = new ECOWalletConfig();

                WC.SaveToFile();

                GlobalVars.ECOWalletConfiguration = WC;
            }
            else
            {
                GlobalVars.ECOWalletConfiguration = ECOWalletConfig.LoadFromFile();
            }
        }
    }
}
