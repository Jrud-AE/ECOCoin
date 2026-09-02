using EcoCoinSharedTypes;

namespace EcoWallet
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            if (GlobalVars.ECOWalletConfiguration.Accounts.Count > 0)
            {
                lblAccountNumber.Text = "Account: " + GlobalVars.ECOWalletConfiguration.Accounts[0].AccountName + "(" + GlobalVars.ECOWalletConfiguration.Accounts[0].AccountID.ToString() + ")";
            }
            else
            {
                Shell.Current.GoToAsync("CreateAccountPage");
                //GlobalFunctions.NavigateToPageAndDropNavigation("CreateAccountPage");
            }
        }

        private void OnPageRegainFocus(object sender, EventArgs e)
        {
            if (GlobalVars.ECOWalletConfiguration.Accounts.Count > 0)
            {
                lblAccountNumber.Text = "Account: " + GlobalVars.ECOWalletConfiguration.Accounts[0].AccountName + "(" + GlobalVars.ECOWalletConfiguration.Accounts[0].AccountID.ToString() + ")";
            }
            else
            {
                Shell.Current.GoToAsync("CreateAccountPage");
                //GlobalFunctions.NavigateToPageAndDropNavigation("CreateAccountPage");
            }
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
