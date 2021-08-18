using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Core;
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds.Management
{
    [ApiController, Route("guilds/[controller]")]
    public class CreateGuildController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] string name = null)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

                try
                {
                    var dataContext = Program.DataContext;
                    var guilds = dataContext.Guilds;
                    var users = dataContext.Users;
                    var members = dataContext.GuildMembers;

                    var user = users.First(p => p.Id == userId);

                    var guild = new GuildsTable
                    {
                        CreatedAt = DateTime.Now,
                        Name = name ?? $"{user.Username}'s server",
                        Owner = user
                    };
                    guilds.Add(guild);
                    dataContext.SaveChanges();
                    members.Add(new GuildMembersTable { User = user, Guild = guild });
                    dataContext.SaveChanges();
                    return Ok("Created");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            })).GetAwaiter().GetResult();
        }
    }
}