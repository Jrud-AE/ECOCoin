using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorSocketServer
{
    public class ValidatorConnections
    {
        private TcpListener Listener;
        private Thread ListenerThread;
        internal ValidatorConnections()
        {
            bool listenerCreated = false;
            while (!listenerCreated)
            {
                try
                {
                    Listener = TcpListener.Create(5001);
                    listenerCreated = true;
                    Listener.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error creating TCP listener: " + ex.Message);
                }
            }
            ListenerThread = new Thread(new ThreadStart(WaitForValidatorConnections));
            ListenerThread.Start();
        }
        internal void WaitForValidatorConnections()
        {
            Console.WriteLine("Waiting for validator connections on port 5001...");
            while (true)
            {
                try
                { 
                    Socket ValidatorSocket = Listener.AcceptSocket();
                    Validator V = new Validator(ValidatorSocket);
                    Controller.ValidatorConnections.Add(V);
                    Console.WriteLine("Validator Connected: " + V.IPAddress.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error accepting validator connection: " + ex.Message);
                }
            }
        }
    }
}
