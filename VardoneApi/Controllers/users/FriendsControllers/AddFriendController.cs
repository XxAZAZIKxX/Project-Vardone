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
                friendsList.Include(p => p.FromUser).Load();
                friendsList.Include(p => p.ToUser).Load();
                var users = Program.DataContext.Users;
                var user1 = users.First(p => p.UserId == userId);
                var user2 = users.First(p => p.UserId == secondId);
                try
                {
                    var list = friendsList.First(p =>
                        (p.FromUser == user1 && p.ToUser == user2 ||
                        p.FromUser == user2 && p.ToUser == user1)
                        );
                    if (list.CreatedByUser == user1) return Ok();
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
                    FromUser = user1,
                    ToUser = user2,
                    CreatedByUser = user1,
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