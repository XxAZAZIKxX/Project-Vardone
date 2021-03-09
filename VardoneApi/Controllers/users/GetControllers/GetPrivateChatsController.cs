using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetPrivateChatsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");

            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.From).Load();
            chats.Include(p => p.To).Load();

            return new JsonResult(JsonConvert.SerializeObject(chats.Where(p => p.From.Id == userId || p.To.Id == userId)));
        }
    }
}