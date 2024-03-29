﻿using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using VardoneEntities.Models.ClientModels;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneLibrary.Core.Client.Base
{
    public abstract class VardoneBaseApi
    {
        public enum RegisterResponse
        {
            EmailBooked, UsernameBooked, None
        }

        protected static readonly RestClient REST_CLIENT = new("https://localhost:5001/") { Timeout = -1 };

        protected static IRestResponse ExecutePost(string resource, string json = null, Dictionary<string, string> queryParameters = null, Dictionary<string, string> headers = null)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (json != null) request.AddParameter("application/json", json, ParameterType.RequestBody);
            if (queryParameters is null && headers is null) return REST_CLIENT.Execute(request);
            if (queryParameters != null) foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);
            if (headers != null) foreach (var header in headers) request.AddHeader(header.Key, header.Value);
            return REST_CLIENT.Execute(request);
        }

        public static string GetUserToken(string email, string password)
        {
            var passwordHash = new StringBuilder();
            using (var sha512 = SHA512.Create())
            {
                var computeHash = sha512.ComputeHash(Encoding.ASCII.GetBytes(password));
                foreach (var b in computeHash) passwordHash.Append(b.ToString("X2"));
            }

            var response = ExecutePost(@"/auth/authUser",
                JsonConvert.SerializeObject(new GetUserTokenClientModel
                {
                    Email = email,
                    PasswordHash = passwordHash.ToString()
                }));
            return response.StatusCode is not HttpStatusCode.OK ? null : JsonConvert.DeserializeObject<string>(response.Content);
        }

        public static bool RegisterUser(RegisterUserModel register, out RegisterResponse rr)
        {
            using (var sha512 = SHA512.Create())
            {
                var sb = new StringBuilder();
                var computeHash = sha512.ComputeHash(Encoding.ASCII.GetBytes(register.PasswordHash));
                foreach (var b in computeHash) sb.Append(b.ToString("X2"));
                register.PasswordHash = sb.ToString();
            }
            var response = ExecutePost(@"/auth/registerUser", JsonConvert.SerializeObject(register));
            if (response.Content.Contains("#1")) rr = RegisterResponse.EmailBooked;
            else if (response.Content.Contains("#2")) rr = RegisterResponse.UsernameBooked;
            else rr = RegisterResponse.None;
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool CheckToken(ref string token)
        {
            if (token is null) return false;
            var response = ExecutePost(@"/auth/checkUserToken", headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } });
            var res = response.StatusCode switch
            {
                HttpStatusCode.OK => JsonConvert.DeserializeObject<bool>(response.Content),
                _ => false
            };
            if (res) return true;
            response = ExecutePost(@"auth/updateToken", headers: new Dictionary<string, string> { { "token", token } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    token = JsonConvert.DeserializeObject<string>(response.Content);
                    return true;
                default: return false;
            }
        }
    }
}