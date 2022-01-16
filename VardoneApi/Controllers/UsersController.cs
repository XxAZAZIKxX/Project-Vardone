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
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;
using VardoneEntities.Models.GeneralModels.Users;

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
                var token = TokenParserWorker.GetUserToken(User);
                if (token is null) return BadRequest("Token parser problem");
                var userId = token.UserId;
                if (!UserChecks.CheckToken(token))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                if (!UserChecks.IsUserExists(secondUsername)) return BadRequest("Friend does not exist");
                if (UserChecks.IsFriends(userId, secondUsername)) return Ok();
                var dataContext = Program.DataContext;
                var friendsList = dataContext.FriendsList;
                friendsList.Include(p => p.FromUser).Load();
                friendsList.Include(p => p.ToUser).Load();
                var users = dataContext.Users;
                var user1 = users.First(p => p.Id == userId);
                var user2 = users.First(p => p.Username == secondUsername);
                if (user1.Id == user2.Id) return BadRequest();
                try
                {
                    var list = friendsList.First(p =>
                        p.FromUser == user1 && p.ToUser == user2 || p.FromUser == user2 && p.ToUser == user1);
                    if (list.FromUser == user1) return Ok();
                    list.Confirmed = true;
                    dataContext.SaveChanges();
                    return Ok();
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
                    return Ok();
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
                var token = TokenParserWorker.GetUserToken(User);
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
                var token = TokenParserWorker.GetUserToken(User);
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
                    var first = friendsList.First(p =>
                        p.FromUser == user1 && p.ToUser == user2 || p.FromUser == user2 && p.ToUser == user1);
                    friendsList.Remove(first);
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
        [HttpPost, Route("getFriends")]
        public async Task<IActionResult> GetFriends([FromHeader] bool onlyId = false)
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
                    var friendsList = dataContext.FriendsList;
                    friendsList.Include(p => p.FromUser).Load();
                    friendsList.Include(p => p.ToUser).Load();
                    friendsList.Include(p => p.FromUser.Info).Load();
                    friendsList.Include(p => p.ToUser.Info).Load();
                    var users = new List<User>();
                    foreach (var row in friendsList.Where(p => (p.FromUser.Id == userId || p.ToUser.Id == userId) && p.Confirmed))
                    {
                        var friend = row.FromUser.Id != userId ? row.FromUser : row.ToUser;
                        users.Add(UserCreateHelper.GetUser(friend, onlyId));
                    }

                    return Ok(users);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getIncomingFriendRequests")]
        public async Task<IActionResult> GetIncomingFriendRequests([FromHeader] bool onlyId = false)
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
                    var friendsList = dataContext.FriendsList;
                    friendsList.Include(p => p.FromUser).Load();
                    friendsList.Include(p => p.ToUser).Load();
                    friendsList.Include(p => p.FromUser.Info).Load();
                    var users = new List<User>();
                    foreach (var row in friendsList.Where(p => p.ToUser.Id == userId && p.Confirmed == false))
                    {
                        users.Add(UserCreateHelper.GetUser(row.FromUser, onlyId));
                    }


                    return Ok(users);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getOutgoingFriendRequests")]
        public async Task<IActionResult> GetOutgoingFriendRequests([FromHeader] bool onlyId = false)
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
                    var friendsList = dataContext.FriendsList;
                    friendsList.Include(p => p.FromUser).Load();
                    friendsList.Include(p => p.ToUser).Load();
                    friendsList.Include(p => p.ToUser.Info).Load();
                    var users = new List<User>();
                    foreach (var row in friendsList.Where(p => p.FromUser.Id == userId && p.Confirmed == false))
                    {
                        users.Add(UserCreateHelper.GetUser(row.ToUser, onlyId));
                    }

                    return Ok(users);
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("getMe")]
        public async Task<IActionResult> GetMe([FromHeader] bool onlyId = false)
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
                    var users = dataContext.Users;
                    dataContext.Users.Include(p => p.Info).Load();
                    var puss = dataContext.PrivateUserSalts;
                    puss.Include(p => p.User).Load();
                    var user = users.First(p => p.Id == userId);
                    var pus = puss.First(p => p.User.Id == user.Id).Pus;

                    var returnUser = new User
                    {
                        UserId = user.Id,
                        Username = user.Username
                    };
                    if (!onlyId)
                    {
                        var byteKey = CryptographyTools.GetByteKey(pus + PasswordOptions.KEY, PasswordOptions.KEY);

                        var fullName = user.Info?.FullName is null
                            ? null
                            : CryptographyTools.DecryptStringFromBytes_Aes(Convert.FromBase64String(user.Info.FullName), byteKey, PasswordOptions.IV);

                        var phone = user.Info?.Phone is null
                            ? null
                            : CryptographyTools.DecryptStringFromBytes_Aes(Convert.FromBase64String(user.Info?.Phone), byteKey, PasswordOptions.IV);

                        DateTime? birthDate = user.Info?.BirthDate is null
                            ? null
                            : DateTime.FromBinary(Convert.ToInt64(CryptographyTools.DecryptStringFromBytes_Aes(
                                Convert.FromBase64String(user.Info?.BirthDate),
                                byteKey,
                                PasswordOptions.IV)));
                        returnUser.Description = user.Info?.Description;
                        returnUser.Base64Avatar = user.Info?.Avatar == null ? null : Convert.ToBase64String(user.Info.Avatar);
                        returnUser.AdditionalInformation = new AdditionalUserInformation
                        {
                            Email = user.Email,
                            FullName = fullName,
                            Phone = phone,
                            BirthDate = birthDate
                        };
                    }

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
                var token = TokenParserWorker.GetUserToken(User);
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
                    return Ok(UserCreateHelper.GetUser(user));
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
                var token = TokenParserWorker.GetUserToken(User);
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
                    try
                    {
                        var user = usersOnline.First(p => p.User.Id == secondId);
                        var span = TimeSpan.FromTicks(DateTime.Now.Ticks) - TimeSpan.FromTicks(user.LastOnlineTime.Ticks);
                        var res = span.Minutes < 1;
                        return Ok(res);
                    }
                    catch
                    {
                        return Ok(false);
                    }
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
                    var guildMembers = dataContext.GuildMembers;
                    guildMembers.Include(p => p.User).Load();
                    guildMembers.Include(p => p.Guild).Load();
                    guildMembers.Include(p => p.Guild.Info).Load();
                    guildMembers.Include(p => p.Guild.Owner).Load();
                    guildMembers.Include(p => p.Guild.Owner.Info).Load();
                    var guilds = new List<Guild>();
                    foreach (var item in guildMembers.Where(p => p.User.Id == userId))
                    {
                        guilds.Add(GuildCreateHelper.GetGuild(item.Guild, true, true, onlyId: onlyId));
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
                    var chatsTable = dataContext.PrivateChats;
                    var privateMessages = dataContext.PrivateMessages;
                    privateMessages.Include(p => p.Author).Load();
                    chatsTable.Include(p => p.FromUser).Load();
                    chatsTable.Include(p => p.FromUser.Info).Load();
                    chatsTable.Include(p => p.ToUser).Load();
                    chatsTable.Include(p => p.ToUser.Info).Load();
                    var chats = new List<PrivateChat>();
                    var @where = chatsTable.Where(p => p.FromUser.Id == userId || p.ToUser.Id == userId).ToList();
                    foreach (var chat in @where)
                    {
                        var user1 = chat.FromUser.Id == userId ? chat.FromUser : chat.ToUser;
                        var user2 = chat.FromUser.Id != userId ? chat.FromUser : chat.ToUser;
                        var lastReadTime = user1 == chat.FromUser ? chat.FromLastReadTimeMessages : chat.ToLastReadTimeMessages;
                        var item = new PrivateChat
                        {
                            ChatId = chat.Id,
                            FromUser = UserCreateHelper.GetUser(user1, onlyId),
                            ToUser = UserCreateHelper.GetUser(user2, onlyId),
                            UnreadMessages = privateMessages.Count(p =>
                                p.Chat.Id == chat.Id && p.Author != user1 &&
                                DateTime.Compare(p.CreatedTime, lastReadTime) > 0)
                        };
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
                var token = TokenParserWorker.GetUserToken(User);
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
                    return Ok();
                }
                catch
                {
                    return BadRequest();
                }
            }));
        }
        //
        [HttpPost, Route("closeCurrentSession")]
        public async Task<IActionResult> CloseCurrentSession()
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

                var dataContext = Program.DataContext;
                var tokens = dataContext.Tokens;
                tokens.Include(p => p.User).Load();
                try
                {
                    var first = tokens.First(p => p.User.Id == userId && p.Token == token.Token);
                    tokens.Remove(first);
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch
                {
                    return BadRequest();
                }
            }));
        }
        //
        [HttpPost, Route("deleteMe")]
        public async Task<IActionResult> DeleteMe()
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

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                try
                {
                    users.Remove(users.First(p => p.Id == userId));
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }));
        }
        //
        [HttpPost, Route("setOnline")]
        public async Task<IActionResult> setOnline()
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

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                var usersOnline = dataContext.UsersOnline;
                usersOnline.Include(p => p.User).Load();
                try
                {
                    var user = usersOnline.First(p => p.User.Id == userId);
                    user.LastOnlineTime = DateTime.Now;
                    usersOnline.Update(user);
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch
                {
                    try
                    {
                        usersOnline.Add(new UsersOnlineTable
                        {
                            User = users.First(p => p.Id == userId),
                            LastOnlineTime = DateTime.Now
                        });
                        dataContext.SaveChanges();
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e);
                    }
                }
            }));
        }
        //
        [HttpPost, Route("updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordModel updatePasswordModel)
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
                var token = TokenParserWorker.GetUserToken(User);
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
                    try
                    {
                        var where = usersInfos.Where(p => p.User.Id == user.Id && p.Id != user.Info.Id);
                        if (where.ToArray().Length == 0) throw new Exception();
                        usersInfos.RemoveRange(where);
                    }
                    catch
                    {
                        // ignored
                    }
                    finally
                    {
                        dataContext.SaveChanges();
                    }

                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }));
        }
    }
}