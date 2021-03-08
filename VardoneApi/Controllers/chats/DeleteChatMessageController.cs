using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class DeleteChatMessageController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long idMessage)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (idMessage <= 0) return BadRequest("Id message lower 0");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");

            var messages = Program.DataContext.PrivateMessages;
            messages.Include(p => p.From).Load();

            try
            {
                var message = messages.First(p => p.From.Id == userId && p.Id == idMessage);
                messages.Remove(message);
                Program.DataContext.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}