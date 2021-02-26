using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users
{
    [ApiController, Route("users/[controller]")]
    public class CheckTokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] LoginResponseModel request)
        {
            if (request == null) return BadRequest();
            var tokens = Program.DataContext.Tokens;
            try
            {
                var unused = tokens.First(t => t.Token == request.Token && t.User.Username == request.Username);
                return new JsonResult(JsonConvert.SerializeObject(true));
            }
            catch (Exception)
            {
                return BadRequest("Bad token");
            }
        }
    }
}