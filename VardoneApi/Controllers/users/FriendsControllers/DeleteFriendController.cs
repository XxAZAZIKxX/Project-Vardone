using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.FriendsControllers
{
    [ApiController, Route("users/[controller]")]
    public class DeleteFriendController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token, [FromQuery] string usernameFriend)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("Empty username");
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (string.IsNullOrWhiteSpace(usernameFriend)) return BadRequest("Empty friend username");
            if (username == usernameFriend) return BadRequest("Username equal friend username");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { Username = username, Token = token }))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.IsUserExists(usernameFriend)) return BadRequest("Friend does not exist");

            var friendsList = Program.DataContext.FriendsList;
            friendsList.Include(p => p.From).Include(p => p.To).Load();

            try
            {
                var first = friendsList.First(p =>
                    p.From.Username == username && p.To.Username == usernameFriend ||
                    p.From.Username == usernameFriend && p.To.Username == username);
                friendsList.Remove(first);
                Program.DataContext.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}