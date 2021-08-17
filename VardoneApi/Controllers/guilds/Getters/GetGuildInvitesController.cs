using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds.Getters
{
    [ApiController, Route("guilds/[controller]")]
    public class GetGuildInvitesController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long guildId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildInvites = dataContext.GuildInvites;
                    guildInvites.Include(p => p.Guild).Load();
                    guildInvites.Include(p => p.Guild.Info).Load();
                    guildInvites.Include(p => p.CreatedByUser).Load();
                    guildInvites.Include(p => p.CreatedByUser.Info).Load();

                    var invites = new List<GuildInvite>();

                    foreach (var item in guildInvites.Where(p => p.Guild.Id == guildId))
                    {
                        invites.Add(new GuildInvite
                        {
                            InviteId = item.Id,
                            CreatedAt = item.CreatedAt,
                            InviteCode = item.InviteCode,
                            CreatedBy = new User
                            {
                                UserId = item.CreatedByUser.Id,
                                Username = item.CreatedByUser.Username,
                                Description = item.CreatedByUser.Info?.Description,
                                Base64Avatar = item.CreatedByUser.Info?.Avatar is not null ? Convert.ToBase64String(item.CreatedByUser.Info.Avatar) : null
                            },
                            Guild = new Guild
                            {
                                GuildId = item.Guild.Id,
                                Name = item.Guild.Name,
                                Base64Avatar = item.Guild.Info?.Avatar is not null ? Convert.ToBase64String(item.Guild.Info.Avatar) : null
                            }
                        });
                    }
                    return Ok(invites);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            })).GetAwaiter().GetResult();
        }
    }
}