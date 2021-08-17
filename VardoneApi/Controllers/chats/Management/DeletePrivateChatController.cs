using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.chats.Management
{
    [ApiController, Route("chats/[controller]")]
    public class DeletePrivateChatController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long chatId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.PrivateChatChecks.IsChatExists(chatId)) return BadRequest("Chat is not exists");
                if (!Core.PrivateChatChecks.IsCanManageChat(userId, chatId)) return BadRequest("No access");

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
            })).GetAwaiter().GetResult();
        }
    }
}