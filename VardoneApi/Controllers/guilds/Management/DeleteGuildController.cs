using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Core;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds.Management
{
    [ApiController, Route("guilds/[controller]")]
    public class DeleteGuildController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long guildId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");

                try
                {
                    var dataContext = Program.DataContext;
                    var guilds = dataContext.Guilds;

                    guilds.Remove(guilds.First(p => p.Id == guildId));
                    dataContext.SaveChanges();
                    return Ok("Deleted");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            })).GetAwaiter().GetResult();
        }
    }
}