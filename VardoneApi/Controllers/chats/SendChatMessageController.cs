﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class SendChatMessageController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId, [FromBody] PrivateMessageModel message)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (message == null) return BadRequest("Empty message");
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (userId == secondId) return BadRequest("Username equal second username");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest();
                if (!Core.PrivateChatsChecks.IsCanWriteMessage(userId, secondId)) return BadRequest("You should be friends");
                if (string.IsNullOrWhiteSpace(message.Text) && string.IsNullOrWhiteSpace(message.Base64Image))
                    return BadRequest("Empty message");

                var chats = Program.DataContext.PrivateChats;
                chats.Include(p => p.FromUser).Load();
                chats.Include(p => p.ToUser).Load();
                var messages = Program.DataContext.PrivateMessages;
                messages.Include(p => p.From).Load();
                messages.Include(p => p.Chat).Load();
                var users = Program.DataContext.Users;
                var user1 = users.First(p => p.UserId == userId);
                var user2 = users.First(p => p.UserId == secondId);

                PrivateChatsTable chat;

                if (!Core.PrivateChatsChecks.IsChatExists(userId, secondId))
                {
                    chat = new PrivateChatsTable { FromUser = user1, ToUser = user2 };
                    chats.Add(chat);
                    Program.DataContext.SaveChanges();
                }
                else
                {
                    chat = chats.First(p =>
                        p.FromUser == user1 && p.ToUser == user2 ||
                        p.FromUser == user2 && p.ToUser == user1);
                }

                try
                {
                    var newMessage = new PrivateMessagesTable
                    {
                        Chat = chat,
                        From = user1,
                        Text = message.Text ?? "",
                        Image = message.Base64Image == null ? null : Convert.FromBase64String(message.Base64Image),
                        CreatedTime = DateTime.Now
                    };
                    messages.Add(newMessage);
                    Program.DataContext.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            })).GetAwaiter().GetResult();
        }
    }
}