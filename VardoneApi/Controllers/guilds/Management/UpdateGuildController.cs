using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Models.GeneralModels.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds.Management
{
    [ApiController, Route("guilds/[controller]")]
    public class UpdateGuildController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromBody] UpdateGuildModel updateModel)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.GuildChecks.IsUserOwner(userId, updateModel.GuildId)) return BadRequest("You are not owner");
                try
                {
                    var dataContext = Program.DataContext;
                    var guildInfos = dataContext.GuildInfos;
                    guildInfos.Include(p => p.Guild).Load();
                    var guilds = dataContext.Guilds;

                    var guild = guilds.First(p => p.Id == updateModel.GuildId);

                    GuildInfosTable info;

                    if (guild.Info is null)
                    {
                        info = new GuildInfosTable { Guild = guild };
                        guildInfos.RemoveRange(guildInfos.Where(p => p.Guild.Id == guild.Id));
                        guildInfos.Add(info);
                        dataContext.SaveChanges();
                        info = guildInfos.First(p => p.Guild.Id == guild.Id);
                        guild.Info = info;
                    }
                    else info = guild.Info;

                    guild.Name = updateModel.Name ?? guild.Name;
                    if (updateModel.Base64Image is not null) info.Avatar = updateModel.Base64Image is "" ? null : Convert.FromBase64String(updateModel.Base64Image);

                    guilds.Update(guild);
                    guildInfos.Update(info);
                    dataContext.SaveChanges();
                    return Ok("Updated");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            })).GetAwaiter().GetResult();
        }
    }
}