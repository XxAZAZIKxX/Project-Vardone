using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class JoinGuildController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long guildId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.GuildsChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    var guilds = dataContext.Guilds;
                    var members = dataContext.Members;
                    members.Include(p => p.User).Load();
                    members.Include(p => p.Guild).Load();

                    var guild = guilds.First(p => p.Id == guildId);
                    var user = users.First(p => p.Id == userId);
                    foreach (var table in members.Where(p => p.Guild == guild))
                    {
                        if (table.User.Id == userId) return Ok();
                    }

                    members.Add(new MembersTable { User = user, Guild = guild });
                    dataContext.SaveChanges();
                    return Ok("Joined");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}