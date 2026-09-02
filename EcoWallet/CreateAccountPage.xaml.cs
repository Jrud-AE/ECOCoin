namespace EcoWallet
{
	public partial class CreateAccountPage : ContentPage
	{
		public CreateAccountPage()
		{
			InitializeComponent();

			EcoCoinSharedTypes.KeyPair KP = EcoCoinSharedTypes.KeyPair.CreateKey();

			txtPrivateKey.Text = KP.PrivateKey;
		}

        private void btnRegenerate_Clicked(object sender, EventArgs e)
        {
			System.Threading.Thread T = new Thread(SetNewKeyPair);

			T.Start();
        }

		private void SetNewKeyPair()
		{
			EcoCoinSharedTypes.KeyPair KP = EcoCoinSharedTypes.KeyPair.CreateKey();

			MainThread.BeginInvokeOnMainThread(() =>
			{
				txtPrivateKey.Text = KP.PrivateKey;
			});
			
        }

        private void btnCreateAccount_Clicked(object sender, EventArgs e)
        {

        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}