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
    public class GetGuildMembersController : ControllerBase
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
                    var members = dataContext.GuildMembers;
                    members.Include(p => p.Guild).Load();
                    members.Include(p => p.User).Load();
                    members.Include(p => p.User.Info).Load();

                    var users = new List<User>();
                    try
                    {
                        var _ = members.First(p => p.User.Id == userId && p.Guild.Id == guildId);
                        foreach (var member in members.Where(p => p.Guild.Id == guildId))
                        {
                            users.Add(new User
                            {
                                UserId = member.User.Id,
                                Username = member.User.Username,
                                Description = member.User.Info?.Description,
                                Base64Avatar = member.User.Info?.Avatar is not null ? Convert.ToBase64String(member.User.Info.Avatar) : null
                            });
                        }

                        return Ok(users);
                    }
                    catch
                    {
                        return BadRequest("You not member that guild");
                    }
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}