using EcoCoinSharedTypes;
using System.Net;
using System.Net.Security;

namespace EcoWallet
{
	public partial class CreateAccountPage : ContentPage
	{
        private EcoCoinSharedTypes.KeyPair kpKP;
        private bool RedirectToHomeOnNextFocus = false;

		public CreateAccountPage()
		{
			InitializeComponent();

            SetNewKeyPair();
		}

        internal void OnPageRegainFocus(object sender, EventArgs e)
        {
            if (RedirectToHomeOnNextFocus)
            {
                Shell.Current.Navigation.PopAsync(true);
            }
        }

        private void btnRegenerate_Clicked(object sender, EventArgs e)
        {
			System.Threading.Thread T = new Thread(SetNewKeyPair);

			T.Start();
        }

		private void SetNewKeyPair()
		{
			EcoCoinSharedTypes.KeyPair KP = EcoCoinSharedTypes.KeyPair.CreateKey();

            kpKP = KP;

			MainThread.BeginInvokeOnMainThread(() =>
			{
                txtPrivateKey.Text = KP.PrivateKey;
			});
			
        }

        private void btnCreateAccount_Clicked(object sender, EventArgs e)
        {
            if (txtAccountName.Text != null && txtAccountName.Text.Length > 0)
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, SslPolicyErrors) => true;

                WebRequest WR = WebRequest.Create(EcoCoinSharedTypes.GlobalVars.ServerURLBase + "/api/Account/AccountCreate?AccountName=" + txtAccountName.Text + "&InitialPublicKey=" + kpKP.PublicKey.ToString());

                PendingTransactionReceiptEnvelope PTRE;

                using (WebResponse Resp = WR.GetResponse())
                {
                    using (Stream S = Resp.GetResponseStream())
                    {
                        using (StreamReader SR = new StreamReader(S))
                        {
                            string sResponse = SR.ReadToEnd();
                            sResponse = sResponse.Replace("\"receipt\"", "\"Receipt\"").Replace("transactionSignature", "TransactionSignature").Replace("transaction", "Transaction");
                            PTRE = System.Text.Json.JsonSerializer.Deserialize<PendingTransactionReceiptEnvelope>(sResponse);
                        }
                    }
                }

                WaitingForTransaction.CurrentlyWaitingOnTransaction = PTRE.Receipt.TransactionID;
                WaitingForTransaction.PrivateKeyForAccountCreation = kpKP.PrivateKey;

                RedirectToHomeOnNextFocus = true;

                Shell.Current.GoToAsync("WaitingForTransaction");
            }
            else
            {
                txtAccountName.BackgroundColor = Color.FromArgb("ff0000");
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}