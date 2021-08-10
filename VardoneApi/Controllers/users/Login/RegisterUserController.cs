﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Entity.Models;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.Login
{
    [ApiController, Route("users/[controller]")]
    public class RegisterUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] RegisterUserModel registerRequestModel)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (registerRequestModel == null) return BadRequest();

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                try
                {
                    var _ = users.First(u => u.Email == registerRequestModel.Email);
                    return BadRequest("Email is already booked");
                }
                catch
                {
                    // ignored
                }
                try
                {
                    var _ = users.First(u => u.Username == registerRequestModel.Username);
                    return BadRequest("Username is already booked");
                }
                catch
                {
                    // ignored
                }

                var user = new UsersTable
                {
                    Username = registerRequestModel.Username,
                    Email = registerRequestModel.Email,
                    Password = registerRequestModel.Password
                };

                users.Add(user);

                try
                {
                    dataContext.SaveChanges();
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