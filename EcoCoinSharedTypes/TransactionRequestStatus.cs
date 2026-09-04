using System;
using System.Collections.Generic;
using System.Data;
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
        private TransactionStatus eApproved;
        private int iApproveValidatorCount;
        private int iDenyValidatorCount;
        private RequestType eTransactionRequestType;

        public TransactionRequestStatus()
        {

        }

        public static TransactionRequestStatus GetFromServer(Guid TransactionRequestID)
        {
            TransactionRequestStatus TRS = new TransactionRequestStatus();

            return TRS;
        }

        public TransactionRequestStatus(DataRow DRStatus) 
        {
            if (DRStatus["TransactionStartDate"].ToString() != "")
            {
                TransactionStartDate = (DateTime)DRStatus["TransactionStartDate"];
            }
            if (DRStatus["NewAccountID"].ToString() != "")
            {
                NewAccountID = (Guid)DRStatus["NewAccountID"];
            }
            if (DRStatus["TransactionEndDate"].ToString() != "")
            {
                TransactionEndDate = (DateTime)DRStatus["TransactionEndDate"];
            }
            if (DRStatus["TransactionRequestType"].ToString() != "")
            {
                TransactionRequestType = Enum.Parse<RequestType>(DRStatus["TransactionRequestType"].ToString());
            }
            if (DRStatus["ApproveValidatorCount"].ToString() != "")
            {
                ApproveValidatorCount = (int)DRStatus["ApproveValidatorCount"];
            }
            if (DRStatus["DenyValidatorCount"].ToString() != "")
            {
                DenyValidatorCount = (int)DRStatus["DenyValidatorCount"];
            }
            if (DRStatus["Status"].ToString() != "")
            {
                Status = Enum.Parse<TransactionStatus>(DRStatus["Status"].ToString());
            }
            if (DRStatus["TransactionRequestID"].ToString() != "")
            {
                TransactionRequestID = (Guid)DRStatus["TransactionRequestID"];
            }
        }

        public void SaveToDB()
        {
            GenericDataAccessClassCore.SQLParameterCollection Params = new GenericDataAccessClassCore.SQLParameterCollection();

            Params.AddParameter("TransactionRequestID", TransactionRequestID);
            Params.AddParameter("TransactionStartDate", TransactionStartDate);
            Params.AddParameter("NewAccountID", NewAccountID);
            Params.AddParameter("TransactionEndDate", TransactionEndDate);
            Params.AddParameter("Status", Status);
            Params.AddParameter("ApproveValidatorCount", ApproveValidatorCount);
            Params.AddParameter("DenyValidatorCount", DenyValidatorCount);
            Params.AddParameter("TransactionRequestType", TransactionRequestType);

            if (GlobalVars.DB.DBSelect("SELECT COUNT(*) FROM TransactionRequests WHERE TransactionRequestID = @TransactionRequestID", Params).Tables[0].Rows[0][0].ToString() == "0")
            {
                GlobalVars.DB.DBInsert("INSERT INTO TransactionRequests (TransactionRequestID, TransactionStartDate, TransactionEndDate, NewAccountID, Status, ApproveValidatorCount, DenyValidatorCount, TransactionRequestType) VALUES (@TransactionID, @TransactionStartDate, @TransactionEndDate, @NewAccountID, @Status, @ApproveValidatorCount, @DenyValidatorCount, @TransactionRequestType)", Params);
            }
            else
            {
                GlobalVars.DB.DBUpdate("UPDATE TransactionRequests SET TransactionStartDate = @TransactionStartDate, TransactionEndDate = @TransactionEndDate, NewAccountID = @NewAccountID, Status = @Status, ApproveValidatorCount = @ApproveValidatorCount, DenyValidatorCount = @DenyValidatorCount, TransactionRequestType = @TransactionRequestType WHERE TransactionRequestID = @TransactionRequestID", Params);
            }    
            

        }

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
        public TransactionStatus Status
        {
            get { return eApproved; }
            set { eApproved = value; }
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
        public RequestType TransactionRequestType
        {
            get { return eTransactionRequestType; }
            set { eTransactionRequestType = value; }
        }
    }

    public enum TransactionStatus
    {
        Initiating,
        Validating,
        Approved,
        Denied,
        Errored
    }
}