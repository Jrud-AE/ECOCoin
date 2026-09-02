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

        public MessageReceiver()
        {
            ConnectToJobServer();
        }

        private void ConnectToJobServer()
        {
            JobSocket.Connect("EcoCoinAPITestNet.AutomateEarth.com", 5001);
            Console.WriteLine("Connected to Job Server at https://EcoCoinAPITest.AutomateEarth.com:5001");

            JobThread = new Thread(new ThreadStart(WaitForJobs));

            JobThread.Start();
        }

        private void WaitForJobs()
        {
            byte[] buffer = new byte[JobSocket.ReceiveBufferSize];

            while (true)
            {
                try
                {
                    JobSocket.Receive(buffer);

                    TransactionRequestEnvelope TRE = System.Text.Json.JsonSerializer.Deserialize<TransactionRequestEnvelope>(buffer);

                    bool Result = RequestValidationRouter.ValidateTransaction(TRE.Request, TRE.EnvelopeSignature).Approved;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error receiving job from Job Server: " + ex.Message);
                }
            }
        }


    }
}
