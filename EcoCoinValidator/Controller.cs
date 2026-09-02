using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EcoCoinSharedTypes;
using EcoCoinValidator.Account;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcoCoinValidator
{
    public class Controller
    {
        MessageReceiver MR;

        public Controller()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                GlobalVars.EnvironmentType = EnvironmentType.Test;
            }
            else
            {
                GlobalVars.EnvironmentType = EnvironmentType.Production;
            }

            if (GlobalVars.EnvironmentType == EnvironmentType.Production)
            {
                GlobalVars.AccountStoragePath = "G:/EcoCoinData/ChainData/Accounts/";
                GlobalVars.ECORootStoragePath = "G:/EcoCoinData/";
                GlobalVars.AEOfficialServerAccount = Guid.Parse("086e33a8-d884-4b6f-ac37-5afd81091807");
                GlobalVars.AEAccountCreationAccount = Guid.Parse("a6479df0-445a-4376-b3ed-6dd89fc51cf9");
                GlobalVars.LocalSigningKeyID = 0;
            }
            else
            {
                GlobalVars.AccountStoragePath = "G:/EcoCoinDataTest/ChainData/Accounts/";
                GlobalVars.ECORootStoragePath = "G:/EcoCoinDataTest/";
                GlobalVars.AEOfficialServerAccount = Guid.Parse("43cfec16-6306-4a13-8b63-b6fbcd3f96af");
                GlobalVars.AEAccountCreationAccount = Guid.Parse("54b94908-6924-4b0b-9b1b-b4818184acc1");
                GlobalVars.LocalSigningKeyID = 0;
            }

            MR = new MessageReceiver();
        }

       


    }
}
