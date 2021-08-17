using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds.Getters
{
    [ApiController, Route("guilds/[controller]")]
    public class GetBannedGuildMembersController : ControllerBase
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
                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.Guild).Load();
                    bannedGuildMembers.Include(p => p.User).Load();
                    bannedGuildMembers.Include(p => p.User.Info).Load();

                    var bannedUsers = new List<BannedUser>();
                    foreach (var item in bannedGuildMembers.Where(p => p.Guild.Id == guildId))
                    {
                        bannedUsers.Add(new BannedUser
                        {
                            User = new User
                            {
                                UserId = item.User.Id,
                                Username = item.User.Username,
                                Description = item.User.Info?.Description,
                                Base64Avatar = item.User.Info?.Description is not null ? Convert.ToBase64String(item.User.Info.Avatar) : null
                            },
                            Reason = item.Reason
                        });
                    }
                    return Ok(bannedUsers);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}