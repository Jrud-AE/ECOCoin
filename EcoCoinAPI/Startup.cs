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
            GlobalVars.AccountStoragePath = "G:\\EcoCoinData\\ChainData\\Accounts\\";
        }
    }
}
