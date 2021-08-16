using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models.Channels;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.channels.Messages
{
    [ApiController, Route("channels/[controller]")]
    public class SendChannelMessageController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long channelId, [FromBody] MessageModel message)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.ChannelChecks.IsChannelExists(channelId)) return BadRequest("Channel is not exists");
                if (message is null) return BadRequest("Message is null");
                if (string.IsNullOrWhiteSpace(message.Text) && string.IsNullOrWhiteSpace(message.Base64Image)) return BadRequest("Empty message");

                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    var channelMessages = dataContext.ChannelMessages;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();

                    var channel = channels.First(p => p.Id == channelId);
                    var guild = channel.Guild;

                    if (!guildMembers.Any(p => p.User.Id == userId && p.Guild == guild)) return BadRequest("You are not member this guild");

                    channelMessages.Add(new ChannelMessagesTable
                    {
                        Author = users.First(p => p.Id == userId),
                        Channel = channel,
                        CreatedTime = DateTime.Now,
                        Image = message.Base64Image is not null ? Convert.FromBase64String(message.Base64Image) : null,
                        Text = message.Text ?? ""
                    });

                    dataContext.SaveChanges();
                    return Ok("Message was sent");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}