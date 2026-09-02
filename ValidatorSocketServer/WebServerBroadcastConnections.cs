using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorSocketServer
{
    public class WebServerBroadcastConnections
    {
        private TcpListener Listener;
        private Thread ListenerThread;
        internal WebServerBroadcastConnections()
        {
            bool listenerCreated = false;

            while (!listenerCreated)
            {
                try
                {
                    Listener = TcpListener.Create(5000);
                    listenerCreated = true;
                    Listener.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error creating TCP listener: " + ex.Message);
                }
            }

            ListenerThread = new Thread(new ThreadStart(WaitForWebServerConnections));
            ListenerThread.Start();
        }

        internal void WaitForWebServerConnections()
        {
            Console.WriteLine("Waiting for web server connections on port 5000...");
            while (true)
            {
                try
                {
                    Socket WebServerSocket = Listener.AcceptSocket();
                    WebServer WS = new WebServer(WebServerSocket);
                    Controller.ServerConnections.Add(WS);
                    Console.WriteLine("New Web Server connection: " + WebServerSocket.AddressFamily.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error accepting Web server connections: " + ex.Message);
                }
            }
        }
    }
}
