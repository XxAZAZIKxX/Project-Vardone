using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneApi.Entity;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class GetChatMessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long chatId, [FromQuery] int limit)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (chatId <= 0) return BadRequest("Id lower 0");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.PrivateChatsChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!Core.PrivateChatsChecks.IsCanReadMessages(userId, chatId)) return BadRequest("No access");
                var dataContext = Program.DataContext;
                var privateChats = dataContext.PrivateChats;
                privateChats.Include(p => p.FromUser).Load();
                privateChats.Include(p => p.ToUser).Load();
                privateChats.Include(p => p.ToUser).Load();
                privateChats.Include(p => p.FromUser.Info).Load();
                privateChats.Include(p => p.ToUser.Info).Load();
                var privateMessages = dataContext.PrivateMessages;
                privateMessages.Include(p => p.From).Load();
                privateMessages.Include(p => p.From.Info).Load();
                privateMessages.Include(p => p.Chat).Load();

                try
                {
                    var messages = new List<PrivateMessage>();
                    var chat = privateChats.First(p => p.Id == chatId);
                    var privateMessagesTables = privateMessages.Where(p => p.Chat == chat).Take(limit <= 0 ? privateMessages.Count() : limit);
                    if (limit == 1) privateMessagesTables = privateMessagesTables.OrderByDescending(p => p.Id).Take(1);
                    foreach (var message in privateMessagesTables)
                    {
                        var user1 = message.Chat.FromUser.Id == userId ? message.Chat.FromUser : message.Chat.ToUser;
                        var user2 = message.Chat.FromUser.Id != userId ? message.Chat.FromUser : message.Chat.ToUser;
                        messages.Add(new PrivateMessage
                        {
                            MessageId = message.Id,
                            Chat = new PrivateChat
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
                                }
                            },
                            Author = new User
                            {
                                UserId = message.From.Id,
                                Username = message.From.Username,
                                Base64Avatar = message.From.Info?.Avatar == null ? null : Convert.ToBase64String(message.From.Info.Avatar),
                                Description = message.From.Info?.Description
                            },
                            Text = message.Text,
                            Base64Image = message.Image == null ? null : Convert.ToBase64String(message.Image),
                            CreateTime = message.CreatedTime
                        });
                    }

                    return new JsonResult(JsonConvert.SerializeObject(messages));
                }
                catch
                {
                    // ignored
                }

                return new JsonResult(JsonConvert.SerializeObject(new List<PrivateMessage>(0)));
            })).GetAwaiter().GetResult();
        }
    }
}