using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core;
using VardoneApi.Core.Checks;
using VardoneApi.Core.CreateHelpers;
using VardoneApi.Entity.Models.Guilds;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;
using VardoneEntities.Models.TcpModels;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class GuildsController : ControllerBase
    {
        [HttpPost, Route("getGuild")]
        public async Task<IActionResult> GetGuild([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
                if (!GuildChecks.IsUserMember(token.UserId, guildId)) return BadRequest("You are not a member");
                try
                {
                    var dataContext = Program.DataContext;
                    var guilds = dataContext.Guilds;
                    guilds.Include(p => p.Info).Load();
                    guilds.Include(p => p.Owner).Load();
                    var guild = guilds.FirstOrDefault(p => p.Id == guildId);
                    if (guild == null) return BadRequest("Guild is not exists");
                    return Ok(GuildCreateHelper.GetGuild(guild.Id));
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getBannedGuildMembers")]
        public async Task<IActionResult> GetBannedGuildMembers([FromQuery] long guildId)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
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
                        var member = new BannedMember
                        {
                            BannedUser = UserCreateHelper.GetUser(item.BannedUser.Id),
                            BannedByUser = UserCreateHelper.GetUser(item.BannedByUser.Id),
                            Guild = GuildCreateHelper.GetGuild(item.Guild.Id, false),
                            BanDateTime = item.BanDate,
                            Reason = item.Reason
                        };
                        bannedUsers.Add(member);
                    }
                    return Ok(bannedUsers.ToArray());
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
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
                if (!GuildChecks.IsUserMember(userId, guildId)) return BadRequest("You are not a guild member");
                try
                {
                    return Ok(GuildCreateHelper.GetGuildChannels(guildId).ToArray());
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                        invites.Add(GuildCreateHelper.GetGuildInvite(item.Id));
                    }
                    return Ok(invites.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getGuildMembers")]
        public async Task<IActionResult> GetGuildMembers([FromQuery] long guildId, [FromHeader] bool onlyId = false)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");
                if (!GuildChecks.IsUserMember(userId, guildId)) return BadRequest("You not member that guild");
                try
                {
                    var dataContext = Program.DataContext;
                    var members = dataContext.GuildMembers;
                    members.Include(p => p.Guild).Load();
                    members.Include(p => p.User).Load();
                    members.Include(p => p.User.Info).Load();


                    var returnMembers = new List<Member>();
                    foreach (var member in members.Where(p => p.Guild.Id == guildId))
                    {
                        returnMembers.Add(UserCreateHelper.GetMember(member.Id));
                    }

                    return Ok(returnMembers.ToArray());
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    var guildMembersTable = new GuildMembersTable { User = user, Guild = guild, JoinDate = DateTime.Now };
                    members.Add(guildMembersTable);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(userId, new TcpResponseModel
                        {
                            type = TypeTcpResponse.GuildJoin,
                            data = GuildCreateHelper.GetGuild(guild.Id)
                        });
                    });
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.User).Load();
                    guildMembers.Include(p => p.Guild).Load();
                    var guild = guilds.First(p => p.Id == guildId);
                    var tcpNotify = new TcpResponseModel
                    {
                        type = TypeTcpResponse.GuildLeave,
                        data = GuildCreateHelper.GetGuild(guild.Id, false, false, true)
                    };
                    var idsToNotify = guildMembers.Where(p => p.Guild.Id == guildId).Select(p => p.User.Id).ToArray();
                    guilds.Remove(guild);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        foreach (var id in idsToNotify) Program.TcpServer.SendMessageTo(id, tcpNotify);
                    });

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
                var token = JwtTokenWorker.GetUserToken(User);
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

                    var inviteTable = guildInvites.First(p => p.Id == inviteId);
                    if (inviteTable.Guild.Owner.Id != userId) return BadRequest("You are not owner");
                    var invite = GuildCreateHelper.GetGuildInvite(inviteTable.Id);
                    guildInvites.Remove(inviteTable);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(userId, new TcpResponseModel
                        {
                            type = TypeTcpResponse.DeleteGuildInvite,
                            data = invite
                        });
                    });
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();

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
                    if (updateModel.Base64Image is not null)
                    {
                        var fromBase64String = Convert.FromBase64String(updateModel.Base64Image);
                        if (ImageWorker.IsImage(fromBase64String)) info.Avatar = updateModel.Base64Image is "" ? null : ImageWorker.CompressImageQualityLevel(fromBase64String, 45);
                    }

                    guilds.Update(guild);
                    guildInfos.Update(info);
                    dataContext.SaveChanges();

                    Task.Run(() =>
                    {
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.UpdateGuild,
                            data = GuildCreateHelper.GetGuild(guild.Id)
                        };
                        foreach (var id in guildMembers.Where(p => p.Guild.Id == guild.Id).Select(p => p.User.Id).ToArray())
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                        }
                    });
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    var invites = dataContext.GuildInvites;
                    invites.Include(p => p.CreatedByUser).Load();
                    invites.Include(p => p.Guild).Load();
                    //
                    var guild = guilds.First(p => p.Id == guildId);
                    var bannedUser = users.First(p => p.Id == secondId);
                    var bannedByUser = users.First(p => p.Id == userId);

                    var userChannelMessages = channelMessages.Where(p => p.Author.Id == secondId && p.Channel.Guild.Id == guildId).ToArray();
                    var userInvites = invites.Where(p => p.CreatedByUser.Id == secondId && p.Guild.Id == guildId).ToArray();

                    if (bannedGuildMembers.Any(p => p.BannedUser.Id == secondId && p.Guild.Id == guildId)) return Ok("User is already banned");
                    bannedGuildMembers.Add(new BannedGuildMembersTable { Guild = guild, Reason = reason, BannedUser = bannedUser, BannedByUser = bannedByUser, BanDate = DateTime.Now });

                    var members = guildMembers.Where(p => p.Guild.Id == guildId).ToArray();
                    var member = members.FirstOrDefault(p => p.User.Id == secondId && p.Guild.Id == guildId);
                    if (member is null) return BadRequest("User is not a member");
                    var memberOb = UserCreateHelper.GetMember(secondId, guildId, true);
                    guildMembers.Remove(member);

                    var userInvitesObs = userInvites.Select(p => GuildCreateHelper.GetGuildInvite(p.Id)).ToArray();
                    invites.RemoveRange(userInvites);

                    var userMessageObs = userChannelMessages.Select(p => MessageCreateHelper.GetChannelMessage(p.Id, true)).ToArray();
                    channelMessages.RemoveRange(userChannelMessages);
                    dataContext.SaveChanges();

                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(secondId, new TcpResponseModel
                        {
                            type = TypeTcpResponse.GuildLeave,
                            data = GuildCreateHelper.GetGuild(guildId, false, false, true)
                        });
                        Program.TcpServer.SendMessageTo(userId, new TcpResponseModel
                        {
                            type = TypeTcpResponse.BanMember,
                            data = UserCreateHelper.GetBannedMember(secondId, guildId)
                        });
                        var tcpNoticeDeleteMember = new TcpResponseModel
                        {
                            type = TypeTcpResponse.DeleteMember,
                            data = memberOb
                        };
                        foreach (var id in members.Where(p => p.User.Id != secondId).Select(p => p.User.Id).ToArray())
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNoticeDeleteMember);
                            foreach (var message in userMessageObs)
                            {
                                Program.TcpServer.SendMessageTo(id, new TcpResponseModel
                                {
                                    type = TypeTcpResponse.DeleteChannelMessage,
                                    data = message
                                });
                            }
                        }
                        foreach (var invite in userInvitesObs)
                        {
                            Program.TcpServer.SendMessageTo(userId, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteGuildInvite,
                                data = invite
                            });
                        }
                    });
                    return Ok("User was banned");
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
                var token = JwtTokenWorker.GetUserToken(User);
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

                    var bannedMember = bannedGuildMembers.FirstOrDefault(p => p.BannedUser.Id == secondId && p.Guild.Id == guildId);
                    if (bannedMember is null) return Ok("User not banned");
                    var bannedMemberObj = UserCreateHelper.GetBannedMember(bannedMember.Id, true);
                    bannedGuildMembers.Remove(bannedMember);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(secondId, new TcpResponseModel
                        {
                            type = TypeTcpResponse.UnbanMember,
                            data = bannedMemberObj
                        });
                    });
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    guilds.Include(p => p.Owner).Load();
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
                    var invite = GuildCreateHelper.GetGuildInvite(guildInvite.Id);
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(guild.Owner.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewGuildInvite,
                            data = invite
                        });
                    });
                    return Ok(invite);
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    guildInvites.Include(p => p.CreatedByUser).Load();
                    var bannedGuildMembers = dataContext.BannedGuildMembers;
                    bannedGuildMembers.Include(p => p.Guild).Load();
                    bannedGuildMembers.Include(p => p.BannedUser).Load();
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.User).Load();
                    guildMembers.Include(p => p.Guild).Load();

                    var invite = guildInvites.FirstOrDefault(p => p.InviteCode == inviteCode);
                    if (invite is null) return BadRequest("Invite is not exists");
                    var guildId = invite.Guild.Id;

                    if (bannedGuildMembers.Any(p => p.Guild.Id == guildId && p.BannedUser.Id == userId)) return BadRequest("You are banned on this guild");

                    var guild = guilds.First(p => p.Id == guildId);
                    var user = users.First(p => p.Id == userId);
                    var members = guildMembers.Where(p => p.Guild.Id == guild.Id).ToArray();
                    if (members.Any(p => p.User.Id == userId)) return Ok("You already on this server");

                    guildMembers.Add(new GuildMembersTable { User = user, Guild = guild, JoinDate = DateTime.Now });
                    guildMembers.First(p => p.User.Id == invite.CreatedByUser.Id && p.Guild.Id == invite.Guild.Id).NumberOfInvitedMembers++;
                    invite.NumberOfUses++;
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(userId, new TcpResponseModel
                        {
                            type = TypeTcpResponse.GuildJoin,
                            data = GuildCreateHelper.GetGuild(guildId)
                        });
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewMember,
                            data = UserCreateHelper.GetMember(userId, guildId)
                        };
                        var uIds = members.Where(p => p.User.Id != userId).Select(p => p.User.Id);
                        foreach (var id in uIds)
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                        }
                    });
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
                var token = JwtTokenWorker.GetUserToken(User);
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
                    var channelMessages = dataContext.ChannelMessages;
                    channelMessages.Include(p => p.Author).Load();
                    channelMessages.Include(p => p.Channel).Load();
                    channelMessages.Include(p => p.Channel.Guild).Load();
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.User).Load();
                    var invites = dataContext.GuildInvites;
                    invites.Include(p => p.CreatedByUser).Load();

                    var members = guildMembers.Where(p => p.Guild.Id == guildId).ToArray();
                    var member = members.FirstOrDefault(p => p.User.Id == secondId && p.Guild.Id == guildId);
                    if (member is null) return BadRequest("User is not a member");
                    guildMembers.Remove(member);

                    var userInvites = invites.Where(p => p.CreatedByUser.Id == secondId && p.Guild.Id == guildId).ToArray();
                    var userInvitesObs = userInvites.Select(p => GuildCreateHelper.GetGuildInvite(p.Id));
                    invites.RemoveRange(userInvites);

                    var userChannelMessages = channelMessages.Where(p => p.Author.Id == secondId && p.Channel.Guild.Id == guildId).ToArray();
                    var userChannelMessagesObs = userChannelMessages.Select(p => MessageCreateHelper.GetChannelMessage(p.Id, true)).ToArray();
                    channelMessages.RemoveRange(userChannelMessages);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(secondId, new TcpResponseModel
                        {
                            type = TypeTcpResponse.GuildLeave,
                            data = GuildCreateHelper.GetGuild(guildId, false, false, true)
                        });
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.DeleteMember,
                            data = member
                        };
                        foreach (var id in members.Where(p => p.User.Id != secondId).Select(p => p.User.Id).ToArray())
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                            foreach (var message in userChannelMessagesObs)
                            {
                                Program.TcpServer.SendMessageTo(id, new TcpResponseModel
                                {
                                    type = TypeTcpResponse.DeleteChannelMessage,
                                    data = message
                                });
                            }
                        }

                        foreach (var invite in userInvitesObs)
                        {
                            Program.TcpServer.SendMessageTo(userId, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteGuildInvite,
                                data = invite
                            });
                        }
                    });
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
                var token = JwtTokenWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!GuildChecks.IsGuildExists(guildId)) return BadRequest("Guild is not exists");

                var dataContext = Program.DataContext;
                var guildMembers = dataContext.GuildMembers;
                guildMembers.Include(p => p.User).Load();
                guildMembers.Include(p => p.Guild).Load();
                guildMembers.Include(p => p.Guild.Owner).Load();
                var invites = dataContext.GuildInvites;
                invites.Include(p => p.CreatedByUser).Load();
                var channelMessages = dataContext.ChannelMessages;
                channelMessages.Include(p => p.Author).Load();
                channelMessages.Include(p => p.Channel).Load();
                channelMessages.Include(p => p.Channel.Guild).Load();

                if (!guildMembers.Any(p => p.User.Id == userId && p.Guild.Id == guildId)) return Ok("You are not member");

                var userChannelMessages = channelMessages.Where(p => p.Author.Id == userId).ToArray();
                var userChannelMessagesObs = userChannelMessages.Select(p => MessageCreateHelper.GetChannelMessage(p.Id, true)).ToArray();
                channelMessages.RemoveRange(userChannelMessages);

                var members = guildMembers.Where(p => p.Guild.Id == guildId).ToArray();

                var member = guildMembers.FirstOrDefault(p => p.User.Id == userId && p.Guild.Id == guildId);
                if (member is null) return BadRequest("You are not a member");

                var owner = member.Guild.Owner;
                var memberOb = UserCreateHelper.GetMember(member.Id, true);
                guildMembers.Remove(member);


                var userInvites = invites.Where(p => p.CreatedByUser.Id == userId && p.Guild.Id == guildId).ToArray();
                var userInviteObs = userInvites.Select(p => GuildCreateHelper.GetGuildInvite(p.Id));
                invites.RemoveRange(invites.Where(p => p.CreatedByUser.Id == userId && p.Guild.Id == guildId));
                dataContext.SaveChanges();
                Task.Run(() =>
                {
                    Program.TcpServer.SendMessageTo(userId, new TcpResponseModel
                    {
                        type = TypeTcpResponse.GuildLeave,
                        data = GuildCreateHelper.GetGuild(guildId, false, false, true)
                    });
                    var tcpNotify = new TcpResponseModel
                    {
                        type = TypeTcpResponse.DeleteMember,
                        data = memberOb
                    };
                    foreach (var id in members.Where(p => p.User.Id != userId).Select(p => p.User.Id).ToArray())
                    {
                        Program.TcpServer.SendMessageTo(id, tcpNotify);
                        foreach (var message in userChannelMessagesObs)
                        {
                            Program.TcpServer.SendMessageTo(id, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteChannelMessage,
                                data = message
                            });
                        }
                    }

                    foreach (var invite in userInviteObs)
                    {
                        Program.TcpServer.SendMessageTo(owner.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.DeleteGuildInvite,
                            data = invite
                        });
                    }
                });
                return Ok("Leaved");
            }));
        }

        //Methods
        private static string CreateInviteCode() => CryptographyTools.CreateRandomString(6, 10);
    }
}