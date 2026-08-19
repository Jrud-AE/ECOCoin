using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class TransactionRequest
    {
        private RequestType rtRequestType;
        private string sAccountName;
        private string sAccountID;
        private Guid gTransactionSignerID;
        public TransactionRequest() 
        { 
            
        }

        public RequestType RequestType 
        { 
            get { return rtRequestType; } 
            set { rtRequestType = value; } 
        }
        public string AccountID 
        { 
            get { return sAccountID; } 
            set { sAccountID = value; } 
        }
        public string AccountName 
        { 
            get { return sAccountName; } 
            set { sAccountName = value; } 
        }
        public Guid TransactionSignerID
        {
            get { return gTransactionSignerID; }
            set { gTransactionSignerID = value; }
        }
    }
}
