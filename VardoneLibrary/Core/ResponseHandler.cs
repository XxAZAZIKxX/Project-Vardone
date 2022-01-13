using System.Linq;
using System.Net;
using RestSharp;

namespace VardoneLibrary.Core
{
    public enum ResponseStatus
    {
        Ok, UpdateToken, InvalidToken, Error
    }
    public static class ResponseHandler
    {
        public static ResponseStatus GetResponseStatus(IRestResponse response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: return IsTokenExpired(response) ? ResponseStatus.UpdateToken : ResponseStatus.InvalidToken;
                case HttpStatusCode.OK: return ResponseStatus.Ok;
                default: return ResponseStatus.Error;
            }
        }

        /// <summary>
        /// Просрочен ли токен
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool IsTokenExpired(IRestResponse response) => response.Headers.ToList().Exists(p => p.Name == "Token-Expired" && (string)p.Value == "true");
    }
}