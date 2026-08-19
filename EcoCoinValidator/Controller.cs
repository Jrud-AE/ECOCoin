using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EcoCoinSharedTypes;
using EcoCoinValidator.Account;

namespace EcoCoinValidator
{
    internal class Controller
    {
        public bool ValidateTransaction(TransactionRequest TranReq, byte[] TransactionSignature)
        {
            bool Approval = false;

            switch (TranReq.RequestType)
            {
                case RequestType.CreateAccount:
                    Approval = AccountCreation.Validate(TranReq, TransactionSignature);
                    break;
                case RequestType.AddKey:
                    Approval = AddKey.Validate(TranReq, TransactionSignature);
                    break;
            }

            return Approval;
        }
    }
}
