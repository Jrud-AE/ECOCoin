using EcoCoinSharedTypes;
using Microsoft.AspNetCore.Builder;

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
            if (System.Diagnostics.Debugger.IsAttached)
            {
                GlobalVars.AccountStoragePath = "G:\\EcoCoinData\\ChainData\\Accounts\\";
            }
            else
            {
                GlobalVars.AccountStoragePath = "C:\\EcoCoinData\\ChainData\\Accounts\\";
            }

            GlobalVars.LocalSigningAccountID = Guid.Parse("086e33a8-d884-4b6f-ac37-5afd81091807");
            GlobalVars.LocalSigningKeyID = 0;
        }
    }
}
