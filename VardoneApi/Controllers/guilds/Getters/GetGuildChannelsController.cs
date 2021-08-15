using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class GetGuildChannelsController : ControllerBase
    {
        // POST
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
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();
                    channels.Include(p => p.Guild.Info).Load();
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();

                    if (guildMembers.Count(p => p.User.Id == userId && p.Guild.Id == guildId) == 0) return BadRequest("You are not a guild member");

                    var channelsList = new List<Channel>();

                    foreach (var itemChannelsTable in channels.Where(p => p.Guild.Id == guildId))
                    {
                        channelsList.Add(new Channel
                        {
                            ChannelId = itemChannelsTable.Id,
                            Name = itemChannelsTable.Name,
                            Guild = new Guild
                            {
                                GuildId = itemChannelsTable.Guild.Id,
                                Name = itemChannelsTable.Guild.Name,
                                Base64Avatar = itemChannelsTable.Guild.Info?.Avatar is not null
                                    ? Convert.ToBase64String(itemChannelsTable.Guild.Info.Avatar)
                                    : null
                            }
                        });
                    }

                    return Ok(channelsList);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}