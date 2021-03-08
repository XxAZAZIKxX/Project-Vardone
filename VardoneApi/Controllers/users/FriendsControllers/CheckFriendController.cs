using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.FriendsControllers
{
    [ApiController, Route("users/[controller]")]
    public class CheckFriendController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token, [FromQuery] string secondUsername)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("Empty username");
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (string.IsNullOrWhiteSpace(secondUsername)) return BadRequest("Empty second username");
            if (username == secondUsername) return BadRequest("Username equal second username");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { Username = username, Token = token }))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.IsUserExists(secondUsername)) return BadRequest("Second username does not exists");

            return new JsonResult(JsonConvert.SerializeObject(!Core.UserChecks.IsFriends(username, secondUsername)));
        }
    }
}