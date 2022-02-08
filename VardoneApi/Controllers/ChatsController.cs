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
using VardoneEntities.Models.GeneralModels.Users;
using VardoneEntities.Models.TcpModels;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class ChatsController : ControllerBase
    {
        [HttpPost, Route("getPrivateChat")]
        public async Task<IActionResult> GetPrivateChat([FromQuery] long chatId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!PrivateChatChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!PrivateChatChecks.IsCanManageChat(token.UserId, chatId)) return BadRequest("You are not allowed to get this");

                try
                {
                    var dataContext = Program.DataContext;
                    var privateChats = dataContext.PrivateChats;
                    privateChats.Include(p => p.FromUser).Load();
                    privateChats.Include(p => p.ToUser).Load();
                    var messages = dataContext.PrivateMessages;
                    messages.Include(p => p.Chat).Load();
                    messages.Include(p => p.Author).Load();

                    var chat = privateChats.FirstOrDefault(p => p.Id == chatId);
                    if (chat is null) return BadRequest("Chat is not exists");

                    var chatReturn = PrivateChatCreateHelper.GetPrivateChat(chatId, token.UserId);
                    return Ok(chatReturn);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getPrivateChatWithUser")]
        public async Task<IActionResult> GetPrivateChatWithUser([FromQuery] long secondId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!UserChecks.IsUserExists(secondId)) return BadRequest("Second user does not exists");

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                var privateChats = dataContext.PrivateChats;
                privateChats.Include(p => p.FromUser).Load();
                privateChats.Include(p => p.ToUser).Load();
                privateChats.Include(p => p.FromUser.Info).Load();
                privateChats.Include(p => p.ToUser.Info).Load();

                try
                {
                    var chat = privateChats.First(p => p.FromUser.Id == userId && p.ToUser.Id == secondId || p.FromUser.Id == secondId && p.ToUser.Id == userId);
                    var privateChat = PrivateChatCreateHelper.GetPrivateChat(chat.Id, userId);
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
                        var chat = PrivateChatCreateHelper.GetPrivateChat(newChat.Id, userId);
                        Task.Run(() =>
                        {
                            var tcpNotify = new TcpResponseModel
                            {
                                type = TypeTcpResponse.NewPrivateChat,
                                data = chat
                            };
                            Program.TcpServer.SendMessageTo(chat.FromUser.UserId, tcpNotify);
                            Program.TcpServer.SendMessageTo(chat.ToUser.UserId, tcpNotify);
                        });
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
        public async Task<IActionResult> GetPrivateChatMessages([FromQuery] long chatId, [FromQuery] int limit = 0, [FromQuery] long startFrom = 0)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!PrivateChatChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!PrivateChatChecks.IsCanManageChat(userId, chatId)) return BadRequest("No access");

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
                    var chat = privateChats.FirstOrDefault(p => p.Id == chatId);
                    if (chat is null) return Ok(messages.ToArray());

                    if (chat.FromUser.Id == userId) chat.FromLastReadTimeMessages = DateTime.Now;
                    else chat.ToLastReadTimeMessages = DateTime.Now;
                    privateChats.Update(chat);
                    dataContext.SaveChanges();

                    var selectedMessages = privateMessages.Where(p => p.Chat == chat);
                    if (startFrom > 0) selectedMessages = selectedMessages.Where(p => p.Id < startFrom);
                    if (limit > 0) selectedMessages = selectedMessages.OrderByDescending(p => p.Id).Take(limit);
                    foreach (var message in selectedMessages)
                    {
                        messages.Add(MessageCreateHelper.GetPrivateMessage(message.Id, userId));
                    }
                    return Ok(messages.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getPrivateChatMessage")]
        public async Task<IActionResult> GetPrivateMessage([FromQuery] long messageId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Token invalid");
                }

                try
                {
                    var dataContext = Program.DataContext;
                    var privateMessages = dataContext.PrivateMessages;
                    privateMessages.Include(p => p.Author).Load();
                    privateMessages.Include(p => p.Author.Info).Load();
                    privateMessages.Include(p => p.Chat).Load();
                    privateMessages.Include(p => p.Chat.FromUser).Load();
                    privateMessages.Include(p => p.Chat.ToUser).Load();

                    var message = privateMessages.FirstOrDefault(p => p.Id == messageId);
                    if (message is null) return BadRequest("Message is not exists");

                    if (!PrivateChatChecks.IsCanManageChat(token.UserId, message.Chat.Id))
                        return BadRequest("You are not allowed to do this");

                    var returnMessage = MessageCreateHelper.GetPrivateMessage(message.Id, token.UserId);
                    return Ok(returnMessage);
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
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }
                if (message is null) return BadRequest("Empty message");
                if (message.Text.Length > 255) return BadRequest("Text length is upper then 255 symbols");

                if (!UserChecks.IsUserExists(secondId)) return BadRequest();
                if (!PrivateChatChecks.IsCanWriteMessage(userId, secondId)) return BadRequest("You should be friends");
                if (string.IsNullOrWhiteSpace(message.Text) &&
                    (string.IsNullOrWhiteSpace(message.Base64Image) ||
                     !ImageWorker.IsImage(Convert.FromBase64String(message.Base64Image))))
                    return BadRequest("Empty message");

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
                        Image = message.Base64Image is null
                            ? null
                            : ImageWorker.CompressImageQualityLevel(Convert.FromBase64String(message.Base64Image), 70),
                        CreatedTime = DateTime.Now
                    };
                    messages.Add(newMessage);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewPrivateMessage,
                            data = MessageCreateHelper.GetPrivateMessage(newMessage.Id, userId)
                        };
                        Program.TcpServer.SendMessageTo(userId, tcpNotify);
                        Program.TcpServer.SendMessageTo(secondId, tcpNotify);
                    });
                    return Ok("Sended");
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    var first = privateChats.FirstOrDefault(p => p.Id == chatId);
                    if (first is null) return BadRequest("Chat is not exists");
                    var chat = PrivateChatCreateHelper.GetPrivateChat(first.Id, userId);
                    var tcpNotify = new TcpResponseModel
                    {
                        type = TypeTcpResponse.DeletePrivateChat,
                        data = chat
                    };
                    privateChats.Remove(first);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(chat.FromUser.UserId, tcpNotify);
                        Program.TcpServer.SendMessageTo(chat.ToUser.UserId, tcpNotify);
                    });
                    return Ok("Deleted");
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
                var token = JwtTokenWorker.GetUserToken(User);
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

                    var message = messages.First(p => p.Id == messageId);
                    if (message.Author.Id != userId) return BadRequest("You cannot delete this message");
                    var data = MessageCreateHelper.GetPrivateMessage(message.Id, userId, true);
                    var tcpNotify = new TcpResponseModel
                    {
                        type = TypeTcpResponse.DeletePrivateMessage,
                        data = data
                    };
                    messages.Remove(message);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(data.Chat.FromUser.UserId, tcpNotify);
                        Program.TcpServer.SendMessageTo(data.Chat.ToUser.UserId, tcpNotify);
                    });
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