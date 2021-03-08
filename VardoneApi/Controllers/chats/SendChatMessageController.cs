using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneApi.Models.PrivateChats;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class SendChatMessageController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId, [FromBody] PrivateMessage message)
        {
            if (message == null) return BadRequest("Empty message");
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (userId == secondId) return BadRequest("Username equal second username");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest();
            if (!Core.PrivateChatsChecks.IsCanWriteMessage(userId, secondId)) return BadRequest("You should be friends");
            if (string.IsNullOrWhiteSpace(message.Text) && string.IsNullOrWhiteSpace(message.Base64Image))
                return BadRequest("Empty message");

            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.From).Load();
            chats.Include(p => p.To).Load();
            var messages = Program.DataContext.PrivateMessages;
            messages.Include(p => p.From).Load();
            messages.Include(p => p.Chat).Load();
            var users = Program.DataContext.Users;
            var user1 = users.First(p => p.Id == userId);
            var user2 = users.First(p => p.Id == secondId);

            PrivateChatsTable chat;

            if (!Core.PrivateChatsChecks.IsChatExists(userId, secondId))
            {
                chat = new PrivateChatsTable { From = user1, To = user2 };
                chats.Add(chat);
                Program.DataContext.SaveChanges();
            }
            else
            {
                chat = chats.First(p =>
                    p.From == user1 && p.To == user2 ||
                    p.From == user2 && p.To == user1);
            }

            try
            {
                var newMessage = new PrivateMessagesTable
                {
                    Chat = chat,
                    From = user1,
                    Text = message.Text ?? "",
                    Image = message.Base64Image == null ? null : Convert.FromBase64String(message.Base64Image)
                };
                messages.Add(newMessage);
                Program.DataContext.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}