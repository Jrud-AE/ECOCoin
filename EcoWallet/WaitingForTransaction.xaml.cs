using EcoCoinSharedTypes;
using System.Net;
using System.Text.Json;
using System.Timers;

namespace EcoWallet;

public partial class WaitingForTransaction : ContentPage
{
	public static Guid CurrentlyWaitingOnTransaction;
    public static string PrivateKeyForAccountCreation;

    public WaitingForTransaction()
	{
		InitializeComponent();
	}

    System.Timers.Timer PollTimer;

    internal void OnPageLoad(object Sender, EventArgs E)
    {
        if (PollTimer == null)
        {
            PollTimer = new System.Timers.Timer(500);
        }

        PollTimer.Elapsed += PollTransaction;

        PollTimer.Start();

        lblID.Text = "ID: " + CurrentlyWaitingOnTransaction.ToString();
    }

    public void PollTransaction(object Sender, ElapsedEventArgs e)
    {
        TransactionRequestStatus TRS = GetTransactionStatus();

        MainThread.BeginInvokeOnMainThread(() =>
        {

            lblID.Text = "ID: " + CurrentlyWaitingOnTransaction.ToString();
            lblType.Text = "Type: " + TRS.TransactionRequestType.ToString();
            lblStatus.Text = "Status: " + TRS.Status.ToString();
            lblTiming.Text = "Started: " + TRS.TransactionStartDate.ToString() + " (" + (DateTime.Now - TRS.TransactionStartDate).TotalSeconds.ToString() + " Seconds)";
            lblValidationStatus.Text = "Validation Status: " + TRS.ApproveValidatorCount.ToString() + " Approvers, " + TRS.DenyValidatorCount.ToString() + " Deniers"; 

        });

        if (TRS.Status == TransactionStatus.Approved)
        {
            PollTimer.Stop();
            Thread.Sleep(2000);

            AccountDetails AD = new AccountDetails(TRS.NewAccountID);

            AD.ApprovedKeys[0].PrivateKey = PrivateKeyForAccountCreation;

            AD.SaveAccountToFile();

            EcoCoinSharedTypes.GlobalVars.ECOWalletConfiguration.Accounts.Add(AD.AccountID);

            Preferences.Default.Set("WalletConfig", JsonSerializer.Serialize(EcoCoinSharedTypes.GlobalVars.ECOWalletConfiguration));

            MainThread.BeginInvokeOnMainThread(static () =>
            {
                Shell.Current.Navigation.PopAsync(true);
            });
        }
        else if (TRS.Status == TransactionStatus.Denied)
        {
            PollTimer.Stop();
            Thread.Sleep(4000);
            MainThread.BeginInvokeOnMainThread(static () =>
            {
                Shell.Current.Navigation.PopAsync(true);
            });
        }
        else if (TRS.Status == TransactionStatus.Errored)
        {
            PollTimer.Stop();
            Thread.Sleep(4000);
            MainThread.BeginInvokeOnMainThread(static () =>
            {
                Shell.Current.Navigation.PopAsync(true);
            });
        }
    }

	public TransactionRequestStatus GetTransactionStatus()
    {
        WebRequest WR = WebRequest.Create(EcoCoinSharedTypes.GlobalVars.ServerURLBase + "/api/TransactionBroadcast/CheckTransactionStatus?TransactionRequestID=" + CurrentlyWaitingOnTransaction.ToString());

        TransactionRequestStatus TRS;

        using (WebResponse Resp = WR.GetResponse())
        {
            using (Stream S = Resp.GetResponseStream())
            {
                using (StreamReader SR = new StreamReader(S))
                {
                    string sResponse = SR.ReadToEnd();
                    sResponse = sResponse.Replace("transaction", "Transaction").Replace("newAccount", "NewAccount").Replace("status", "Status").Replace("approveValidator", "ApproveValidator").Replace("denyValidator", "DenyValidator");
                    TRS = System.Text.Json.JsonSerializer.Deserialize<TransactionRequestStatus>(sResponse);
                }
            }
        }
        return TRS;
    }


}