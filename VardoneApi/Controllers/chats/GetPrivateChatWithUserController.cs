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

                var privateChats = Program.DataContext.PrivateChats;
                privateChats.Include(p => p.From).Load();
                privateChats.Include(p => p.To).Load();
                privateChats.Include(p => p.From.Info).Load();
                privateChats.Include(p => p.To.Info).Load();
                var users = Program.DataContext.Users;

                try
                {
                    var first = privateChats.First(p =>
                        p.From.Id == userId && p.To.Id == secondId || p.From.Id == secondId && p.To.Id == userId);
                    var user1 = first.From.Id == userId ? first.From : first.To;
                    var user2 = first.From.Id != userId ? first.From : first.To;
                    var chat = new PrivateChat
                    {
                        ChatId = first.Id,
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
                    };
                    return new JsonResult(JsonConvert.SerializeObject(chat));
                }
                catch
                {
                    try
                    {
                        var newChat = new PrivateChatsTable
                        { From = users.First(p => p.Id == userId), To = users.First(p => p.Id == secondId) };
                        privateChats.Add(newChat);
                        Program.DataContext.SaveChanges();
                        var chat = new PrivateChat
                        {
                            ChatId = newChat.From.Id,
                            FromUser = new User
                            {
                                UserId = newChat.From.Id,
                                Username = newChat.From.Username,
                                Base64Avatar = newChat.From.Info?.Avatar == null ? null : Convert.ToBase64String(newChat.From.Info.Avatar),
                                Description = newChat.From.Info?.Description
                            },
                            ToUser = new User
                            {
                                UserId = newChat.To.Id,
                                Username = newChat.To.Username,
                                Base64Avatar = newChat.To.Info?.Avatar == null ? null : Convert.ToBase64String(newChat.To.Info.Avatar),
                                Description = newChat.To.Info?.Description
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