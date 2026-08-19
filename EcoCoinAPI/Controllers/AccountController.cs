using EcoCoinSharedTypes;
using Microsoft.AspNetCore.Mvc;

namespace EcoCoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet(Name ="AccountCreate")]
        public AccountDetails AccountCreate(string AccountName)
        {
            AccountDetails AD = AccountDetails.CreateAccount(AccountName);

            AD.SaveAccountToFile();

            return AD;
        }

        [HttpGet(Name = "AccountRetrieve")]
        public AccountDetails AccountRetrieve(Guid AccountId)
        {
            AccountDetails AD = new AccountDetails(AccountId);

            foreach (KeyPair K in AD.ApprovedKeys)
            {
                K.PrivateKey = "";
            }

            return AD;
        }
    }


}
