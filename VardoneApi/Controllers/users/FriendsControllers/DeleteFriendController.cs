using System;
using System.Linq;
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
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (userId == secondId) return BadRequest("Username equal friend userId");
            if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("Friend does not exist");

            var friendsList = Program.DataContext.FriendsList;
            friendsList.Include(p => p.From).Include(p => p.To).Load();
            var users = Program.DataContext.Users;
            var user1 = users.First(p => p.Id == userId);
            var user2 = users.First(p => p.Id == secondId);
            try
            {
                var first = friendsList.First(p =>
                    p.From == user1 && p.To == user2 ||
                    p.From == user2 && p.To == user1);
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