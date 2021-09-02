using System;
using System.Collections.Generic;
using RestSharp;
using VardoneLibrary.Exceptions;

namespace VardoneLibrary.Core.Client.Base
{
    public abstract class VardoneBaseClient : VardoneBaseApi
    {
        public string Token { get; protected set; }

        protected VardoneBaseClient(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            Token = token;
            if (!CheckToken(Token)) throw new UnauthorizedException("Invalid token");
        }
        protected IRestResponse ExecutePostWithToken(string resource, string json = null, Dictionary<string, string> queryParameters = null)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {Token}");
            if (json != null) request.AddParameter("application/json", json, ParameterType.RequestBody);
            if (queryParameters == null) return REST_CLIENT.Execute(request);
            foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);
            return REST_CLIENT.Execute(request);
        }
    }
}