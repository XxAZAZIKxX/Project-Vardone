using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.FriendsControllers
{
    [ApiController, Route("users/[controller]")]
    public class AddFriendController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token, [FromQuery] string usernameFriend)
        {
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized("Username empty");
            if (string.IsNullOrWhiteSpace(token)) return Unauthorized("Token empty");
            if (string.IsNullOrWhiteSpace(usernameFriend)) return BadRequest("Friend username empty");
            if (username == usernameFriend) return BadRequest("Username equal friend username");
            if (!Core.UserChecks.CheckToken(new TokenUserModel {Username = username, Token = token}))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.UserExists(usernameFriend)) return BadRequest("Friend does not exist");

            var friendsList = Program.DataContext.FriendsList;
            friendsList.Include(p=>p.From).Include(p=>p.To).Load();
            var users = Program.DataContext.Users;

            try
            {
                var list = friendsList.First(p =>
                    p.From.Username == username && p.To.Username == usernameFriend ||
                    p.From.Username == usernameFriend && p.To.Username == username);
                list.Confirmed = true;
                Program.DataContext.SaveChanges();
                return Ok();
            }
            catch
            {
                // ignored
            }

            var newFl = new FriendsList
            {
                From = users.First(p => p.Username == username), To = users.First(p => p.Username == usernameFriend),
                Confirmed = false
            };

            try
            {
                friendsList.Add(newFl);
                Program.DataContext.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}