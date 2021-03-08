﻿using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VardoneLibrary.Models.ApiModels;

namespace VardoneLibrary.Core.Base
{
    public abstract class BaseApi
    {
        protected static readonly RestClient REST_CLIENT = new("https://localhost:5001/") { Timeout = -1 };

        protected static IRestResponse ExecutePost(string resource, string json = null, Dictionary<string, string> queryParameters = null)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (json != null) request.AddParameter("application/json", json, ParameterType.RequestBody);

            if (queryParameters == null) return REST_CLIENT.Execute(request);

            foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);

            return REST_CLIENT.Execute(request);
        }

        public static TokenModel GetUserToken(string email, string password)
        {
            var response = ExecutePost(@"/users/authUser",
                JsonConvert.SerializeObject(new GetTokenModel { Email = email, Password = password }));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return JsonConvert.DeserializeObject<TokenModel>(response.Content);
        }

        public static bool RegisterUser(RegisterModel register)
        {
            var response = ExecutePost(@"/users/registerUser", JsonConvert.SerializeObject(register));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool CheckToken(long userId, string token)
        {
            var response = ExecutePost(@"/users/checkUserToken", JsonConvert.SerializeObject(new TokenModel { UserId = userId, Token = token }));
            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return JsonConvert.DeserializeObject<bool>(response.Content);
        }
    }
}