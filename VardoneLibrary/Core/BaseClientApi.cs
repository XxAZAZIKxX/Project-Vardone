using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VardoneLibrary.Models;

namespace VardoneLibrary.Core
{
    public abstract class BaseClientApi
    {
        private static readonly RestClient RestClient = new("https://localhost:5001/") {Timeout = -1};

        public static IRestResponse ExecutePost(string resource, string json)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            return RestClient.Execute(request);
        }

        public static string GetUserToken(string username, string password)
        {
            var response = ExecutePost(@"/users/auth",
                JsonConvert.SerializeObject(new LoginRequestModel {Username = username, Password = password}));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return JsonConvert.DeserializeObject<LoginResponseModel>(response.Content)?.Token;
        }

        public static bool RegisterUser(RegisterRequestModel register)
        {
            var response = ExecutePost(@"/users/register", JsonConvert.SerializeObject(register));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool CheckToken(string username, string token)
        {
            var response = ExecutePost(@"/users/checkToken",
                JsonConvert.SerializeObject(new LoginResponseModel {Username = username, Token = token}));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            var r = JsonConvert.DeserializeObject<bool>(response.Content);
            return r;
        }
    }
}