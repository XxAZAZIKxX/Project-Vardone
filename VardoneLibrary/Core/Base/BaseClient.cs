using System;
using System.Collections.Generic;
using RestSharp;

namespace VardoneLibrary.Core.Base
{
    public abstract class BaseClient : BaseApi
    {
        public string Username { get; protected set; }
        public string Token { get; protected set; }

        protected BaseClient(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));

            Username = username;
            Token = token;
            if (!CheckToken(username, token)) throw new Exception("Invalid arguments. User invalid");
        }

        protected IRestResponse ExecutePostWithToken(string resource, string json, Dictionary<string, string> queryParameters = null)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("username", Username);
            request.AddHeader("token", Token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            if (queryParameters == null) return REST_CLIENT.Execute(request);

            foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);

            return REST_CLIENT.Execute(request);
        }
    }
}