using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.channels
{
    [ApiController, Route("channels/[controller]")]
    public class GetChannelMessagesController : ControllerBase
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
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Author.Info).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();

                    var channel = channels.First(p => p.Id == channelId);

                    if (guildMembers.Count(p => p.Guild == channel.Guild && p.User.Id == userId) == 0) return BadRequest("You are not a member guild");

                    var channelMessagesList = new List<ChannelMessage>();

                    foreach (var item in channelMessages.Where(p => p.Channel.Id == channelId))
                    {
                        channelMessagesList.Add(new ChannelMessage
                        {
                            MessageId = item.Id,
                            Text = item.Text,
                            CreatedTime = item.CreatedTime,
                            Base64Image = item.Image is not null ? Convert.ToBase64String(item.Image) : null,
                            Author = new User
                            {
                                UserId = item.Author.Id,
                                Username = item.Author.Username,
                                Base64Avatar = item.Author.Info?.Avatar is not null ? Convert.ToBase64String(item.Author.Info.Avatar) : null,
                                Description = item.Author.Info?.Description
                            },
                            Channel = new Channel
                            {
                                ChannelId = item.Channel.Id,
                                Name = item.Channel.Name,
                                Guild = new Guild
                                {
                                    GuildId = item.Channel.Guild.Id,
                                    Name = item.Channel.Guild.Name,
                                    Base64Avatar = item.Channel.Guild.Info?.Avatar is not null ? Convert.ToBase64String(item.Channel.Guild.Info.Avatar) : null
                                }
                            }
                        });
                    }
                    return Ok(channelMessagesList);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            })).GetAwaiter().GetResult();
        }
    }
}