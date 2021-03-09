using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneLibrary.Core.Base;
using VardoneLibrary.Core.Exceptions;

namespace VardoneLibrary.Core
{
    public class VardoneClient : BaseClient
    {
        public VardoneClient(long userId, string token) : base(userId, token) { }

        public User GetMe()
        {
            var response = ExecutePostWithToken("users/getMe");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetMe"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetMe"),
                _ => JsonConvert.DeserializeObject<User>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public User GetUser(long id)
        {
            var response = ExecutePostWithToken("users/getUser", null, new Dictionary<string, string> { { "secondId", id.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetUser"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetUser"),
                _ => JsonConvert.DeserializeObject<User>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public List<User> GetFriends()
        {
            var response = ExecutePostWithToken("users/getFriends");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetFriends"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetFriends"),
                _ => JsonConvert.DeserializeObject<List<User>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }
        
    }
}