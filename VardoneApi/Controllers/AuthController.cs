using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VardoneApi.Config;
using VardoneApi.Core;
using VardoneApi.Core.Checks;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Models.ApiModels;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers
{
    [ApiController, Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly Random Random = new();
        //
        [HttpPost, Route("authUser")]
        public async Task<IActionResult> AuthUser([FromBody] GetUserTokenApiModel loginRequestModel)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                if (loginRequestModel == null) return BadRequest("Empty model");

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                var tokens = dataContext.Tokens;
                tokens.Include(p => p.User).Load();
                var puss = dataContext.PrivateUserSalts;
                puss.Include(p => p.User).Load();
                UsersTable user;
                try
                {
                    user = users.First(usr => usr.Email == loginRequestModel.Email);
                    var pus = puss.First(p => p.User.Id == user.Id).Pus;
                    var passwordHash = CryptographyTools.GetPasswordHash(pus, loginRequestModel.PasswordHash);
                    if (user.PasswordHash != passwordHash) throw new Exception();
                }
                catch
                {
                    return BadRequest("Login failed");
                }

                var newToken = new TokensTable
                {
                    User = user,
                    CreatedAt = DateTime.Now,
                    MacAddress = loginRequestModel.MacAddress,
                    Token = GenerateToken()
                };

                try
                {
                    var remove = tokens.First(t =>
                        t.User.Email == loginRequestModel.Email && t.MacAddress == loginRequestModel.MacAddress);
                    tokens.RemoveRange(remove);
                }
                catch
                {
                    // ignored
                }

                tokens.Add(newToken);
                dataContext.SaveChanges();
                return Ok(GetJwtToken(GetIdentity(user.Id, newToken.Token)));
            }));
        }
        //
        [HttpPost, Route("checkUserToken"), Authorize]
        public async Task<IActionResult> CheckUserToken()
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                try
                {
                    var request = new UserTokenModel
                    {
                        UserId = Convert.ToInt64((User.Claims.First(p => p.Type == "id")).Value),
                        Token = User.Claims.First(p => p.Type == "token").Value
                    };
                    if (UserChecks.CheckToken(request)) return Ok(true);
                    return BadRequest();
                }
                catch
                {
                    return BadRequest("Incorrect token");
                }
            }));
        }
        //
        [HttpPost, Route("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel registerRequestModel)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                if (registerRequestModel == null) return BadRequest();

                if (!IsValidEmail(registerRequestModel.Email)) return BadRequest("Incorrect email");

                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    var puss = dataContext.PrivateUserSalts;

                    if (users.Any(p => p.Email == registerRequestModel.Email)) return BadRequest("<#!> Email is already booked");
                    if (users.Any(p => p.Username == registerRequestModel.Username)) return BadRequest("<#!> Username is already booked");


                    var user = new UsersTable
                    {
                        Username = registerRequestModel.Username,
                        Email = registerRequestModel.Email,
                        PasswordHash = "<@>"
                    };

                    users.Add(user);
                    dataContext.SaveChanges();
                    var pus = CryptographyTools.GetSha512Hash(
                        Encoding.ASCII.GetBytes(
                        Convert.ToBase64String(
                            Encoding.Default.GetBytes(user.Id + user.Email + user.Username))));
                    puss.Add(new PrivateUserSaltsTable
                    {
                        User = user,
                        Pus = pus
                    });
                    user.PasswordHash = CryptographyTools.GetPasswordHash(pus, registerRequestModel.PasswordHash);
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
        [HttpPost, Route("updateToken")]
        public async Task<IActionResult> UpdateToken([FromHeader] string token)
        {
            return await Task.Run(new Func<IActionResult>(() =>
            {
                UserTokenModel model;
                try
                {
                    var jwt = new JwtSecurityToken(token);
                    if (!CheckSignature(jwt)) return BadRequest("Invalid signature");
                    model = new UserTokenModel
                    {
                        UserId = Convert.ToInt64(jwt.Claims.First(p => p.Type == "id").Value),
                        Token = jwt.Claims.First(p => p.Type == "token").Value
                    };
                }
                catch
                {
                    return BadRequest("Incorrect token");
                }

                if (!UserChecks.CheckToken(model))
                {
                    Response.Headers.Add("Token-Invalid", "true");
                    return Unauthorized("Invalid token");
                }

                var dataContext = Program.DataContext;
                var tokens = dataContext.Tokens;
                tokens.Include(p => p.User).Load();
                return Ok(GetJwtToken(GetIdentity(model.UserId, model.Token)));
            }));
        }

        //Methods
        private static bool CheckSignature(JwtSecurityToken jwt)
        {
            var signature = JwtTokenUtilities.CreateEncodedSignature(jwt.EncodedHeader + "." + jwt.EncodedPayload,
                new SigningCredentials(TokenOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return signature == jwt.RawSignature;
        }
        private static ClaimsIdentity GetIdentity(long id, string token)
        {
            if (token is null) return null;
            var claims = new List<Claim>
            {
                new ("id", id.ToString()),
                new ("token", token)
            };

            return new ClaimsIdentity(claims);
        }
        private static string GetJwtToken(ClaimsIdentity ident)
        {
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                TokenOptions.ISSUER,
                TokenOptions.AUDIENCE,
                ident.Claims,
                now,
                now.Add(TimeSpan.FromMinutes(TokenOptions.LIFETIME)),
                new SigningCredentials(TokenOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));


            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        private static string GenerateToken() => CreateMd5((int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() + CreateRandomString());
        private static string CreateRandomString()
        {
            var sb = new StringBuilder();
            var n = Random.Next(1, 23);
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
        private static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var t in hashBytes) sb.Append(t.ToString("X2"));
            return sb.ToString();
        }
        private static bool IsValidEmail(string email) => new EmailAddressAttribute().IsValid(email);
    }
}