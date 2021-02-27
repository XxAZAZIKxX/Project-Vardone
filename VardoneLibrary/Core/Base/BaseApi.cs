using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VardoneLibrary.Models;

namespace VardoneLibrary.Core.Base
{
    public abstract class BaseApi
    {
        protected static readonly RestClient REST_CLIENT = new("https://localhost:5001/") {Timeout = -1};

        protected static IRestResponse ExecutePost(string resource, string json)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            return REST_CLIENT.Execute(request);
        }

        public static string GetUserToken(string username, string password)
        {
            var response = ExecutePost(@"/users/auth",
                JsonConvert.SerializeObject(new GetTokenModel {Username = username, Password = password}));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return JsonConvert.DeserializeObject<TokenModel>(response.Content)?.Token;
        }

        public static bool RegisterUser(RegisterModel register)
        {
            var response = ExecutePost(@"/users/register", JsonConvert.SerializeObject(register));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool CheckToken(string username, string token)
        {
            var response = ExecutePost(@"/users/checkToken", JsonConvert.SerializeObject(new TokenModel {Username = username, Token = token}));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return JsonConvert.DeserializeObject<bool>(response.Content);
        }
    }
}