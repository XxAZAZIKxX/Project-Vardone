using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class LeaveGuildController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long guildId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.GuildsChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");

                var dataContext = Program.DataContext;
                var members = dataContext.Members;
                members.Include(p => p.User).Load();
                members.Include(p => p.Guild).Load();

                try
                {
                    var first = members.First(p => p.User.Id == userId && p.Guild.Id == guildId);
                    members.Remove(first);
                    dataContext.SaveChanges();
                    return Ok("Leaved");
                }
                catch
                {
                    return Ok("You are not member");
                }
            })).GetAwaiter().GetResult();
        }
    }
}