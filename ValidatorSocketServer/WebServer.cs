using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
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
            List<byte> messageBuffer = new List<byte>();

            while (WebServerSocket.Connected)
            {
                try
                {
                    byte[] buffer = new byte[WebServerSocket.Available];

                    if (buffer.Length == 0)
                    {
                        buffer = new byte[1];
                    }

                    WebServerSocket.Receive(buffer);

                    foreach (byte b in buffer)
                    {
                        messageBuffer.Add(b);

                        if (System.Text.UTF8Encoding.UTF8.GetString(messageBuffer.ToArray()).EndsWith("☺"))
                        {
                            messageBuffer.RemoveAt(messageBuffer.Count - 1);
                            messageBuffer.RemoveAt(messageBuffer.Count - 1);
                            messageBuffer.RemoveAt(messageBuffer.Count - 1);
                            
                            TransactionRequestEnvelope TRE = System.Text.Json.JsonSerializer.Deserialize<TransactionRequestEnvelope>(messageBuffer.ToArray());

                            messageBuffer.Clear();

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(DateTime.Now.ToString() + " - Transaction Request " + TRE.Request.TransactionID.ToString() + " Arrived, passing to validators.");

                            Controller.BroadcastTransactionRequestToValidators(TRE);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "An existing connection was forcibly closed by the remote host." || ex.Message == "A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using a sendto call) no address was supplied.")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Web server dropped connection");
                        try
                        {
                            Controller.ServerConnections.Remove(this);
                        }
                        catch (Exception ex2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error removing web server connection from controller list: " + ex2.Message);
                        }

                        try
                        {
                            WebServerSocket.Dispose();
                        }
                        catch (Exception ex2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error disposing web server connection: " + ex2.Message);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error while receiving job from web Server: " + ex.Message);
                    }
                }
            }
        }
    }
}

