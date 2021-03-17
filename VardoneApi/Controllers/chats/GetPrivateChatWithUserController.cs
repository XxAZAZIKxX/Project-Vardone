using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneApi.Entity.Models;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class GetPrivateChatWithUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("Second user does not exists");

                var dataContext = Program.DataContext;
                var privateChats = dataContext.PrivateChats;
                var users = dataContext.Users;
                var privateMessages = dataContext.PrivateMessages;
                privateChats.Include(p => p.FromUser).Load();
                privateChats.Include(p => p.ToUser).Load();
                privateChats.Include(p => p.FromUser.Info).Load();
                privateChats.Include(p => p.ToUser.Info).Load();

                try
                {
                    var chat = privateChats.First(p => p.FromUser.Id == userId && p.ToUser.Id == secondId || p.FromUser.Id == secondId && p.ToUser.Id == userId);
                    var user1 = chat.FromUser.Id == userId ? chat.FromUser : chat.ToUser;
                    var user2 = chat.FromUser.Id != userId ? chat.FromUser : chat.ToUser;
                    var privateChat = new PrivateChat
                    {
                        ChatId = chat.Id,
                        FromUser = new User
                        {
                            UserId = user1.Id,
                            Username = user1.Username,
                            Base64Avatar = user1.Info?.Avatar == null ? null : Convert.ToBase64String(user1.Info.Avatar),
                            Description = user1.Info?.Description
                        },
                        ToUser = new User
                        {
                            UserId = user2.Id,
                            Username = user2.Username,
                            Base64Avatar = user2.Info?.Avatar == null ? null : Convert.ToBase64String(user2.Info.Avatar),
                            Description = user2.Info?.Description
                        },
                        UnreadMessages = privateMessages.Count(p => p.Chat.Id == chat.Id && p.Author != user1 && DateTime.Compare(p.CreatedTime, DateTime.Now) < 0)
                    };
                    return new JsonResult(JsonConvert.SerializeObject(privateChat));
                }
                catch
                {
                    try
                    {
                        var newChat = new PrivateChatsTable
                        {
                            FromUser = users.First(p => p.Id == userId),
                            ToUser = users.First(p => p.Id == secondId),
                            FromLastReadTimeMessages = DateTime.Now,
                            ToLastReadTimeMessages = DateTime.Now
                        };
                        privateChats.Add(newChat);
                        dataContext.SaveChanges();
                        var chat = new PrivateChat
                        {
                            ChatId = newChat.Id,
                            FromUser = new User
                            {
                                UserId = newChat.FromUser.Id,
                                Username = newChat.FromUser.Username,
                                Base64Avatar = newChat.FromUser.Info?.Avatar == null ? null : Convert.ToBase64String(newChat.FromUser.Info.Avatar),
                                Description = newChat.FromUser.Info?.Description
                            },
                            ToUser = new User
                            {
                                UserId = newChat.ToUser.Id,
                                Username = newChat.ToUser.Username,
                                Base64Avatar = newChat.ToUser.Info?.Avatar == null ? null : Convert.ToBase64String(newChat.ToUser.Info.Avatar),
                                Description = newChat.ToUser.Info?.Description
                            }
                        };
                        return new JsonResult(JsonConvert.SerializeObject(chat));
                    }
                    catch
                    {
                        return BadRequest();
                    }
                }
            })).GetAwaiter().GetResult();
        }
    }
}