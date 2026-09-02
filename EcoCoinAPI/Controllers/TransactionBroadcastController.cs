using EcoCoinSharedTypes;
using GenericDataAccessClassCore;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EcoCoinAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionBroadcastController : ControllerBase
    {
        [HttpGet(Name = "BalanceTransferRequest")]
        public BalanceTransferStub BalanceTransferRequest()
        {
            BalanceTransferStub BTS = new BalanceTransferStub();

            BTS.SystemID = Guid.NewGuid();
            BTS.DueTime = DateTime.Now.AddSeconds(5);

            return BTS;
        }

        [HttpGet(Name = "CheckTrasactionStatus")]
        public TransactionRequestStatus CheckTrasactionStatus(Guid TransactionRequestID)
        {
            SQLParameterCollection Params = new SQLParameterCollection();

            Params.AddParameter("TransactionRequestID", TransactionRequestID);

            DataRow DR = GlobalVars.DB.DBSelect("SELECT TransactionStartDate, NewAccountID, ApproveValidatorCount, DenyValidatorCount Approved, TransactionEndDate FROM TransactionRequests WHERE TransactionID = @TransactionRequestID", Params).Tables[0].Rows[0];

            TransactionRequestStatus TRS = new TransactionRequestStatus();

            TRS.TransactionStartDate = (DateTime)DR["TransactionStartDate"];
            TRS.NewAccountID = (Guid)DR["NewAccountID"];
            TRS.TransactionEndDate = (DateTime)DR["TransactionEndDate"];
            TRS.TransactionRequestID = TransactionRequestID;
            TRS.Approved = (bool)DR["Approved"];
            TRS.ApproveValidatorCount = (int)DR["ApproveValidatorCount"];
            TRS.DenyValidatorCount = (int)DR["DenyValidatorCount"];

            return TRS;
        }
    }

    public class BalanceTransferStub
    {
        private Guid gSystemID;
        private DateTime dtDueTime;

        public Guid SystemID
        {
            get { return gSystemID; }
            set { gSystemID = value; }
        }

        public DateTime DueTime
        {
            get { return dtDueTime; }
            set { dtDueTime = value; }
        }

        
    }
}
