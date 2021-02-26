using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VardoneLibrary.Models;

namespace VardoneLibrary
{
    public class VardoneClient
    {
        private string Username { get; }
        private string Token { get; }

        private static readonly RestClient RestClient = new("https://localhost:5001/");

        public VardoneClient(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            Username = username;
            Token = token;
        }

        public static string GetToken(string username, string password)
        {
            var request = new RestRequest(RestClient.BaseUrl + @"/users/auth", Method.POST);
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json",
                JsonConvert.SerializeObject(new LoginRequestModel {Username = username, Password = password}),
                ParameterType.RequestBody);

            var response = RestClient.Execute(request);
            Console.WriteLine(response.Content);

            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(response.ErrorMessage);
            return JsonConvert.DeserializeObject<LoginResponseModel>(response.Content).Token;
        }
    }
}