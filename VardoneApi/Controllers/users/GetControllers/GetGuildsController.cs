using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetGuildsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.Guild.Info).Load();
                    guildMembers.Include(p => p.User).Load();


                    var guilds = new List<Guild>();
                    foreach (var item in guildMembers.Where(p => p.User.Id == userId))
                    {
                        guilds.Add(new Guild
                        {
                            GuildId = item.Guild.Id,
                            Name = item.Guild.Name,
                            Base64Avatar = item.Guild.Info?.Avatar is not null
                                ? Convert.ToBase64String(item.Guild.Info.Avatar)
                                : null
                        });
                    }
                    return Ok(guilds);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}