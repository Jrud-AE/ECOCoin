using Microsoft.AspNetCore.Mvc;

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
