using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorSocketServer
{
    class WebServer
    {
        Socket WebServerSocket;

        public WebServer(Socket WebServerSocket)
        {
            this.WebServerSocket = WebServerSocket;
        }
    }
}
