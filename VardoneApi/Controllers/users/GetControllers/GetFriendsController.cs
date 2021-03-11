using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetFriendsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");

                var dataContext = Program.DataContext;
                var friendsList = dataContext.FriendsList;
                friendsList.Include(p => p.FromUser).Load();
                friendsList.Include(p => p.ToUser).Load();
                friendsList.Include(p => p.FromUser.Info).Load();
                friendsList.Include(p => p.ToUser.Info).Load();

                var users = new List<User>();

                foreach (var row in friendsList.Where(p => (p.FromUser.Id == userId || p.ToUser.Id == userId) && p.Confirmed))
                {
                    var friend = row.FromUser.Id != userId ? row.FromUser : row.ToUser;

                    users.Add(new User
                    {
                        UserId = friend.Id,
                        Username = friend.Username,
                        Base64Avatar = friend.Info?.Avatar == null ? null : Convert.ToBase64String(friend.Info.Avatar),
                        Description = friend.Info?.Description
                    });
                }

                return new JsonResult(JsonConvert.SerializeObject(users));
            })).GetAwaiter().GetResult();
        }
    }
}