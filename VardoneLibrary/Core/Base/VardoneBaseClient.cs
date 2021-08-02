using System;
using System.Collections.Generic;
using RestSharp;
using VardoneLibrary.Exceptions;

namespace VardoneLibrary.Core.Base
{
    public abstract class VardoneBaseClient : VardoneBaseApi
    {
        public long UserId { get; protected set; }
        public string Token { get; protected set; }

        protected VardoneBaseClient(long userId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            UserId = userId;
            Token = token;
            if (!CheckToken(UserId, Token)) throw new UnauthorizedException("Invalid token");
        }

        protected IRestResponse ExecutePostWithToken(string resource, string json = null,
            Dictionary<string, string> queryParameters = null)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("userId", UserId.ToString());
            request.AddHeader("token", Token);
            if (json != null) request.AddParameter("application/json", json, ParameterType.RequestBody);
            if (queryParameters == null) return RestClient.Execute(request);
            foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);
            return RestClient.Execute(request);
        }
    }
}