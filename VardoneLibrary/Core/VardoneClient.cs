using System;
using RestSharp;
using VardoneLibrary.Core.Base;

namespace VardoneLibrary.Core
{
    public class VardoneClient : BaseClient
    {
        public string Username { get; protected set; }
        public string Token { get; protected set; }

        public VardoneClient(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            Username = username;
            Token = token;
            if (CheckToken(username, token) == false) throw new Exception("Invalid arguments. User invalid");
        }

        protected IRestResponse ExecutePostWithToken(string resource, string json)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("username", Username);
            request.AddHeader("token", Token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            return REST_CLIENT.Execute(request);
        }
    }
}