using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.channels.Messages
{
    [ApiController, Route("channels/[controller]")]
    public class DeleteChannelMessageController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long messageId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

                try
                {
                    var dataContext = Program.DataContext;
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    channelMessages.Include(p => p.Channel.Guild.Owner).Load();

                    if (channelMessages.Count(p => p.Id == messageId) == 0) return BadRequest("Message is not exists");

                    var channelMessage = channelMessages.First(p => p.Id == messageId);

                    if (channelMessage.Author.Id != userId && channelMessage.Channel.Guild.Owner.Id != userId) return BadRequest("You cannot delete this message");

                    channelMessages.Remove(channelMessage);
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