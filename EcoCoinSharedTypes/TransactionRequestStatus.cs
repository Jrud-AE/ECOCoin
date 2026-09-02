using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class TransactionRequestStatus
    {
        private Guid gTransactionRequestID;
        private DateTime dtTransactionStartDate;
        private DateTime dtTransactionEndDate;
        private Guid gNewAccountID;
        private bool bApproved;
        private int iApproveValidatorCount;
        private int iDenyValidatorCount;

        public Guid TransactionRequestID
        {
            get { return gTransactionRequestID; }
            set { gTransactionRequestID = value; }
        }
        public DateTime TransactionStartDate
        {
            get { return dtTransactionStartDate; }
            set { dtTransactionStartDate = value; }
        }
        public DateTime TransactionEndDate
        {
            get { return dtTransactionEndDate; }
            set { dtTransactionEndDate = value; }
        }
        public Guid NewAccountID
        {
            get { return gNewAccountID; }
            set { gNewAccountID = value; }
        }
        public bool Approved
        {
            get { return bApproved; }
            set { bApproved = value; }
        }
        public int ApproveValidatorCount
        {
            get { return iApproveValidatorCount; }
            set { iApproveValidatorCount = value; }
        }
        public int DenyValidatorCount
        {
            get { return iDenyValidatorCount; }
            set { iDenyValidatorCount = value; }
        }
    }
}