using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core;
using VardoneApi.Core.Checks;
using VardoneApi.Core.CreateHelpers;
using VardoneApi.Entity.Models.PrivateChats;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.User;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class ChatsController : ControllerBase
    {
        [HttpPost, Route("getPrivateChatWithUser")]
        public async Task<IActionResult> GetPrivateChatWithUser([FromQuery] long secondId, [FromHeader] bool onlyId = false)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!UserChecks.IsUserExists(secondId)) return BadRequest("Second user does not exists");

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
                    var lastReadTime = user1 == chat.FromUser ? chat.FromLastReadTimeMessages : chat.ToLastReadTimeMessages;
                    

                    var privateChat = new PrivateChat
                    {
                        ChatId = chat.Id,
                        FromUser = UserCreateHelper.GetUser(user1, onlyId),
                        ToUser = UserCreateHelper.GetUser(user2, onlyId),
                        UnreadMessages = privateMessages.Count(p => p.Chat.Id == chat.Id && p.Author != user1 && DateTime.Compare(p.CreatedTime, lastReadTime) > 0)
                    };
                    return Ok(privateChat);
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
                            FromUser = UserCreateHelper.GetUser(newChat.FromUser, onlyId),
                            ToUser = UserCreateHelper.GetUser(newChat.ToUser, onlyId)
                        };
                        return Ok(chat);
                    }
                    catch (Exception e)
                    {
                        return Problem(e.Message);
                    }
                }
            }));
        }
        //
        [HttpPost, Route("getPrivateChatMessages")]
        public async Task<IActionResult> GetPrivateChatMessages([FromQuery] long chatId, [FromQuery] int limit = 0, [FromQuery] long startFrom = 0, [FromHeader] bool onlyId = false)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!PrivateChatChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!PrivateChatChecks.IsCanManageChat(userId, chatId)) return BadRequest("No access");

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

                        if (!onlyId)
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
                            
                            var item = new PrivateMessage
                            {
                                MessageId = message.Id,
                                Chat = new PrivateChat
                                {
                                    ChatId = chat.Id,
                                    FromUser = UserCreateHelper.GetUser(user1, onlyId),
                                    ToUser = UserCreateHelper.GetUser(user2, onlyId)
                                },
                                Author = UserCreateHelper.GetUser(message.Author, onlyId),
                                CreatedTime = message.CreatedTime
                            };
                            if (!onlyId)
                            {
                                item.Text = message.Text;
                                item.Base64Image = message.Image == null ? null : Convert.ToBase64String(message.Image);
                            }

                            messages.Add(item);
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
            }));
        }
        //
        [HttpPost, Route("getLastDeleteMessageTime")]
        public async Task<IActionResult> GetLastDeleteMessageTime([FromQuery] long chatId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!PrivateChatChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!PrivateChatChecks.IsCanManageChat(userId, chatId)) return BadRequest("No access");

                try
                {
                    var dataContext = Program.DataContext;
                    var privateChats = dataContext.PrivateChats;
                    var chat = privateChats.First(p => p.Id == chatId);
                    return chat.LastDeleteMessageTime is not null ? Ok(chat.LastDeleteMessageTime) : Ok();
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("sendPrivateChatMessage")]
        public async Task<IActionResult> SendPrivateChatMessage([FromQuery] long secondId, [FromBody] MessageModel message)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (message == null) return BadRequest("Empty message");
                if (message.Text.Length > 255) return BadRequest("<#!> Сообщение больше чем 255 символов!");

                if (!UserChecks.IsUserExists(secondId)) return BadRequest();
                if (!PrivateChatChecks.IsCanWriteMessage(userId, secondId)) return BadRequest("You should be friends");
                if (string.IsNullOrWhiteSpace(message.Text) && string.IsNullOrWhiteSpace(message.Base64Image)) return BadRequest("Empty message");

                try
                {
                    var dataContext = Program.DataContext;
                    var chats = dataContext.PrivateChats;
                    chats.Include(p => p.FromUser).Load();
                    chats.Include(p => p.ToUser).Load();
                    var messages = dataContext.PrivateMessages;
                    messages.Include(p => p.Author).Load();
                    messages.Include(p => p.Chat).Load();
                    var users = dataContext.Users;
                    var user1 = users.First(p => p.Id == userId);
                    var user2 = users.First(p => p.Id == secondId);

                    PrivateChatsTable chat;

                    if (!PrivateChatChecks.IsChatExists(userId, secondId))
                    {
                        chat = new PrivateChatsTable { FromUser = user1, ToUser = user2 };
                        chats.Add(chat);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        chat = chats.First(p =>
                            p.FromUser == user1 && p.ToUser == user2 ||
                            p.FromUser == user2 && p.ToUser == user1);
                    }
                    var newMessage = new PrivateMessagesTable
                    {
                        Chat = chat,
                        Author = user1,
                        Text = message.Text ?? "",
                        Image = message.Base64Image == null ? null : Convert.FromBase64String(message.Base64Image),
                        CreatedTime = DateTime.Now
                    };
                    messages.Add(newMessage);
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("deletePrivateChat")]
        public async Task<IActionResult> DeletePrivateChat([FromQuery] long chatId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!PrivateChatChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!PrivateChatChecks.IsCanManageChat(userId, chatId)) return BadRequest("No access");

                try
                {
                    var dataContext = Program.DataContext;
                    var privateChats = dataContext.PrivateChats;
                    privateChats.Remove(privateChats.First(p => p.Id == chatId));
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }));
        }
        //
        [HttpPost, Route("deletePrivateChatMessage")]
        public async Task<IActionResult> DeletePrivateChatMessage([FromQuery] long messageId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                try
                {
                    var dataContext = Program.DataContext;
                    var messages = dataContext.PrivateMessages;
                    messages.Include(p => p.Author).Load();
                    messages.Include(p => p.Chat).Load();
                    var chats = dataContext.PrivateChats;

                    var message = messages.First(p => p.Id == messageId);
                    var chat = chats.First(p => p.Id == message.Chat.Id);
                    if (message.Author.Id != userId) return BadRequest("You cannot delete this message");
                    chat.LastDeleteMessageTime = DateTime.Now;
                    messages.Remove(message);
                    dataContext.SaveChanges();
                    return Ok("Deleted");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
    }
}