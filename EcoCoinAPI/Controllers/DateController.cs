using Microsoft.AspNetCore.Mvc;

namespace EcoCoinAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DateController
    {
        [HttpGet]
        public DateTime Get()
        {
            return DateTime.UtcNow;
        }

    }
}
