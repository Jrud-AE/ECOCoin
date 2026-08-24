using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinValidator
{
    internal class MessageReceiver
    {
        private Socket JobSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Thread JobThread;

        internal void Start()
        {
            ConnectToJobServer();
        }

        private void ConnectToJobServer()
        {
            JobSocket.Connect("https://EcoCoinJobRouter.AutomateEarth.com", 90);

            JobThread = new Thread(new ThreadStart(WaitForJobs));

            JobThread.Start();
        }

        private void WaitForJobs()
        {
            byte[] buffer = new byte[JobSocket.ReceiveBufferSize];

            while (true)
            {
                JobSocket.Receive(buffer);

                TransactionRequestEnvelope TranReqW = System.Text.Json.JsonSerializer.Deserialize<TransactionRequestEnvelope>(buffer);

                bool Result = RequestValidationRouter.ValidateTransaction(TranReqW.Request, TranReqW.TransactionSignature);
            }
        }


    }
}
