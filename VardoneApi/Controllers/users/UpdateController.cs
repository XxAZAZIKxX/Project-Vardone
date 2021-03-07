using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users
{
    [ApiController, Route("users/[controller]")]
    public class UpdateController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token,
            [FromBody] UpdateUserModel updateUserModel)
        {
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized("Empty username");
            if (string.IsNullOrWhiteSpace(token)) return Unauthorized("Empty token");
            if (updateUserModel == null) return BadRequest("Empty user model");
            if (!Core.UserChecks.CheckToken(new TokenUserModel {Username = username, Token = token}))
                return Unauthorized("Invalid token");

            var users = Program.DataContext.Users;
            users.Include(p=>p.Info).Load();
            var usersInfos = Program.DataContext.UserInfos;
            usersInfos.Include(p=>p.User).Load();

            var user = users.First(p => p.Username == username);
            var userInfo = user.Info ?? new UserInfos();
            
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
                    if (res) userInfo.Avatar = updateUserModel.GetImageBytes();
                }
            }

            try
            {
                usersInfos.Update(userInfo);
                Program.DataContext.SaveChanges();
                user.Info = userInfo;
                users.Update(user);
                Program.DataContext.SaveChanges();
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
                    Program.DataContext.SaveChanges();
                }

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}