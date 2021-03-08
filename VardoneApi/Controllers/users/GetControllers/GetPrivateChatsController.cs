using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetPrivateChatsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("Empty username");
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { Username = username, Token = token }))
                return Unauthorized("Invalid token");

            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.From).Load();

            return new JsonResult(JsonConvert.SerializeObject(chats.Where(p => p.From.Username == username || p.To.Username == username)));
        }
    }
}