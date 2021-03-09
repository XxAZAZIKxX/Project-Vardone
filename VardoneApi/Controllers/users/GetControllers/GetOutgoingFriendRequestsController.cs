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
    public class GetOutgoingFriendRequestsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

                var friendsList = Program.DataContext.FriendsList;
                friendsList.Include(p => p.From).Load();
                friendsList.Include(p => p.To).Load();
                friendsList.Include(p => p.To.Info).Load();
                var users = new List<User>();
                try
                {
                    foreach (var row in friendsList.Where(p => p.From.Id == userId && p.Confirmed == false))
                    {
                        users.Add(new User
                        {
                            UserId = row.To.Id,
                            Username = row.To.Username,
                            Base64Avatar = row.To.Info?.Avatar == null ? null : Convert.ToBase64String(row.To.Info.Avatar),
                            Description = row.To.Info?.Description
                        });
                    }
                }
                catch
                {
                    // ignored
                }

                return new JsonResult(JsonConvert.SerializeObject(users));
            })).GetAwaiter().GetResult();
        }
    }
}