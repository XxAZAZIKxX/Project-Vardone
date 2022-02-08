using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VardoneApi.Config;
using VardoneApi.Core.Checks;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Core
{
    internal static class JwtTokenWorker
    {
        public static UserTokenModel GetUserToken(ClaimsPrincipal user) => GetUserToken(user.Claims);
        public static UserTokenModel GetUserToken(IEnumerable<Claim> claims)
        {
            claims = claims.ToArray();
            if (!claims.Any()) return null;
            try
            {
                var userId = Convert.ToInt64(claims.First(p => p.Type == "id").Value);
                var token = claims.First(p => p.Type == "token").Value;
                return new UserTokenModel { UserId = userId, Token = token };
            }
            catch
            {
                return null;
            }
        }

        public static bool CheckJwtToken(string jwtToken, out JwtSecurityToken jwt)
        {
            try
            {
                jwt = new JwtSecurityToken(jwtToken);
            }
            catch
            {
                jwt = null;
                return false;
            }
            if (!CheckSignature(jwt)) return false;
            if (jwt.ValidTo.CompareTo(DateTime.UtcNow) < 0) return false;
            var token = GetUserToken(jwt.Claims);
            return UserChecks.CheckToken(token);
        }

        public static bool CheckSignature(JwtSecurityToken jwt)
        {
            var signature = JwtTokenUtilities.CreateEncodedSignature(jwt.EncodedHeader + "." + jwt.EncodedPayload,
                new SigningCredentials(TokenOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return signature == jwt.RawSignature;
        }

        public static ClaimsIdentity GetIdentity(long id, string token)
        {
            if (token is null) return null;
            var claims = new List<Claim>
            {
                new ("id", id.ToString()),
                new ("token", token)
            };

            return new ClaimsIdentity(claims);
        }

        public static string GetJwtToken(ClaimsIdentity ident)
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
    }
}