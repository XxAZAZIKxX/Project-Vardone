using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.FriendsControllers
{
    [ApiController, Route("users/[controller]")]
    public class DeleteFriendController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (userId == secondId) return BadRequest("Username equal friend userId");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("Friend does not exist");
                if (!Core.UserChecks.IsFriends(userId, secondId)) return Ok();

                var dataContext = Program.DataContext;
                var friendsList = dataContext.FriendsList;
                friendsList.Include(p => p.FromUser).Include(p => p.ToUser).Load();
                var users = dataContext.Users;
                var user1 = users.First(p => p.Id == userId);
                var user2 = users.First(p => p.Id == secondId);
                try
                {
                    var first = friendsList.First(p =>
                        p.FromUser == user1 && p.ToUser == user2 ||
                        p.FromUser == user2 && p.ToUser == user1);
                    friendsList.Remove(first);
                    dataContext.SaveChanges();
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