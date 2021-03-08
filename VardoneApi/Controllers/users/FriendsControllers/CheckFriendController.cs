using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.FriendsControllers
{
    [ApiController, Route("users/[controller]")]
    public class CheckFriendController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (userId == secondId) return BadRequest("Username equal second userId");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("Second userId does not exists");

            return new JsonResult(JsonConvert.SerializeObject(!Core.UserChecks.IsFriends(userId, secondId)));
        }
    }
}