using EcoCoinSharedTypes;
using GoogleGson;
using System.Net;

namespace EcoWallet;

public partial class WaitingForTransaction : ContentPage
{
	public Guid CurrentlyWaitingOnTransaction;

    public WaitingForTransaction()
	{
		InitializeComponent();
	}

	public TransactionRequestStatus GetTransactionStatus()
    {
        WebRequest WR = WebRequest.Create("https://ecocoinapi.automateearth.com/api/TransactionBroadcast/CheckTransactionStatus?TransactionRequestID=" + CurrentlyWaitingOnTransaction.ToString());

        TransactionRequestStatus TRS;

        using (WebResponse Resp = WR.GetResponse())
        {
            using (Stream S = Resp.GetResponseStream())
            {
                using (StreamReader SR = new StreamReader(S))
                {
                    string sResponse = SR.ReadToEnd();
                    TRS = System.Text.Json.JsonSerializer.Deserialize<TransactionRequestStatus>(sResponse);
                }
            }
        }
        return TRS;
    }
}