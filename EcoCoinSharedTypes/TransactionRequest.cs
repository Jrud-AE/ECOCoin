using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class TransactionRequest
    {
        private Guid gTransactionID;
        private RequestType rtRequestType;
        private string sAccountName;
        private Guid gAccountID;
        private Guid gTransactionSignerID;
        private string sInitialPublicKey;
        private string sNewPublicKey;
        public TransactionRequest() 
        {
            TransactionID = Guid.NewGuid();
        }

        public RequestType RequestType 
        { 
            get { return rtRequestType; } 
            set { rtRequestType = value; } 
        }
        public Guid AccountID 
        { 
            get { return gAccountID; } 
            set { gAccountID = value; } 
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
        public string InitialPublicKey
        {
            get { return sInitialPublicKey; }
            set { sInitialPublicKey = value; }
        }
        public string NewPublicKey
        {
            get { return sNewPublicKey; }
            set { sNewPublicKey = value; }
        }
        public Guid TransactionID
        {
            get { return gTransactionID; }
            set { gTransactionID = value; }
        }
    }

    public class TransactionRequestEnvelope
    {
        private TransactionRequest trRequest;
        private byte[] bTransactionSignature;

        public TransactionRequest Request 
        {
            get 
            { 
                return trRequest; 
            }
            set
            {
                trRequest = value;
            }
        }
        public byte[] TransactionSignature 
        {
            get
            {
                return bTransactionSignature;
            }
            set
            {
                bTransactionSignature = value;
            }
        }

        public TransactionRequestEnvelope() { }
        public TransactionRequestEnvelope(TransactionRequest Request, byte[] TransactionSignature)
        {
            this.Request = Request;
            this.TransactionSignature = TransactionSignature;
        }
    }
}
