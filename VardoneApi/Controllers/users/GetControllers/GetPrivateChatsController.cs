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

                var dataContext = Program.DataContext;
                var chatsTable = dataContext.PrivateChats;
                var privateMessages = dataContext.PrivateMessages;
                privateMessages.Include(p => p.Author).Load();
                chatsTable.Include(p => p.FromUser).Load();
                chatsTable.Include(p => p.ToUser).Load();
                chatsTable.Include(p => p.FromUser.Info).Load();
                chatsTable.Include(p => p.ToUser.Info).Load();

                var chats = new List<PrivateChat>();

                try
                {
                    var @where = chatsTable.Where(p => p.FromUser.Id == userId || p.ToUser.Id == userId).ToList();
                    foreach (var chat in @where)
                    {
                        var user1 = chat.FromUser.Id == userId ? chat.FromUser : chat.ToUser;
                        var user2 = chat.FromUser.Id != userId ? chat.FromUser : chat.ToUser;
                        var item = new PrivateChat
                        {
                            ChatId = chat.Id,
                            FromUser = new User
                            {
                                UserId = user1.Id,
                                Username = user1.Username,
                                Base64Avatar = user1.Info?.Avatar == null
                                    ? null
                                    : Convert.ToBase64String(user1.Info.Avatar),
                                Description = user1.Info?.Description
                            },
                            ToUser = new User
                            {
                                UserId = user2.Id,
                                Username = user2.Username,
                                Base64Avatar = user2.Info?.Avatar == null
                                    ? null
                                    : Convert.ToBase64String(user2.Info.Avatar),
                                Description = user2.Info?.Description
                            },
                            UnreadMessages = privateMessages.Count(p => p.Chat.Id == chat.Id && p.Author != user1 && DateTime.Compare(p.CreatedTime, DateTime.Now) < 0)
                        };
                        chats.Add(item);
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