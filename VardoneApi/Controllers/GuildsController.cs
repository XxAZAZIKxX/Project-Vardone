using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core;
using VardoneApi.Core.Checks;
using VardoneApi.Core.CreateHelpers;
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Entities;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class GuildsController : ControllerBase
    {
        [HttpPost, Route("getBannedGuildMembers")]
        public async Task<IActionResult> GetBannedGuildMembers([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");

                try
                {
                    var dataContext = Program.DataContext;
                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.Guild).Load();
                    bannedGuildMembers.Include(p => p.Guild.Info).Load();
                    bannedGuildMembers.Include(p => p.BannedUser).Load();
                    bannedGuildMembers.Include(p => p.BannedUser.Info).Load();
                    bannedGuildMembers.Include(p => p.BannedByUser).Load();
                    bannedGuildMembers.Include(p => p.BannedByUser.Info).Load();

                    var bannedUsers = new List<BannedMember>();
                    foreach (var item in bannedGuildMembers.Where(p => p.Guild.Id == guildId))
                    {
                        bannedUsers.Add(new BannedMember
                        {
                            BannedUser = UserCreateHelper.GetUser(item.BannedUser),
                            BannedByUser = UserCreateHelper.GetUser(item.BannedByUser),
                            Reason = item.Reason,
                            BanDateTime = item.BanDate,
                            Guild = GuildCreateHelper.GetGuild(item.Guild)
                        });
                    }
                    return Ok(bannedUsers);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getGuildChannels")]
        public async Task<IActionResult> GetGuildChannels([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var channels = dataContext.Channels;
                    channels.Include(p => p.Guild).Load();
                    channels.Include(p => p.Guild.Info).Load();
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.Guild.Owner).Load();
                    guildMembers.Include(p => p.Guild.Owner.Info).Load();
                    guildMembers.Include(p => p.User).Load();

                    if (guildMembers.Count(p => p.User.Id == userId && p.Guild.Id == guildId) == 0) return BadRequest("You are not a guild member");

                    var channelsList = new List<Channel>();

                    foreach (var itemChannelsTable in channels.Where(p => p.Guild.Id == guildId))
                    {
                        channelsList.Add(new Channel
                        {
                            ChannelId = itemChannelsTable.Id,
                            Name = itemChannelsTable.Name,
                            Guild = GuildCreateHelper.GetGuild(itemChannelsTable.Guild)
                        });
                    }

                    return Ok(channelsList);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getGuildInvites")]
        public async Task<IActionResult> GetGuildInvites([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildInvites = dataContext.GuildInvites;
                    guildInvites.Include(p => p.Guild).Load();
                    guildInvites.Include(p => p.Guild.Info).Load();
                    guildInvites.Include(p => p.CreatedByUser).Load();
                    guildInvites.Include(p => p.CreatedByUser.Info).Load();

                    var invites = new List<GuildInvite>();

                    InviteChecks.ClearExpiredInvites();

                    foreach (var item in guildInvites.Where(p => p.Guild.Id == guildId))
                    {
                        invites.Add(new GuildInvite
                        {
                            InviteId = item.Id,
                            CreatedAt = item.CreatedAt,
                            InviteCode = item.InviteCode,
                            CreatedBy = UserCreateHelper.GetUser(item.CreatedByUser),
                            Guild = GuildCreateHelper.GetGuild(item.Guild),
                            NumberOfUses = item.NumberOfUses
                        });
                    }
                    return Ok(invites);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            }));
        }
        //
        [HttpPost, Route("getGuildMembers")]
        public async Task<IActionResult> GetGuildMembers([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var members = dataContext.GuildMembers;
                    members.Include(p => p.Guild).Load();
                    members.Include(p => p.User).Load();
                    members.Include(p => p.User.Info).Load();

                    var returnMembers = new List<Member>();
                    if (!members.Any(p => p.Guild.Id == guildId && p.User.Id == userId)) return BadRequest("You not member that guild");

                    foreach (var member in members.Where(p => p.Guild.Id == guildId))
                    {
                        returnMembers.Add(UserCreateHelper.GetMember(member));
                    }

                    return Ok(returnMembers);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("createGuild")]
        public async Task<IActionResult> CreateGuild([FromQuery] string name = null)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                try
                {
                    var dataContext = Program.DataContext;
                    var guilds = dataContext.Guilds;
                    var users = dataContext.Users;
                    var members = dataContext.GuildMembers;

                    var user = users.First(p => p.Id == userId);

                    var guild = new GuildsTable
                    {
                        CreatedAt = DateTime.Now,
                        Name = name ?? $"{user.Username}'s server",
                        Owner = user
                    };
                    guilds.Add(guild);
                    dataContext.SaveChanges();
                    members.Add(new GuildMembersTable { User = user, Guild = guild, JoinDate = DateTime.Now });
                    dataContext.SaveChanges();
                    return Ok("Created");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            }));
        }
        //
        [HttpPost, Route("deleteGuild")]
        public async Task<IActionResult> DeleteGuild([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");

                try
                {
                    var dataContext = Program.DataContext;
                    var guilds = dataContext.Guilds;

                    guilds.Remove(guilds.First(p => p.Id == guildId));
                    dataContext.SaveChanges();
                    return Ok("Deleted");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }

            }));
        }
        //
        [HttpPost, Route("deleteGuildInvite")]
        public async Task<IActionResult> DeleteGuildInvite([FromQuery] long inviteId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!InviteChecks.IsInviteExists(inviteId)) return BadRequest("Invite is not exists");

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
            }));
        }
        //
        [HttpPost, Route("updateGuild")]
        public async Task<IActionResult> UpdateGuild([FromBody] UpdateGuildModel updateModel)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsUserOwner(userId, updateModel.GuildId)) return BadRequest("You are not owner");
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
            }));
        }
        //
        [HttpPost, Route("banGuildMember")]
        public async Task<IActionResult> BanGuildMember([FromQuery] long secondId, [FromQuery] long guildId, [FromQuery] string reason = null)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");
                if (!UserChecks.IsUserExists(secondId)) return BadRequest("Second user is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    var guilds = dataContext.Guilds;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.BannedUser).Load();
                    bannedGuildMembers.Include(p => p.Guild).Load();
                    var invites = dataContext.GuildInvites;
                    invites.Include(p => p.CreatedByUser).Load();

                    bannedGuildMembers.RemoveRange(bannedGuildMembers.Where(p => p.BannedUser.Id == secondId && p.Guild.Id == guildId));
                    guildMembers.RemoveRange(guildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));
                    invites.RemoveRange(invites.Where(p => p.CreatedByUser.Id == secondId && p.Guild.Id == guildId));

                    dataContext.SaveChanges();

                    var guild = guilds.First(p => p.Id == guildId);
                    var bannedUser = users.First(p => p.Id == secondId);
                    var bannedByUser = users.First(p => p.Id == userId);

                    bannedGuildMembers.Add(new BannedGuildMembersTable { Guild = guild, Reason = reason, BannedUser = bannedUser, BannedByUser = bannedByUser, BanDate = DateTime.Now });
                    dataContext.SaveChanges();
                    return Ok("Banned");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("unbanGuildMember")]
        public async Task<IActionResult> UnbanGuildMember([FromQuery] long secondId, [FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");
                if (!UserChecks.IsUserExists(secondId)) return BadRequest("Second user is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.BannedUser).Load();
                    bannedGuildMembers.Include(p => p.Guild).Load();

                    bannedGuildMembers.RemoveRange(bannedGuildMembers.Where(p => p.BannedUser.Id == secondId && p.Guild.Id == guildId));
                    dataContext.SaveChanges();
                    return Ok("Unbanned");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("createGuildInvite")]
        public async Task<IActionResult> CreateGuildInvite([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
                if (!GuildChecks.IsUserMember(userId, guildId)) return BadRequest("You are not member");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildInvites = dataContext.GuildInvites;
                    guildInvites.Include(p => p.Guild).Load();
                    guildInvites.Include(p => p.Guild.Info).Load();
                    var guilds = dataContext.Guilds;
                    guilds.Include(p => p.Info).Load();
                    var users = dataContext.Users;
                    users.Include(p => p.Info).Load();

                    InviteChecks.ClearExpiredInvites();

                    var inviteCode = CreateInviteCode();
                    while (InviteChecks.IsInviteExists(inviteCode)) inviteCode = CreateInviteCode();

                    var createdByUser = users.First(p => p.Id == userId);
                    var guild = guilds.First(p => p.Id == guildId);
                    var guildInvite = new GuildInvitesTable
                    {
                        CreatedAt = DateTime.Now,
                        CreatedByUser = createdByUser,
                        Guild = guild,
                        InviteCode = inviteCode
                    };
                    guildInvites.Add(guildInvite);
                    dataContext.SaveChanges();
                    return Ok(new GuildInvite
                    {
                        InviteId = guildInvite.Id,
                        InviteCode = guildInvite.InviteCode,
                        CreatedAt = guildInvite.CreatedAt,
                        CreatedBy = UserCreateHelper.GetUser(guildInvite.CreatedByUser),
                        Guild = GuildCreateHelper.GetGuild(guildInvite.Guild),
                        NumberOfUses = guildInvite.NumberOfUses
                    });
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("joinGuild")]
        public async Task<IActionResult> JoinGuild([FromQuery] string inviteCode)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!InviteChecks.IsInviteExists(inviteCode)) return BadRequest("Invite is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    var guilds = dataContext.Guilds;

                    var guildInvites = dataContext.GuildInvites;
                    guildInvites.Include(p => p.Guild).Load();

                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.Guild).Load();
                    bannedGuildMembers.Include(p => p.BannedUser).Load();

                    var members = dataContext.GuildMembers;
                    members.Include(p => p.User).Load();
                    members.Include(p => p.Guild).Load();

                    var invite = guildInvites.First(p => p.InviteCode == inviteCode);
                    var guildId = invite.Guild.Id;

                    if (bannedGuildMembers.Any(p => p.Guild.Id == guildId && p.BannedUser.Id == userId)) return BadRequest("You are banned on this guild");

                    var guild = guilds.First(p => p.Id == guildId);
                    var user = users.First(p => p.Id == userId);
                    foreach (var table in members.Where(p => p.Guild == guild))
                    {
                        if (table.User.Id == userId) return Ok();
                    }

                    members.Add(new GuildMembersTable { User = user, Guild = guild, JoinDate = DateTime.Now });
                    members.First(p => p.User.Id == invite.CreatedByUser.Id).NumberOfInvitedMembers++;
                    invite.NumberOfUses++;
                    dataContext.SaveChanges();
                    return Ok("Joined");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("kickGuildMember")]
        public async Task<IActionResult> KickGuildMember([FromQuery] long secondId, [FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsUserOwner(userId, guildId)) return BadRequest("You are not owner");
                if (!UserChecks.IsUserExists(secondId)) return BadRequest("Second user is not exists");

                try
                {
                    var dataContext = Program.DataContext;
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var invites = dataContext.GuildInvites;
                    invites.Include(p => p.CreatedByUser).Load();

                    guildMembers.RemoveRange(guildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));
                    invites.RemoveRange(invites.Where(p => p.CreatedByUser.Id == secondId && p.Guild.Id == guildId));

                    dataContext.SaveChanges();
                    return Ok("Kicked");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("leaveGuild")]
        public async Task<IActionResult> LeaveGuild([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");

                var dataContext = Program.DataContext;
                var members = dataContext.GuildMembers;
                members.Include(p => p.User).Load();
                members.Include(p => p.Guild).Load();
                var invites = dataContext.GuildInvites;
                invites.Include(p => p.CreatedByUser).Load();

                if (!members.Any(p => p.User.Id == userId && p.Guild.Id == guildId)) return Ok("You are not member");

                var first = members.First(p => p.User.Id == userId && p.Guild.Id == guildId);
                members.Remove(first);
                invites.RemoveRange(invites.Where(p => p.CreatedByUser.Id == userId && p.Guild.Id == guildId));
                dataContext.SaveChanges();
                return Ok("Leaved");
            }));
        }

        //Methods
        private static string CreateInviteCode() => CryptographyTools.CreateRandomString(6, 10);
    }
}