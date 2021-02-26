using System;

namespace VardoneLibrary.Core
{
    public class VardoneClient : BaseClientApi
    {
        public string Username { get; }
        public string Token { get; }

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
    }
}