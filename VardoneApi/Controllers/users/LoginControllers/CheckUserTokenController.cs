using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.LoginControllers
{
    [ApiController, Route("users/[controller]")]
    public class CheckUserTokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] UserTokenModel request)
        {
            if (request == null) return BadRequest();
            if (Core.UserChecks.CheckToken(request)) return new JsonResult(JsonConvert.SerializeObject(true));
            return BadRequest();
        }
    }
}