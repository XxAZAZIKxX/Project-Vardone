using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.channels
{
    [ApiController, Route("channels/[controller]")]
    public class DeleteChannelController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long channelId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.ChannelChecks.IsChannelExists(channelId)) return BadRequest("Channel is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();
                    channels.Include(p => p.Guild.Owner).Load();

                    var channel = channels.First(p => p.Id == channelId);
                    if (channel.Guild.Owner.Id != userId) return BadRequest();

                    channels.Remove(channel);
                    dataContext.SaveChanges();
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

                return Ok("Deleted");
            })).GetAwaiter().GetResult();
        }
    }
}