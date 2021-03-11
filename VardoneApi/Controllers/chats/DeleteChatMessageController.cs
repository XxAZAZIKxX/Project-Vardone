using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats
{
    [ApiController, Route("chats/[controller]")]
    public class DeleteChatMessageController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long idMessage)
        {
            return Task.Run( new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (idMessage <= 0) return BadRequest("Id message lower 0");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
            })).GetAwaiter().GetResult();
        }
    }
}