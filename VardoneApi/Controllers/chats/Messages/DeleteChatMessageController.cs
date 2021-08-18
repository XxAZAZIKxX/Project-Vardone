using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats.Messages
{
    [ApiController, Route("chats/[controller]")]
    public class DeleteChatMessageController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long messageId)
        {
            return Task.Run(new Func<IActionResult>(() =>
           {
               if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
               if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

               try
               {
                   var dataContext = Program.DataContext;
                   var messages = dataContext.PrivateMessages;
                   messages.Include(p => p.Author).Load();

                   try
                   {
                       var message = messages.First(p => p.Author.Id == userId && p.Id == messageId);
                       messages.Remove(message);
                       dataContext.SaveChanges();
                       return Ok("Deleted");
                   }
                   catch
                   {
                       return BadRequest("You cannot delete this message");
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