using System;
using System.Linq;
using System.Security.Claims;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Core
{
    public abstract class TokenParserWorker
    {
        public static UserTokenModel GetUserToken(ClaimsPrincipal user)
        {
            try
            {
                var userId = Convert.ToInt64(user.Claims.First(p => p.Type == "id").Value);
                var token = user.Claims.First(p => p.Type == "token").Value;
                return new UserTokenModel { UserId = userId, Token = token };
            }
            catch
            {
                return null;
            }
        }
    }
}