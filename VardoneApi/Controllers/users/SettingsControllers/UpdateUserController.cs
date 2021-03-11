using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.SettingsControllers
{
    [ApiController, Route("users/[controller]")]
    public class UpdateUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token,
            [FromBody] UpdateUserModel updateUserModel)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (updateUserModel == null) return BadRequest("Empty user model");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (updateUserModel.Email is not null && !Core.UserChecks.IsEmailAvailable(updateUserModel.Email))
                    return BadRequest("Email is booked");

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                users.Include(p => p.Info).Load();
                var usersInfos = dataContext.UserInfos;
                usersInfos.Include(p => p.User).Load();

                var user = users.First(p => p.Id == userId);
                var userInfo = user.Info ?? new UserInfosTable();

                userInfo.User = user;

                if (updateUserModel.Username is not null) user.Username = updateUserModel.Username;
                if (updateUserModel.Password is not null) user.Password = updateUserModel.Password;
                if (updateUserModel.Description is not null)
                    userInfo.Description = string.IsNullOrEmpty(updateUserModel.Description)
                        ? null
                        : updateUserModel.Description;

                if (updateUserModel.Email is not null) user.Email = updateUserModel.Email;
                if (updateUserModel.Base64Image is not null)
                {
                    if (string.IsNullOrEmpty(updateUserModel.Base64Image)) userInfo.Avatar = null;
                    else
                    {
                        var res = Convert.TryFromBase64String(updateUserModel.Base64Image,
                            new Span<byte>(new byte[updateUserModel.Base64Image.Length]), out _);
                        if (res) userInfo.Avatar = Convert.FromBase64String(updateUserModel.Base64Image);
                    }
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
            })).GetAwaiter().GetResult();
        }
    }
}