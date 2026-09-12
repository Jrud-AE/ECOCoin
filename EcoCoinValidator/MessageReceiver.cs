using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public void SendResult(TransactionValidationResponse TVR)
        {
            TransactionValidationResponseEnvelope TVRE = new TransactionValidationResponseEnvelope();
            TVRE.ValidationResponse = TVR;
            TVRE.ValidatorID = Controller.LocalAccountID;
            TVRE.ValidatorSignature = GlobalFunctions.GenerateCryptoHashForObject(TVRE.ValidationResponse, TVRE.ValidatorID, Controller.LocalKeyID);

            JobSocket.Send(EcoCoinSharedTypes.GlobalFunctions.SerializeObjectToByteArray(TVRE));

            JobSocket.Send(Encoding.UTF8.GetBytes("☺"));
        }

        private void ConnectToJobServer()
        {
            JobSocket.DontFragment = true;
            JobSocket.ReceiveTimeout = 60000;
            if (GlobalVars.EnvironmentType == EnvironmentType.Test)
            {
                JobSocket.Connect("EcoCoinAPITestNet.AutomateEarth.com", 5001);
            }
            else
            {
                JobSocket.Connect("EcoCoinAPI.AutomateEarth.com", 6001);
            }
            Console.ForegroundColor = ConsoleColor.Green;

            if (GlobalVars.EnvironmentType == EnvironmentType.Test)
            {
                Console.WriteLine("Connected to Job Server at https://EcoCoinAPITestNet.AutomateEarth.com:5001");
            }
            else
            {
                Console.WriteLine("Connected to Job Server at https://EcoCoinAPI.AutomateEarth.com:6001");
            }

            JobThread = new Thread(new ThreadStart(WaitForJobs));

            JobThread.Start();
        }

        private void WaitForJobs()
        {
            List<byte> messageBuffer = new List<byte>();

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[JobSocket.Available];

                    if (buffer.Length == 0)
                    {
                        buffer = new byte[1];
                    }
                    JobSocket.Receive(buffer);

                    foreach (byte b in buffer)
                    {
                        if (b == 0x01)
                        {
                            TransactionRequestEnvelope TRE = System.Text.Json.JsonSerializer.Deserialize<TransactionRequestEnvelope>(messageBuffer.ToArray());

                            messageBuffer.Clear();

                            TransactionValidationResponse TVR = RequestValidationRouter.ValidateTransaction(TRE.Request, TRE.EnvelopeSignature);

                            Console.WriteLine("    Result: " + TVR.Approved.ToString());
                            if (!TVR.Approved)
                            {
                                Console.WriteLine("    Deny Reason: " + TVR.DenyReason.ToString());
                            }

                            SendResult(TVR);           
                        }
                        else
                        {
                            messageBuffer.Add(b);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException || ex.Message == "An existing connection was forcibly closed by the remote host." || ex.Message == "A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using a sendto call) no address was supplied.")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Lost connection to EcoCoin Validadator Server.  Attempting to reconnect...");
                        //lost connection to server, wait a few seconds and retry

                        bool ReconnectSuccess = false;
                        while (!ReconnectSuccess)
                        {
                            try
                            {
                                JobSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                JobSocket.DontFragment = true;
                                JobSocket.ReceiveTimeout = 60000;
                                if (GlobalVars.EnvironmentType == EnvironmentType.Test)
                                {
                                    JobSocket.Connect("EcoCoinAPITestNet.AutomateEarth.com", 5001);
                                }
                                else
                                {
                                    JobSocket.Connect("EcoCoinAPI.AutomateEarth.com", 6001);
                                }


                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Reconnection Successful.");

                                ReconnectSuccess = true;
                            }
                            catch (Exception ex2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Reconnection attempt failed, retrying in 5 seconds...");
                                Thread.Sleep(5000);
                            }
                        }
                    }
                    else if (ex.Message == "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.")
                    {
                        //This is just the 1 minute timeout happening because the line was quiet.  Ignore.
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error receiving job from Job Server: " + ex.Message);
                    }
                }
            }
        }

    }
}
