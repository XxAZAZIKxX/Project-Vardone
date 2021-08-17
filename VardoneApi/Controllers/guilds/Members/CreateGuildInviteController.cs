using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds.Members
{
    [ApiController, Route("guilds/[controller]")]
    public class CreateGuildInviteController : ControllerBase
    {
        private static readonly Random Random = new();
        // POST
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long guildId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");
                if (!Core.GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
                if (!Core.GuildChecks.IsUserMember(userId, guildId)) return BadRequest("You are not member");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildInvites = dataContext.GuildInvites;
                    var guilds = dataContext.Guilds;
                    var users = dataContext.Users;

                    var inviteCode = CreateInviteCode();
                    while (Core.InviteChecks.IsInviteExists(inviteCode)) inviteCode = CreateInviteCode();

                    guildInvites.Add(new GuildInvitesTable
                    {
                        CreatedAt = DateTime.Now,
                        CreatedByUser = users.First(p => p.Id == userId),
                        Guild = guilds.First(p => p.Id == guildId),
                        InviteCode = inviteCode
                    });

                    dataContext.SaveChanges();
                    return Ok(inviteCode);

                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }

        private static string CreateInviteCode()
        {
            var sb = new StringBuilder();
            var n = Random.Next(6, 9);
            for (var i = 0; i < n; i++)
            {
                switch (Random.Next(1, 3))
                {
                    case 1:
                        sb.Append((char)Random.Next(65, 91));
                        break;
                    case 2:
                        sb.Append((char)Random.Next(97, 123));
                        break;
                }
            }
            return sb.ToString();
        }
    }
}