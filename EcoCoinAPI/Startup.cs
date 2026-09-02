using EcoCoinSharedTypes;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json.Linq;

namespace EcoCoinAPI
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

            
        }

        public static void SetGlobalVars()
        {
            var Settings = (JObject.Parse(System.IO.File.ReadAllText(AppContext.BaseDirectory + "appsettings.json")));

            if (Settings["Environment"].ToString() == "TEST")
            {
                EcoCoinSharedTypes.GlobalVars.EnvironmentType = EnvironmentType.Test;
            }
            else
            {
                EcoCoinSharedTypes.GlobalVars.EnvironmentType = EnvironmentType.Production;
            }

            if (EcoCoinSharedTypes.GlobalVars.EnvironmentType == EnvironmentType.Production)
            {
                EcoCoinSharedTypes.GlobalVars.AccountStoragePath = "C:\\EcoCoinData\\ChainData\\Accounts\\";
                EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "C:\\EcoCoinData\\";
                EcoCoinSharedTypes.GlobalVars.AEOfficialServerAccount = Guid.Parse("086e33a8-d884-4b6f-ac37-5afd81091807");
                EcoCoinSharedTypes.GlobalVars.AEAccountCreationAccount = Guid.Parse("a6479df0-445a-4376-b3ed-6dd89fc51cf9");
                EcoCoinSharedTypes.GlobalVars.LocalSigningKeyID = 0;
            }
            else
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    EcoCoinSharedTypes.GlobalVars.AccountStoragePath = "G:\\EcoCoinDataTest\\ChainData\\Accounts\\";
                    EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "G:\\EcoCoinDataTest\\";
                    EcoCoinSharedTypes.GlobalVars.AEOfficialServerAccount = Guid.Parse("43cfec16-6306-4a13-8b63-b6fbcd3f96af");
                    EcoCoinSharedTypes.GlobalVars.AEAccountCreationAccount = Guid.Parse("54b94908-6924-4b0b-9b1b-b4818184acc1");
                    EcoCoinSharedTypes.GlobalVars.LocalSigningKeyID = 0;
                }
                else
                {
                    EcoCoinSharedTypes.GlobalVars.AccountStoragePath = "C:\\EcoCoinDataTest\\ChainData\\Accounts\\";
                    EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "C:\\EcoCoinDataTest\\";
                    EcoCoinSharedTypes.GlobalVars.AEOfficialServerAccount = Guid.Parse("43cfec16-6306-4a13-8b63-b6fbcd3f96af");
                    EcoCoinSharedTypes.GlobalVars.AEAccountCreationAccount = Guid.Parse("54b94908-6924-4b0b-9b1b-b4818184acc1");
                    EcoCoinSharedTypes.GlobalVars.LocalSigningKeyID = 0;
                }

            }

        }
    }
}
