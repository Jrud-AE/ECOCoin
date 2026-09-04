using EcoCoinSharedTypes;
using System.Net.Sockets;

namespace EcoCoinAPI
{
    public class GlobalFunctions
    {
        private static Socket SocketServer;

        public static void SendTransactionRequestToValidators(TransactionRequestEnvelope TRE)
        {
            TransactionRequestStatus TRS = new TransactionRequestStatus();

            TRS.Status = TransactionStatus.Initiating;
            TRS.TransactionStartDate = DateTime.Now;
            TRS.TransactionRequestID = TRE.Request.TransactionID;
            TRS.DenyValidatorCount = 0;
            TRS.ApproveValidatorCount = 0;

            TRS.SaveToDB();

            if (SocketServer == null || !SocketServer.Connected)
            {
                ConnectToValidatorServer();
            }

            SocketServer.Send(EcoCoinSharedTypes.GlobalFunctions.SerializeObjectToByteArray(TRE));
        }

        private static void ConnectToValidatorServer()
        {
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketServer.Connect("localhost", 5000);
        }

    }
}
