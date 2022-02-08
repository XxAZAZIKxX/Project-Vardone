using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Config;
using VardoneApi.Core;
using VardoneApi.Core.Checks;
using VardoneApi.Core.CreateHelpers;
using VardoneApi.Entity.Models.Channels;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneEntities.Models.TcpModels;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class UsersController : ControllerBase
    {
        [HttpPost, Route("addFriend")]
        public async Task<IActionResult> AddFriend([FromQuery] string secondUsername)
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

                if (!UserChecks.IsUserExists(secondUsername)) return BadRequest("Friend does not exist");
                if (UserChecks.IsFriends(userId, secondUsername)) return Ok("You are already friends");
                var dataContext = Program.DataContext;
                var friendsList = dataContext.FriendsList;
                friendsList.Include(p => p.FromUser).Load();
                friendsList.Include(p => p.ToUser).Load();
                var users = dataContext.Users;
                var user1 = users.First(p => p.Id == userId);
                var user2 = users.First(p => p.Username == secondUsername);
                if (user1.Id == user2.Id) return BadRequest("You can not add yourself as friend");
                try
                {
                    var friendTable = friendsList.First(p => p.FromUser == user1 && p.ToUser == user2 || p.FromUser == user2 && p.ToUser == user1);
                    if (friendTable.FromUser == user1) return Ok("Already sended friend request");
                    friendTable.Confirmed = true;
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(user1.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewFriend,
                            data = UserCreateHelper.GetUser(user2.Id)
                        });
                        Program.TcpServer.SendMessageTo(user2.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewFriend,
                            data = UserCreateHelper.GetUser(user1.Id)
                        });
                        Program.TcpServer.SendMessageTo(friendTable.FromUser.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.DeleteOutgoingFriendRequest,
                            data = UserCreateHelper.GetUser(friendTable.ToUser.Id)
                        });
                        Program.TcpServer.SendMessageTo(friendTable.ToUser.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.DeleteIncomingFriendRequest,
                            data = UserCreateHelper.GetUser(friendTable.FromUser.Id)
                        });
                    });
                    return Ok("Friend added");
                }
                catch
                {
                    // ignored
                }

                var newFl = new FriendsListTable { FromUser = user1, ToUser = user2, Confirmed = false };
                try
                {
                    friendsList.Add(newFl);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        Program.TcpServer.SendMessageTo(user1.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewOutgoingFriendRequest,
                            data = UserCreateHelper.GetUser(user2.Id)
                        });
                        Program.TcpServer.SendMessageTo(user2.Id, new TcpResponseModel
                        {
                            type = TypeTcpResponse.NewIncomingFriendRequest,
                            data = UserCreateHelper.GetUser(user1.Id)
                        });
                    });
                    return Ok("Friend request sended");
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }));
        }
        //
        [HttpPost, Route("checkFriend")]
        public async Task<IActionResult> CheckFriend([FromQuery] long secondId)
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
                if (userId == secondId) return BadRequest("Username equal second userId");

                if (!UserChecks.IsUserExists(secondId)) return BadRequest("Second userId does not exists");
                return Ok(!UserChecks.IsFriends(userId, secondId));
            }));
        }
        //
        [HttpPost, Route("deleteFriend")]
        public async Task<IActionResult> DeleteFriend([FromQuery] long secondId)
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
                if (userId == secondId) return BadRequest("Username equal friend userId");

                if (!UserChecks.IsUserExists(secondId)) return BadRequest("Friend does not exist");
                var dataContext = Program.DataContext;
                var friendsList = dataContext.FriendsList;
                friendsList.Include(p => p.FromUser).Include(p => p.ToUser).Load();
                var users = dataContext.Users;
                var user1 = users.First(p => p.Id == userId);
                var user2 = users.First(p => p.Id == secondId);
                try
                {
                    var first = friendsList.First(p => p.FromUser == user1 && p.ToUser == user2 || p.FromUser == user2 && p.ToUser == user1);
                    friendsList.Remove(first);
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        if (first.Confirmed)
                        {
                            var u1 = UserCreateHelper.GetUser(user1.Id, true);
                            var u2 = UserCreateHelper.GetUser(user2.Id, true);
                            Program.TcpServer.SendMessageTo(u1.UserId, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteFriend,
                                data = u2
                            });
                            Program.TcpServer.SendMessageTo(u2.UserId, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteFriend,
                                data = u1
                            });
                        }
                        else
                        {
                            var from = UserCreateHelper.GetUser(first.FromUser.Id);
                            var to = UserCreateHelper.GetUser(first.ToUser.Id);
                            Program.TcpServer.SendMessageTo(from.UserId, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteOutgoingFriendRequest,
                                data = to
                            });
                            Program.TcpServer.SendMessageTo(to.UserId, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteIncomingFriendRequest,
                                data = from
                            });
                        }
                    });
                    return Ok("Deleted");
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }));
        }
        //
        [HttpPost, Route("getFriends")]
        public async Task<IActionResult> GetFriends()
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
                    var friendsList = dataContext.FriendsList;
                    friendsList.Include(p => p.FromUser).Load();
                    friendsList.Include(p => p.ToUser).Load();
                    friendsList.Include(p => p.FromUser.Info).Load();
                    friendsList.Include(p => p.ToUser.Info).Load();
                    var users = new List<User>();
                    foreach (var row in friendsList.Where(p => (p.FromUser.Id == userId || p.ToUser.Id == userId) && p.Confirmed))
                    {
                        var friend = row.FromUser.Id != userId ? row.FromUser : row.ToUser;
                        users.Add(UserCreateHelper.GetUser(friend.Id));
                    }

                    return Ok(users.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getIncomingFriendRequests")]
        public async Task<IActionResult> GetIncomingFriendRequests()
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
                    var friendsList = dataContext.FriendsList;
                    friendsList.Include(p => p.FromUser).Load();
                    friendsList.Include(p => p.ToUser).Load();
                    friendsList.Include(p => p.FromUser.Info).Load();
                    var users = new List<User>();
                    foreach (var row in friendsList.Where(p => p.ToUser.Id == userId && p.Confirmed == false))
                    {
                        users.Add(UserCreateHelper.GetUser(row.FromUser.Id));
                    }

                    return Ok(users.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getOutgoingFriendRequests")]
        public async Task<IActionResult> GetOutgoingFriendRequests()
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
                    var friendsList = dataContext.FriendsList;
                    friendsList.Include(p => p.FromUser).Load();
                    friendsList.Include(p => p.ToUser).Load();
                    friendsList.Include(p => p.ToUser.Info).Load();
                    var users = new List<User>();
                    foreach (var row in friendsList.Where(p => p.FromUser.Id == userId && p.Confirmed == false))
                    {
                        users.Add(UserCreateHelper.GetUser(row.ToUser.Id));
                    }

                    return Ok(users.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getMe")]
        public async Task<IActionResult> GetMe()
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
                    var users = dataContext.Users;
                    dataContext.Users.Include(p => p.Info).Load();
                    var puss = dataContext.PrivateUserSalts;
                    puss.Include(p => p.User).Load();
                    var user = users.First(p => p.Id == userId);
                    var pus = puss.First(p => p.User.Id == user.Id).Pus;

                    var returnUser = new User
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Description = user.Info?.Description,
                        Base64Avatar = user.Info?.Avatar == null ? null : Convert.ToBase64String(user.Info.Avatar)
                    };

                    var byteKey = CryptographyTools.GetByteKey(pus + PasswordOptions.KEY, PasswordOptions.KEY);

                    var fullName = user.Info?.FullName is null
                        ? null
                        : CryptographyTools.DecryptStringFromBytes_Aes(Convert.FromBase64String(user.Info.FullName), byteKey, PasswordOptions.IV);

                    var phone = user.Info?.Phone is null
                        ? null
                        : CryptographyTools.DecryptStringFromBytes_Aes(Convert.FromBase64String(user.Info?.Phone), byteKey, PasswordOptions.IV);

                    DateTime? birthDate = user.Info?.BirthDate is null
                        ? null
                        : DateTime.FromBinary(Convert.ToInt64(
                            CryptographyTools.DecryptStringFromBytes_Aes(Convert.FromBase64String(user.Info?.BirthDate),
                                byteKey, PasswordOptions.IV)));
                    returnUser.AdditionalInformation = new AdditionalUserInformation
                    {
                        Email = user.Email,
                        FullName = fullName,
                        Phone = phone,
                        BirthDate = birthDate
                    };

                    return Ok(returnUser);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getUser")]
        public async Task<IActionResult> GetUser([FromQuery] long secondId)
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

                if (!UserChecks.IsUserExists(secondId)) return BadRequest("User does not exist");
                if (!UserChecks.CanGetUser(userId, secondId)) return BadRequest("You not allowed to do this");
                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    users.Include(p => p.Info).Load();
                    var user = users.First(p => p.Id == secondId);
                    return Ok(UserCreateHelper.GetUser(user.Id));
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getUserOnline")]
        public async Task<IActionResult> getUserOnline([FromQuery] long secondId)
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

                if (!UserChecks.IsUserExists(secondId)) return BadRequest("BannedUser is not exists");
                if (!UserChecks.CanGetUser(userId, secondId)) return BadRequest("No access");
                try
                {
                    var dataContext = Program.DataContext;
                    var usersOnline = dataContext.UsersOnline;
                    usersOnline.Include(p => p.User).Load();

                    return Ok(usersOnline.FirstOrDefault(p => p.User.Id == secondId)?.IsOnline);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getGuilds")]
        public async Task<IActionResult> GetGuilds([FromHeader] bool onlyId = false)
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
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.User).Load();
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.Guild.Info).Load();
                    guildMembers.Include(p => p.Guild.Owner).Load();
                    guildMembers.Include(p => p.Guild.Owner.Info).Load();
                    var guilds = new List<Guild>();
                    foreach (var item in guildMembers.Where(p => p.User.Id == userId))
                    {
                        guilds.Add(GuildCreateHelper.GetGuild(item.Guild.Id, true, true, onlyId: onlyId));
                    }

                    return Ok(guilds.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getPrivateChats")]
        public async Task<IActionResult> GetPrivateChats([FromHeader] bool onlyId = false)
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
                    var chatsTable = dataContext.PrivateChats;
                    var privateMessages = dataContext.PrivateMessages;
                    privateMessages.Include(p => p.Author).Load();
                    chatsTable.Include(p => p.FromUser).Load();
                    chatsTable.Include(p => p.FromUser.Info).Load();
                    chatsTable.Include(p => p.ToUser).Load();
                    chatsTable.Include(p => p.ToUser.Info).Load();
                    var chats = new List<PrivateChat>();
                    foreach (var chat in chatsTable.Where(p => p.FromUser.Id == userId || p.ToUser.Id == userId))
                    {
                        var item = PrivateChatCreateHelper.GetPrivateChat(chat.Id, userId);
                        chats.Add(item);
                    }
                    return Ok(chats.ToArray());
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("closeAllSessions")]
        public async Task<IActionResult> CloseAllSessions()
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

                var dataContext = Program.DataContext;
                var tokens = dataContext.Tokens;
                tokens.Include(p => p.User).Load();
                try
                {
                    var @where = tokens.Where(p => p.User.Id == userId);
                    tokens.RemoveRange(@where);
                    dataContext.SaveChanges();
                    Program.TcpServer.RemoveConnection(userId);
                    return Ok("Closed");
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }));
        }
        //
        [HttpPost, Route("closeCurrentSession")]
        public async Task<IActionResult> CloseCurrentSession()
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

                var dataContext = Program.DataContext;
                var tokens = dataContext.Tokens;
                tokens.Include(p => p.User).Load();
                try
                {
                    var first = tokens.First(p => p.User.Id == userId && p.Token == token.Token);
                    tokens.Remove(first);
                    dataContext.SaveChanges();
                    Program.TcpServer.RemoveConnection(token);
                    return Ok("Closed");
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }));
        }
        //
        [HttpPost, Route("deleteMe")]
        public async Task<IActionResult> DeleteMe()
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

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                var members = dataContext.GuildMembers;
                members.Include(p => p.Guild).Load();
                members.Include(p => p.Guild.Owner).Load();
                members.Include(p => p.User).Load();
                var privateChats = dataContext.PrivateChats;
                privateChats.Include(p => p.FromUser).Load();
                privateChats.Include(p => p.ToUser).Load();
                var invites = dataContext.GuildInvites;
                invites.Include(p => p.Guild).Load();
                invites.Include(p => p.CreatedByUser).Load();
                var friends = dataContext.FriendsList;
                friends.Include(p => p.FromUser).Load();
                friends.Include(p => p.ToUser).Load();
                var bannedMembers = dataContext.BannedGuildMembers;
                bannedMembers.Include(p => p.Guild).Load();
                bannedMembers.Include(p => p.Guild.Owner).Load();
                bannedMembers.Include(p => p.BannedUser).Load();
                var channelMessages = dataContext.ChannelMessages;
                channelMessages.Include(p => p.Author).Load();
                channelMessages.Include(p => p.Channel).Load();
                channelMessages.Include(p => p.Channel.Guild).Load();

                try
                {
                    var user = users.First(p => p.Id == userId);
                    var userOb = UserCreateHelper.GetUser(user.Id, true);

                    var userFriendList = friends.Where(p => p.FromUser.Id == userId || p.ToUser.Id == userId).ToArray();
                    var userIncomingFriendRequsts = userFriendList.Where(p => p.ToUser.Id == userId && !p.Confirmed).ToArray();
                    var userOutgoingFriendRequsts = userFriendList.Where(p => p.FromUser.Id == userId && !p.Confirmed).ToArray();
                    var userFriends = userFriendList.Where(p => p.Confirmed).ToArray();

                    var userPrivateChats = privateChats.Where(p => p.FromUser.Id == userId || p.ToUser.Id == userId).Select(p => PrivateChatCreateHelper.GetPrivateChat(p.Id, userId)).ToArray();

                    var userGuilds = members.Where(p => p.User.Id == userId);
                    var userGuildObs = userGuilds.ToDictionary(p => p.Guild.Id, p => GuildCreateHelper.GetGuild(p.Guild.Id, false, false, true));
                    var userMemberObs = userGuilds.ToDictionary(p => p.Guild.Id, p => UserCreateHelper.GetMember(p.Id, true));
                    var userOwnedGuildMemberIds = members
                        .Where(p => p.Guild.Owner.Id == userId && userGuilds.Any(c => c.Guild.Id == p.Guild.Id))
                        .ToLookup(p => p.Guild.Id)
                        .ToDictionary(p => p.Key, p => p.Select(c => c.User.Id).ToArray());
                    var userUnownedGuildMemberIds = members
                        .Where(p => p.Guild.Owner.Id != userId && userGuilds.Any(c => c.Guild.Id == p.Guild.Id))
                        .ToLookup(p => p.Guild.Id)
                        .ToDictionary(p => p.Key, p => p.Select(c => c.User.Id).ToArray());

                    var userBannedOnGuilds = bannedMembers.Where(p => p.BannedUser.Id == userId).ToArray();

                    var userChannelMessages = channelMessages.AsEnumerable().Where(p => p.Author.Id == userId && userUnownedGuildMemberIds.ContainsKey(p.Channel.Guild.Id)).ToList();
                    var userChannelMessageObs = userChannelMessages.ToDictionary(p => p.Id, p => MessageCreateHelper.GetChannelMessage(p.Id, true));

                    users.Remove(user);
                    dataContext.SaveChanges();
                    Program.TcpServer.RemoveConnection(userId);
                    Task.Run(() =>
                    {
                        foreach (var incomingFriendRequst in userIncomingFriendRequsts)
                        {
                            Program.TcpServer.SendMessageTo(incomingFriendRequst.FromUser.Id, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteOutgoingFriendRequest,
                                data = userOb
                            });
                        }
                        foreach (var outgoingFriendRequst in userOutgoingFriendRequsts)
                        {
                            Program.TcpServer.SendMessageTo(outgoingFriendRequst.ToUser.Id, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteIncomingFriendRequest,
                                data = userOb
                            });
                        }
                        foreach (var friend in userFriends)
                        {
                            var uid = friend.FromUser.Id == userId ? friend.ToUser.Id : friend.FromUser.Id;
                            Program.TcpServer.SendMessageTo(uid, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteFriend,
                                data = userOb
                            });
                        }

                        foreach (var chat in userPrivateChats)
                        {
                            Program.TcpServer.SendMessageTo(chat.ToUser.UserId, new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeletePrivateChat,
                                data = chat
                            });
                        }

                        foreach (var (guildId, membersIds) in userUnownedGuildMemberIds)
                        {
                            var tcpNotice = new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteMember,
                                data = userMemberObs[guildId]
                            };
                            foreach (var id in membersIds)
                            {
                                Program.TcpServer.SendMessageTo(id, tcpNotice);
                            }
                        }

                        foreach (var messageItem in userChannelMessages)
                        {
                            var tcpNotice = new TcpResponseModel
                            {
                                type = TypeTcpResponse.DeleteChannelMessage,
                                data = userChannelMessageObs[messageItem.Id]
                            };
                            foreach (var id in userUnownedGuildMemberIds[messageItem.Channel.Guild.Id])
                            {
                                Program.TcpServer.SendMessageTo(id, tcpNotice);
                            }
                        }

                        foreach (var (guildId, memberIds) in userOwnedGuildMemberIds)
                        {
                            var tcpNotice = new TcpResponseModel
                            {
                                type = TypeTcpResponse.GuildLeave,
                                data = userGuildObs[guildId]
                            };
                            foreach (var id in memberIds)
                            {
                                Program.TcpServer.SendMessageTo(id, tcpNotice);
                            }
                        }

                        foreach (var banItem in userBannedOnGuilds)
                        {
                            Program.TcpServer.SendMessageTo(banItem.Guild.Owner.Id, new TcpResponseModel
                            {
                                type = TypeTcpResponse.UnbanMember,
                                data = userOb
                            });
                        }
                    });
                    return Ok("Deleted successfully");
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordModel updatePasswordModel)
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

                var dataContext = Program.DataContext;
                try
                {
                    var users = dataContext.Users;
                    var puss = dataContext.PrivateUserSalts;
                    puss.Include(p => p.User).Load();
                    var user = users.First(p => p.Id == userId);

                    var pus = puss.First(p => p.User.Id == user.Id).Pus;
                    var prevPasswordHash = CryptographyTools.GetPasswordHash(pus, updatePasswordModel.PreviousPasswordHash);

                    if (prevPasswordHash != user.PasswordHash) return BadRequest("Incorrect previous password");

                    if (string.IsNullOrWhiteSpace(updatePasswordModel.NewPasswordHash)) return BadRequest("Incorrect new password");
                    var newPasswordHash = CryptographyTools.GetPasswordHash(pus, updatePasswordModel.NewPasswordHash);

                    user.PasswordHash = newPasswordHash;
                    users.Update(user);
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }));
        }
        //
        [HttpPost, Route("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel updateUserModel)
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

                if (updateUserModel.Email is not null && !UserChecks.IsEmailAvailable(updateUserModel.Email)) return BadRequest("Email is booked");

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                users.Include(p => p.Info).Load();
                var usersInfos = dataContext.UserInfos;
                usersInfos.Include(p => p.User).Load();
                var puss = dataContext.PrivateUserSalts;
                puss.Include(p => p.User).Load();

                var user = users.First(p => p.Id == userId);
                var userInfo = user.Info ?? new UserInfosTable();
                userInfo.User = user;

                if (updateUserModel.Username is not null) user.Username = updateUserModel.Username;
                if (updateUserModel.Description is not null) userInfo.Description = string.IsNullOrEmpty(updateUserModel.Description) ? null : updateUserModel.Description;
                if (updateUserModel.Email is not null) user.Email = updateUserModel.Email;
                if (updateUserModel.Base64Image is not null)
                {
                    if (string.IsNullOrEmpty(updateUserModel.Base64Image)) userInfo.Avatar = null;
                    else
                    {
                        var res = Convert.TryFromBase64String(updateUserModel.Base64Image, new Span<byte>(new byte[updateUserModel.Base64Image.Length]), out _);
                        if (res)
                        {
                            var bytes = Convert.FromBase64String(updateUserModel.Base64Image);
                            if (ImageWorker.IsImage(bytes)) userInfo.Avatar = ImageWorker.CompressImageQualityLevel(bytes, 50);
                        }
                    }
                }

                var pus = puss.First(p => p.User.Id == user.Id).Pus;

                var byteKey = CryptographyTools.GetByteKey(pus + PasswordOptions.KEY, PasswordOptions.KEY);

                if (updateUserModel.Phone is not null)
                    userInfo.Phone = Convert.ToBase64String(
                        CryptographyTools.EncryptStringToBytes_Aes(updateUserModel.Phone, byteKey, PasswordOptions.IV));

                if (updateUserModel.BirthDate is not null)
                    userInfo.BirthDate = Convert.ToBase64String(
                        CryptographyTools.EncryptStringToBytes_Aes(
                            updateUserModel.BirthDate.Value.ToBinary().ToString(), byteKey, PasswordOptions.IV));

                if (updateUserModel.FullName is not null)
                {
                    userInfo.FullName = Convert.ToBase64String(CryptographyTools.EncryptStringToBytes_Aes(updateUserModel.FullName, byteKey, PasswordOptions.IV));
                }

                try
                {
                    usersInfos.Update(userInfo);
                    dataContext.SaveChanges();
                    user.Info = userInfo;
                    users.Update(user);
                    dataContext.SaveChanges();
                    usersInfos.RemoveRange(usersInfos.Where(p => p.User.Id == user.Id && p.Id != user.Info.Id));
                    dataContext.SaveChanges();
                    Task.Run(() =>
                    {
                        var friendsList = dataContext.FriendsList;
                        friendsList.Include(p => p.FromUser).Load();
                        friendsList.Include(p => p.ToUser).Load();
                        var guildMembers = dataContext.GuildMembers;
                        guildMembers.Include(p => p.Guild).Load();
                        guildMembers.Include(p => p.User).Load();
                        var tcpNotify = new TcpResponseModel
                        {
                            type = TypeTcpResponse.UpdateUser,
                            data = UserCreateHelper.GetUser(userId)
                        };
                        Program.TcpServer.SendMessageTo(userId, tcpNotify);
                        var guildIds = guildMembers.Where(p => p.User.Id == userId).Select(p => p.Guild.Id).ToArray();
                        var userIds = guildMembers.Where(p => guildIds.Contains(p.Guild.Id)).Select(p => p.User.Id).ToArray();
                        foreach (var id in userIds)
                        {
                            Program.TcpServer.SendMessageTo(id, tcpNotify);
                        }
                        foreach (var friendsListTable in friendsList.Where(p => p.FromUser.Id == userId || p.ToUser.Id == userId))
                        {
                            var u = friendsListTable.FromUser.Id == userId ? friendsListTable.ToUser.Id : friendsListTable.FromUser.Id;
                            if (userIds.Contains(u)) continue;
                            Program.TcpServer.SendMessageTo(u, tcpNotify);
                        }
                    });
                    return Ok("Updated");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }));
        }
    }
}