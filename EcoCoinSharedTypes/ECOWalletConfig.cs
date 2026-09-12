using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class ECOWalletConfig
    {
        private List<Guid> lAccounts;
        public EnvironmentType EnvironmentType;

        public ECOWalletConfig()
        {
            lAccounts = new List<Guid>();
        }

        public List<Guid> Accounts
        {
            get 
            { 
                return lAccounts; 
            }
            set
            {
                lAccounts = value;
            }
        }


    }
}
