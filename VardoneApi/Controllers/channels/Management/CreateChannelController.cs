using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Core;
using VardoneApi.Entity.Models.Channels;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.channels.Management
{
    [ApiController, Route("channels/[controller]")]
    public class CreateChannelController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long guildId, [FromQuery] string name = null)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("Not owner");
                try
                {
                    var dataContext = Program.DataContext;
                    var guilds = dataContext.Guilds;
                    var channels = dataContext.Channels;
                    var guild = guilds.First(p => p.Id == guildId);
                    channels.Add(new ChannelsTable { Name = name ?? "New channel", Guild = guild });
                    dataContext.SaveChanges();
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

                return Ok("Created");
            })).GetAwaiter().GetResult();
        }
    }
}