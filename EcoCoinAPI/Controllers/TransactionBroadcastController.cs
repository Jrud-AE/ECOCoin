using EcoCoinSharedTypes;
using GenericDataAccessClassCore;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EcoCoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionBroadcastController : ControllerBase
    {
        [HttpGet("BalanceTransferRequest", Name = "BalanceTransferRequest")]
        public BalanceTransferStub BalanceTransferRequest()
        {
            BalanceTransferStub BTS = new BalanceTransferStub();

            BTS.SystemID = Guid.NewGuid();
            BTS.DueTime = DateTime.Now.AddSeconds(5);

            return BTS;
        }

        [HttpGet("CheckTransactionStatus", Name = "CheckTransactionStatus")]
        public TransactionRequestStatus CheckTransactionStatus(Guid TransactionRequestID)
        {
            SQLParameterCollection Params = new SQLParameterCollection();

            Params.AddParameter("TransactionRequestID", TransactionRequestID);

            DataRow DR = GlobalVars.DB.DBSelect("SELECT TransactionRequestID, TransactionStartDate, NewAccountID, ApproveValidatorCount, DenyValidatorCount, Status, TransactionEndDate, TransactionRequestType FROM TransactionRequests WHERE TransactionRequestID = @TransactionRequestID", Params).Tables[0].Rows[0];

            TransactionRequestStatus TRS = new TransactionRequestStatus(DR);

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
