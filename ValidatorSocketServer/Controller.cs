using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorSocketServer
{
    public class Controller
    {
        internal static List<Validator> ValidatorConnections = new List<Validator>();
        internal static List<WebServer> 

        internal static void BroadcastTransactionRequestToValidators(TransactionRequestEnvelope TRE)
        {
            foreach (Validator V in ValidatorConnections)
            {
                byte[] data = GlobalFunctions.SerializeObjectToByteArray(TRE);

                V.Send(data);
            }
        }

    }
}
