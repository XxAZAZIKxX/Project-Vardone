using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats.Messages
{
    [ApiController, Route("chats/[controller]")]
    public class GetPrivateChatMessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long chatId, [FromQuery] bool read = true, [FromQuery] int limit = 0, [FromQuery] long startFrom = 0)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.PrivateChatChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!Core.PrivateChatChecks.IsCanReadMessages(userId, chatId)) return BadRequest("No access");

                try
                {
                    var dataContext = Program.DataContext;
                    var privateChats = dataContext.PrivateChats;
                    privateChats.Include(p => p.FromUser).Load();
                    privateChats.Include(p => p.ToUser).Load();
                    privateChats.Include(p => p.ToUser).Load();
                    privateChats.Include(p => p.FromUser.Info).Load();
                    privateChats.Include(p => p.ToUser.Info).Load();
                    var privateMessages = dataContext.PrivateMessages;
                    privateMessages.Include(p => p.Author).Load();
                    privateMessages.Include(p => p.Author.Info).Load();
                    privateMessages.Include(p => p.Chat).Load();

                    try
                    {
                        var messages = new List<PrivateMessage>();
                        var chat = privateChats.First(p => p.Id == chatId);

                        if (read)
                        {
                            if (chat.FromUser.Id == userId) chat.FromLastReadTimeMessages = DateTime.Now;
                            else chat.ToLastReadTimeMessages = DateTime.Now;
                            privateChats.Update(chat);
                            dataContext.SaveChanges();
                        }

                        var selectedMessages = privateMessages.Where(p => p.Chat == chat);
                        if (startFrom > 0) selectedMessages = selectedMessages.Where(p => p.Id < startFrom);
                        if (limit > 0) selectedMessages = selectedMessages.OrderByDescending(p => p.Id).Take(limit);
                        foreach (var message in selectedMessages)
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
                                    UserId = message.Author.Id,
                                    Username = message.Author.Username,
                                    Base64Avatar = message.Author.Info?.Avatar == null ? null : Convert.ToBase64String(message.Author.Info.Avatar),
                                    Description = message.Author.Info?.Description
                                },
                                Text = message.Text,
                                Base64Image = message.Image == null ? null : Convert.ToBase64String(message.Image),
                                CreateTime = message.CreatedTime
                            });
                        }

                        return Ok(messages);
                    }
                    catch
                    {
                        return Ok(new List<PrivateMessage>(0));
                    }
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}