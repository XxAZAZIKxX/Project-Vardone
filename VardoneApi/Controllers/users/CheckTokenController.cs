using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users
{
    [ApiController, Route("users/[controller]")]
    public class CheckTokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] TokenUserModel request)
        {
            if (request == null) return BadRequest();
            if (Core.UserChecks.CheckToken(request)) return new JsonResult(JsonConvert.SerializeObject(true));
            return BadRequest();
        }
    }
}