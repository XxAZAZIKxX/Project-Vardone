using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers.Friends
{
    [ApiController, Route("users/[controller]")]
    public class GetIncomingFriendRequestsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

                try
                {
                    var dataContext = Program.DataContext;
                    var friendsList = dataContext.FriendsList;
                    friendsList.Include(p => p.FromUser).Load();
                    friendsList.Include(p => p.ToUser).Load();
                    friendsList.Include(p => p.FromUser.Info).Load();
                    var users = new List<User>();
                    try
                    {
                        foreach (var row in friendsList.Where(p => p.ToUser.Id == userId && p.Confirmed == false))
                        {
                            users.Add(new User
                            {
                                UserId = row.FromUser.Id,
                                Username = row.FromUser.Username,
                                Base64Avatar = row.FromUser.Info?.Avatar == null
                                    ? null
                                    : Convert.ToBase64String(row.FromUser.Info.Avatar),
                                Description = row.FromUser.Info?.Description
                            });
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    return Ok(users);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}