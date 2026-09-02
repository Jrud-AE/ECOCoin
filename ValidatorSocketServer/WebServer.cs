using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ValidatorSocketServer
{
    class WebServer
    {
        Socket WebServerSocket;
        Thread MessageWatcherThread;

        public WebServer(Socket WebServerSocket)
        {
            this.WebServerSocket = WebServerSocket;
            MessageWatcherThread = new Thread(new ThreadStart(WatchForMessages));
            MessageWatcherThread.Start();
        }

        public int Send(byte[] data)
        {
            return WebServerSocket.Send(data);
        }

        public void WatchForMessages()
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = WebServerSocket.Receive(buffer);

                if (bytesRead > 0)
                {
                    TransactionRequestEnvelope TRE = System.Text.Json.JsonSerializer.Deserialize<TransactionRequestEnvelope>(buffer);

                    Controller.BroadcastTransactionRequestToValidators(TRE);
                }
            }
        }
    }
}
