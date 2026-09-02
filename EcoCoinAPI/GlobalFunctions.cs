using EcoCoinSharedTypes;
using System.Net.Sockets;

namespace EcoCoinAPI
{
    public class GlobalFunctions
    {
        private static Socket SocketServer;

        public static void SendTransactionRequestToValidators(TransactionRequestEnvelope TRE)
        {
            GenericDataAccessClassCore.SQLParameterCollection Params = new GenericDataAccessClassCore.SQLParameterCollection();

            Params.AddParameter("TransactionID", TRE.Request.TransactionID);
            Params.AddParameter("TransactionStartDate", TRE.Request.TransactionStartDate);
            Params.AddParameter("NewAccountID", TRE.Request.AccountID);

            EcoCoinAPI.GlobalVars.DB.DBInsert("INSERT INTO TransactionRequests (TransactionID, TransactionStartDate, NewAccountID) VALUES (@TransactionID, @TransactionStartDate, @NewAccountID)", Params);

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
