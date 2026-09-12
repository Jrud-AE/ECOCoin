using EcoCoinSharedTypes;
using System.Text.Json;

namespace EcoWallet
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("CreateAccountPage", typeof(CreateAccountPage));
            Routing.RegisterRoute("WaitingForTransaction", typeof(WaitingForTransaction));

            EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "G:/EcoCoinDataTest/";


            EcoCoinSharedTypes.GlobalVars.ECOWalletConfiguration = System.Text.Json.JsonSerializer.Deserialize<ECOWalletConfig>(Preferences.Default.Get("WalletConfig", JsonSerializer.Serialize(new ECOWalletConfig())).ToString());

        }
    }
}
