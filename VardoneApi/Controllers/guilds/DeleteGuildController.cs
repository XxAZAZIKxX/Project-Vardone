using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Core;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class DeleteGuildController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long guildId)
        {
            if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
            if (!GuildsChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
            if (!GuildsChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");

            try
            {
                var dataContext = Program.DataContext;
                var guilds = dataContext.Guilds;

                guilds.Remove(guilds.First(p => p.Id == guildId));
                dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return Ok("Deleted");
        }
    }
}