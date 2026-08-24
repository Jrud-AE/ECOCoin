using System.Net.Sockets;

namespace EcoCoinAPI
{
    public class GlobalFunctions
    {
        private static Socket SocketServer;

        public static void SendTransactionRequestToValidators()
        { 

        }

        private static void ConnectToValidatorServer()
        {
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketServer.Connect("localhost", 5000);
        }

    }
}
