using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class GetChatMessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long chatId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (chatId <= 0) return BadRequest("ChatId lower 0");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.UserChecks.IsUserExists(chatId)) return BadRequest("Second user does not exists");

                var privateChats = Program.DataContext.PrivateChats;
                privateChats.Include(p => p.From).Load();
                privateChats.Include(p => p.To).Load();
                privateChats.Include(p => p.From.Info).Load();
                privateChats.Include(p => p.To.Info).Load();
                var privateMessages = Program.DataContext.PrivateMessages;
                privateMessages.Include(p => p.From).Load();
                privateMessages.Include(p => p.From.Info).Load();
                privateMessages.Include(p => p.Chat).Load();

                try
                {
                    var messages = new List<PrivateMessage>();
                    var chat = privateChats.First(p => p.Id == chatId);
                    var privateMessagesTables = privateMessages.Where(p => p.Chat == chat);
                    foreach (var message in privateMessagesTables)
                    {
                        var user1 = message.Chat.From.Id == userId ? message.Chat.From : message.Chat.To;
                        var user2 = message.Chat.From.Id != userId ? message.Chat.From : message.Chat.To;
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