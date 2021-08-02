using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VardoneEntities.Models.ClientModels;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneLibrary.Core.Base
{
    public abstract class VardoneBaseApi
    {
        protected static readonly RestClient RestClient = new("https://localhost:5001/") { Timeout = -1 };

        private static IRestResponse ExecutePost(string resource, string json = null, Dictionary<string, string> queryParameters = null)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (json != null) request.AddParameter("application/json", json, ParameterType.RequestBody);

            if (queryParameters == null) return RestClient.Execute(request);

            foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);

            return RestClient.Execute(request);
        }

        public static UserTokenModel GetUserToken(string email, string password)
        {
            var response = ExecutePost(@"/users/authUser",
                JsonConvert.SerializeObject(new GetUserTokenClientModel { Email = email, Password = password }));
            return response.StatusCode == HttpStatusCode.BadRequest ? null : JsonConvert.DeserializeObject<UserTokenModel>(response.Content);
        }

        public static bool RegisterUser(RegisterUserModel register)
        {
            var response = ExecutePost(@"/users/registerUser", JsonConvert.SerializeObject(register));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.Content);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool CheckToken(long userId, string token)
        {
            var response = ExecutePost(@"/users/checkUserToken", JsonConvert.SerializeObject(new UserTokenModel { UserId = userId, Token = token }));
            return response.StatusCode != HttpStatusCode.BadRequest && JsonConvert.DeserializeObject<bool>(response.Content);
        }
    }
}