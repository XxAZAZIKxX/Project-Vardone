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
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Entities;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class GuildsController : ControllerBase
    {
        private static readonly Random Random = new();
        //
        [HttpPost, Route("getBannedGuildMembers")]
        public async Task<IActionResult> GetBannedGuildMembers([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
            }));
        }
        //
        [HttpPost, Route("getGuildChannels")]
        public async Task<IActionResult> GetGuildChannels([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
            }));
        }
        //
        [HttpPost, Route("getGuildInvites")]
        public async Task<IActionResult> GetGuildInvites([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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

            }));
        }
        //
        [HttpPost, Route("getGuildMembers")]
        public async Task<IActionResult> GetGuildMembers([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
            }));
        }
        //
        [HttpPost, Route("createGuild")]
        public async Task<IActionResult> CreateGuild([FromQuery] string name = null)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                    members.Add(new GuildMembersTable { User = user, Guild = guild });
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                    bannedGuildMembers.Include(p => p.User).Load();
                    bannedGuildMembers.Include(p => p.Guild).Load();

                    bannedGuildMembers.RemoveRange(bannedGuildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));
                    guildMembers.RemoveRange(guildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));

                    dataContext.SaveChanges();

                    var guild = guilds.First(p => p.Id == guildId);
                    var user = users.First(p => p.Id == secondId);

                    bannedGuildMembers.Add(new BannedGuildMembersTable { Guild = guild, Reason = reason, User = user });
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                    bannedGuildMembers.Include(p => p.User).Load();
                    bannedGuildMembers.Include(p => p.Guild).Load();

                    bannedGuildMembers.RemoveRange(bannedGuildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                    var guilds = dataContext.Guilds;
                    guilds.Include(p => p.Info).Load();
                    var users = dataContext.Users;
                    users.Include(p => p.Info).Load();

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
                        CreatedBy = new User
                        {
                            UserId = createdByUser.Id,
                            Username = createdByUser.Username,
                            Description = createdByUser.Info?.Description,
                            Base64Avatar = createdByUser.Info?.Avatar is not null ? Convert.ToBase64String(createdByUser.Info.Avatar) : null
                        },
                        Guild = new Guild
                        {
                            GuildId = guild.Id,
                            Name = guild.Name,
                            Base64Avatar = guild.Info?.Avatar is not null ? Convert.ToBase64String(guild.Info.Avatar) : null
                        }
                    });
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
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
        //
        [HttpPost, Route("joinGuild")]
        public async Task<IActionResult> JoinGuild([FromQuery] string inviteCode)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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
                    var guildId = guildInvites.First(p => p.InviteCode == inviteCode).Guild.Id;
                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.Guild).Load();
                    bannedGuildMembers.Include(p => p.User).Load();
                    var members = dataContext.GuildMembers;
                    members.Include(p => p.User).Load();
                    members.Include(p => p.Guild).Load();

                    foreach (var item in bannedGuildMembers.Where(p => p.Guild.Id == guildId)) if (item.User.Id == userId) return BadRequest("You are banned on this guild");

                    var guild = guilds.First(p => p.Id == guildId);
                    var user = users.First(p => p.Id == userId);
                    foreach (var table in members.Where(p => p.Guild == guild))
                    {
                        if (table.User.Id == userId) return Ok();
                    }

                    members.Add(new GuildMembersTable { User = user, Guild = guild });
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
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

                    guildMembers.RemoveRange(guildMembers.Where(p => p.User.Id == secondId && p.Guild.Id == guildId));
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
                long userId;
                string token;
                try
                {
                    userId = Convert.ToInt64(User.Claims.First(p => p.Type == "id").Value);
                    token = User.Claims.First(p => p.Type == "token").Value;
                }
                catch
                {
                    return BadRequest("Token parser problem");
                }
                if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");

                var dataContext = Program.DataContext;
                var members = dataContext.GuildMembers;
                members.Include(p => p.User).Load();
                members.Include(p => p.Guild).Load();

                try
                {
                    var first = members.First(p => p.User.Id == userId && p.Guild.Id == guildId);
                    members.Remove(first);
                    dataContext.SaveChanges();
                    return Ok("Leaved");
                }
                catch
                {
                    return Ok("You are not member");
                }
            }));
        }
    }
}