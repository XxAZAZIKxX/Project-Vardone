using System;
using System.Collections.Generic;
using RestSharp;
using VardoneLibrary.Core.Exceptions;

namespace VardoneLibrary.Core.Base
{
    public abstract class BaseClient : BaseApi
    {
        public long UserId { get; protected set; }
        public string Token { get; protected set; }

        protected BaseClient(long userId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));

            UserId = userId;
            Token = token;
            if (!CheckToken(UserId, Token)) throw new UnauthorizedException("User invalid");
        }

        protected IRestResponse ExecutePostWithToken(string resource, string json = null, Dictionary<string, string> queryParameters = null)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("userId", UserId.ToString());
            request.AddHeader("token", Token);
            if (json != null) request.AddParameter("application/json", json, ParameterType.RequestBody);

            if (queryParameters == null) return REST_CLIENT.Execute(request);

            foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);

            return REST_CLIENT.Execute(request);
        }
    }
}