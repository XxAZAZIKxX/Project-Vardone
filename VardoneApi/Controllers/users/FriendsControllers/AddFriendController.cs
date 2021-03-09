using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.FriendsControllers
{
    [ApiController, Route("users/[controller]")]
    public class AddFriendController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {

                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Token empty");
                if (userId == secondId) return BadRequest("Username equal friend userId");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("Friend does not exist");
                if (Core.UserChecks.IsFriends(userId, secondId)) return Ok();

                var friendsList = Program.DataContext.FriendsList;
                friendsList.Include(p => p.From).Include(p => p.To).Load();
                var users = Program.DataContext.Users;
                var user1 = users.First(p => p.Id == userId);
                var user2 = users.First(p => p.Id == secondId);
                try
                {
                    var list = friendsList.First(p =>
                        p.From == user1 && p.To == user2 ||
                        p.From == user2 && p.To == user1);
                    list.Confirmed = true;
                    Program.DataContext.SaveChanges();
                    return Ok();
                }
                catch
                {
                    // ignored
                }

                var newFl = new FriendsListTable
                {
                    From = user1,
                    To = user2,
                    Confirmed = false
                };

                try
                {
                    friendsList.Add(newFl);
                    Program.DataContext.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            })).GetAwaiter().GetResult();
        }
    }
}