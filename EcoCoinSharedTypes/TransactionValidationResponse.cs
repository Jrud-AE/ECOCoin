using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class TransactionValidationResponse
    {
        private Guid gTransactionID;
        private bool bApproved;
        private string sDenyReason;

        public bool Approved
        {
            get { return bApproved; }
            set { bApproved = value; }
        }

        public string DenyReason
        {
            get { return sDenyReason; }
            set { sDenyReason = value; }
        }
        public Guid TransactionID
        {
            get { return gTransactionID; }
            set { gTransactionID = value; }
        }
    }

    public class TransactionValidationResponseEnvelope
    {
        private TransactionValidationResponse tValidationResponse;
        private Guid gValidatorID;
        private string sValidatorSignature;
        public TransactionValidationResponse ValidationResponse
        {
            get { return tValidationResponse; }
            set { tValidationResponse = value; }
        }
        public Guid ValidatorID
        {
            get { return gValidatorID; }
            set { gValidatorID = value; }
        }
        public string ValidatorSignature
        {
            get { return sValidatorSignature; }
            set { sValidatorSignature = value; }
        }
    }
}