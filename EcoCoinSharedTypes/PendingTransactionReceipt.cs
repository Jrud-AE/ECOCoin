using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class PendingTransactionReceipt
    {
        private Guid gTransactionID;
        private DateTime dtTransactionDueDate;

        public PendingTransactionReceipt()
        {
            dtTransactionDueDate = DateTime.UtcNow.AddSeconds(5);
        }
        public PendingTransactionReceipt(Guid TransactionID)
        {
            gTransactionID = TransactionID;
            dtTransactionDueDate = DateTime.UtcNow.AddSeconds(5);
        }

        public Guid TransactionID
        {
            get { return gTransactionID; }
            set { gTransactionID = value; }
        }
        public DateTime TransactionDueDate
        {
            get { return dtTransactionDueDate; }
            set { dtTransactionDueDate = value; }
        }
    }

    public class PendingTransactionReceiptEnvelope
    {
        private PendingTransactionReceipt ptrReceipt;
        private byte[] bTransactionSignature;

        public PendingTransactionReceiptEnvelope(PendingTransactionReceipt Receipt, byte[] TransactionSignature)
        {
            ptrReceipt = Receipt;
            bTransactionSignature = TransactionSignature;
        }
        public PendingTransactionReceipt Receipt
        {
            get { return ptrReceipt; }
            set { ptrReceipt = value; }
        }
        public byte[] TransactionSignature
        {
            get { return bTransactionSignature; }
            set { bTransactionSignature = value; }
        }
    }
}
