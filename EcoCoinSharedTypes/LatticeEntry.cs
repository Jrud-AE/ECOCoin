using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class LatticeEntry
    {
        private Guid gTransactionID;
        private Guid gAccountID;
        private long lNOnce;
        private string sInitialPublicKey;
        private string sNewPublicKey;
        private RequestType eRequestType;
        private Guid gTransactionInitiatorAccountID;
        private int iTransactionInitiatorKeyID;
        private List<Guid> lApprovingValidatorIDs = new List<Guid>();
        private List<Guid> lDisapprovingValidatorIDs = new List<Guid>();
        private DateTime dtTransactionSubmissionTime;
        private DateTime dtTransactionApprovalTime;

        public DateTime TransactionSubmissionTime
        {
            get { return dtTransactionSubmissionTime; }
            set { dtTransactionSubmissionTime = value; }
        }
        public DateTime TransactionApprovalTime
        {
            get { return dtTransactionApprovalTime; }
            set { dtTransactionApprovalTime = value; }
        }
        public decimal TimeToApproval
        {
            get
            { 
                decimal tta = (decimal)(dtTransactionApprovalTime - dtTransactionSubmissionTime).TotalSeconds;
                tta += (decimal)(dtTransactionApprovalTime - dtTransactionSubmissionTime).Milliseconds / 1000;
                return tta;
            }
        }
        public Guid TransactionID
        {
            get { return gTransactionID; }
            set { gTransactionID = value; }
        }
        public Guid AccountID
        {
            get { return gAccountID; }
            set { gAccountID = value; }
        }
        public long NOnce
        {
            get { return lNOnce; }
            set { lNOnce = value; }
        }
        public string InitialPublicKey
        {
            get { return sInitialPublicKey; }
            set { sInitialPublicKey = value; }
        }
        public RequestType RequestType
        {
            get { return eRequestType; }
            set { eRequestType = value; }
        }
        public string NewPublicKey
        {
            get { return sNewPublicKey; }
            set { sNewPublicKey = value; }
        }
        public Guid TransactionInitiatorAccountID
        {
            get { return gTransactionInitiatorAccountID; }
            set { gTransactionInitiatorAccountID = value; }
        }
        public int TransactionInitiatorKeyID
        {
            get { return iTransactionInitiatorKeyID; }
            set { iTransactionInitiatorKeyID = value; }
        }
        public List<Guid> ApprovingValidators
        {
            get 
            { 
                if (lApprovingValidatorIDs == null)
                {
                    lApprovingValidatorIDs = new List<Guid>();
                }

                return lApprovingValidatorIDs; 
            }
            set { lApprovingValidatorIDs = value; }
        }
        public List<Guid> DisapprovingValidators
        {
            get 
            { 
                if (lDisapprovingValidatorIDs == null)
                {
                    lDisapprovingValidatorIDs = new List<Guid>();
                }
                return lDisapprovingValidatorIDs; 
            }
            set { lDisapprovingValidatorIDs = value; }
        }
        private string GenerateApproverList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Guid Approver in lApprovingValidatorIDs)
            {
                sb.Append(Approver.ToString() + ",");
            }

            return sb.ToString().TrimEnd(',');
        }
        private string GenerateDenierList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Guid Denier in lDisapprovingValidatorIDs)
            {
                sb.Append(Denier.ToString() + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        public void AppendEntryToLatticeFile()
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("{");
            SB.AppendLine("TransactionID=" + gTransactionID.ToString() + ",");
            SB.AppendLine("NOnce=" + lNOnce.ToString() + ",");
            SB.AppendLine("TimeStamp=" + DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture) + ",");
            SB.AppendLine("TransactionInitiatorAccount=" + gTransactionInitiatorAccountID.ToString() + ",");
            SB.AppendLine("TransactionInitiatorKeyID=" + iTransactionInitiatorKeyID.ToString() + ",");
            SB.AppendLine("TransactionSubmissionTimestamp=" + dtTransactionSubmissionTime.ToString("o", System.Globalization.CultureInfo.InvariantCulture) + ",");
            SB.AppendLine("TransactionApprovalTimestamp=" + dtTransactionApprovalTime.ToString("o", System.Globalization.CultureInfo.InvariantCulture) + ",");
            SB.AppendLine("TransactionProcessingTime=" + TimeToApproval.ToString() + ",");

            switch (eRequestType)
            {
                case RequestType.CreateAccount:
                    SB.AppendLine("TransactionType=CreateAccount,");
                    SB.AppendLine("InitialPublicKey=" + sInitialPublicKey + ",");
                    break;
                case RequestType.AddKey:
                    SB.AppendLine("TransactionType=AddKey,");
                    SB.AppendLine("NewPublicKey=" + sNewPublicKey + ",");
                    break;
                default:
                    SB.AppendLine("TransactionType=Unknown,");
                    break;
            }

            SB.AppendLine("CurrentAccountHash=" + GlobalFunctions.HashFileAsString(GlobalVars.AccountStoragePath + gAccountID.ToString() + ".acc") + ",");
            SB.AppendLine("Validation={Approvers=[" + GenerateApproverList() + "],Deniers=[" + GenerateDenierList() + "]}");

            SB.Append("}");

            List<string> lLines = new List<string>();

            lLines.Add(SB.ToString());

            System.IO.File.AppendAllLines(GlobalVars.AccountStoragePath + gAccountID.ToString() + ".lat", lLines);
        }

        public LatticeEntry(TransactionRequest TR)
        {
            gTransactionID = TR.TransactionID;
            gAccountID = TR.AccountID;
            lNOnce = TR.NOnce;
            sInitialPublicKey = TR.InitialPublicKey;
            sNewPublicKey = TR.NewPublicKey;
            eRequestType = TR.RequestType;
            gTransactionInitiatorAccountID = TR.TransactionSignerID;
            iTransactionInitiatorKeyID = TR.TransactionSignerKeyID;
        }

        public LatticeEntry()
        {

        }

    }
}
