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
        public IActionResult Post([FromBody] TokenUserModel request)
        {
            if (request == null) return BadRequest();

            if (CheckToken(request)) return new JsonResult(JsonConvert.SerializeObject(true));

            return BadRequest();
        }

        public static bool CheckToken(TokenUserModel token)
        {
            if (token == null) return false;
            var tokens = Program.DataContext.Tokens;
            try
            {
                var unused = tokens.First(t => t.Token == token.Token && t.User.Username == token.Username);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}