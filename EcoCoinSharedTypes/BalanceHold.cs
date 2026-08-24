using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class BalanceHold
    {
        private decimal dHoldAmount;
        private HoldReason eHoldReason;

        public decimal HoldAmount
        {
            get { return dHoldAmount; }
            set { dHoldAmount = value; }
        }

        public HoldReason HoldReason
        {
            get { return eHoldReason; }
            set { eHoldReason = value; }
        }
    }
}
