using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds.Management
{
    [ApiController, Route("guilds/[controller]")]
    public class DeleteGuildInviteController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long inviteId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.InviteChecks.IsInviteExists(inviteId)) return BadRequest("Invite is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildInvites = dataContext.GuildInvites;
                    guildInvites.Include(p => p.Guild).Load();
                    guildInvites.Include(p => p.Guild.Owner).Load();

                    var invite = guildInvites.First(p => p.Id == inviteId);
                    if (invite.Guild.Owner.Id != userId) return BadRequest("You are not owner");

                    guildInvites.Remove(invite);
                    dataContext.SaveChanges();
                    return Ok("Invite deleted");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}