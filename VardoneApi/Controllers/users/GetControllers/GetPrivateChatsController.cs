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
    public class GetPrivateChatsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");

                var chatsTable = Program.DataContext.PrivateChats;
                chatsTable.Include(p => p.From).Load();
                chatsTable.Include(p => p.To).Load();
                chatsTable.Include(p => p.From.Info).Load();
                chatsTable.Include(p => p.To.Info).Load();

                var chats = new List<PrivateChat>();

                try
                {
                    foreach (var row in chatsTable.Where(p => p.From.Id == userId || p.To.Id == userId))
                    {
                        var user = row.From.Id == userId ? row.From : row.To;
                        var anotherUser = row.From.Id != userId ? row.From : row.To;
                        chats.Add(new PrivateChat
                        {
                            ChatId = row.Id,
                            FromUser = new User
                            {
                                UserId = user.Id,
                                Username = user.Username,
                                Base64Avatar = user.Info?.Avatar == null ? null : Convert.ToBase64String(user.Info.Avatar),
                                Description = user.Info?.Description
                            },
                            ToUser = new User
                            {
                                UserId = anotherUser.Id,
                                Username = anotherUser.Username,
                                Base64Avatar = anotherUser.Info?.Avatar == null ? null : Convert.ToBase64String(anotherUser.Info.Avatar),
                                Description = anotherUser.Info?.Description
                            }
                        });
                    }
                }
                catch
                {
                    // ignored
                }

                return new JsonResult(JsonConvert.SerializeObject(chats));
            })).GetAwaiter().GetResult();
        }
    }
}