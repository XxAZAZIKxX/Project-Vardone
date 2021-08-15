using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class BanGuildMemberController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId, [FromQuery] long guildId, [FromQuery] string reason = null)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.GuildsChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");
                if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("Second user is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    var guilds = dataContext.Guilds;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.User).Load();
                    bannedGuildMembers.Include(p => p.Guild).Load();

                    bannedGuildMembers.RemoveRange(bannedGuildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));
                    guildMembers.RemoveRange(guildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));

                    dataContext.SaveChanges();

                    var guild = guilds.First(p => p.Id == guildId);
                    var user = users.First(p => p.Id == secondId);

                    bannedGuildMembers.Add(new BannedGuildMembersTable { Guild = guild, Reason = reason, User = user });
                    dataContext.SaveChanges();
                    return Ok("Banned");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            })).GetAwaiter().GetResult();
        }
    }
}