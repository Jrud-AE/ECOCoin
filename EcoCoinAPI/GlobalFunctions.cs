using EcoCoinSharedTypes;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;

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

            bool SendSuccess = false;
            int Attempts = 0;
            while (!SendSuccess)
            {
                try
                {
                    if (SocketServer == null || !SocketServer.Connected)
                    {
                        ConnectToValidatorServer();
                    }

                    byte[] data = EcoCoinSharedTypes.GlobalFunctions.SerializeObjectToByteArray(TRE);

                    if (data.ToList().Contains(0x01))
                    {
                        throw new Exception("Error, transaction request contains a smiley face!");
                    }

                    SocketServer.Send(data);

                    SocketServer.Send(Encoding.UTF8.GetBytes("☺"));

                    SendSuccess = true;

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("An existing connection was forcibly closed by the remote host."))
                    {
                        ConnectToValidatorServer();
                    }

                    Attempts++;
                    if (Attempts == 3)
                    {
                        throw;
                    }
                }
            }
        }

        private static void ConnectToValidatorServer()
        {
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketServer.Connect("localhost", 5000);
        }

    }
}
