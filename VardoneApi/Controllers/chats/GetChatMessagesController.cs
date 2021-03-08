using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneApi.Entity.Models;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class GetChatMessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long id)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (id <=0) return BadRequest("Id lower 0");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.IsUserExists(id)) return BadRequest("Second username does not exists");

            var privateChats = Program.DataContext.PrivateChats;
            privateChats.Include(p => p.From).Load();
            privateChats.Include(p => p.To).Load();
            var privateMessages = Program.DataContext.PrivateMessages;
            privateMessages.Include(p => p.From).Load();
            privateMessages.Include(p => p.Chat).Load();

            var users = Program.DataContext.Users;
            var user1 = users.First(p => p.Id == userId);
            var user2 = users.First(p => p.Id == userId);

            try
            {
                var chat = privateChats.First(p =>
                    p.From == user1 && p.To == user2 ||
                    p.From == user2 && p.To == user1);
                return new JsonResult(JsonConvert.SerializeObject(privateMessages.Where(p => p.Chat == chat)));
            }
            catch
            {
                // ignored
            }

            try
            {
                privateChats.Add(new PrivateChatsTable { From = user1, To = user2 });
                Program.DataContext.SaveChanges();
                return new JsonResult(JsonConvert.SerializeObject(new PrivateMessagesTable[0]));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}