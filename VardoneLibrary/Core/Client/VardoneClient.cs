using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core.Base;
using VardoneLibrary.Exceptions;
using static VardoneLibrary.VardoneEvents.VardoneEvents;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient : VardoneBaseClient
    {
        private VardoneClientBackground _clientBackground;
        public bool SetOnline => _clientBackground.setOnline;

        public VardoneClient(string token) : base(token) => _clientBackground = new VardoneClientBackground(this);

        private void StopClient()
        {
            _clientBackground.StopThreads();
            _clientBackground = null;
            Token = null;
        }
        ~VardoneClient() => StopClient();

        private static bool IsTokenExpired(IRestResponse response) => response.Headers.ToList().Exists(p => p.Name == "Token-Expired" && (string)p.Value == "true");
        private static bool IsTokenInvalid(IRestResponse response) => response.Headers.ToList().Exists(p => p.Name == "Token-Invalid" && (string)p.Value == "true");

        //Get
        public User GetMe()
        {
            var response = ExecutePostWithToken("users/getMe");

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetMe();
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<User>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public User GetUser(long id)
        {
            var response = ExecutePostWithToken("users/getUser", null, new Dictionary<string, string> { { "secondId", id.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetUser(id);
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<User>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<User> GetFriends()
        {
            var response = ExecutePostWithToken("users/getFriends");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetFriends();
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<User>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<User> GetIncomingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getIncomingFriendRequests");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetIncomingFriendRequests();
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<User>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<User> GetOutgoingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getOutgoingFriendRequests");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetOutgoingFriendRequests();
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<User>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public bool GetOnlineUser(long userId)
        {
            var response = ExecutePostWithToken("users/getUserOnline", queryParameters: new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetOnlineUser(userId);
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<bool>((response.Content));
                default:
                    throw new Exception(response.Content);
            }
        }
        //Delete
        public void DeleteFriend(long idUser)
        {
            var response = ExecutePostWithToken("users/deleteFriend", queryParameters: new Dictionary<string, string> { { "secondId", idUser.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeleteFriend(idUser);
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    {
                        onUpdateFriendList?.Invoke();
                        onUpdateIncomingFriendRequestList?.Invoke(true);
                        onUpdateOutgoingFriendRequestList?.Invoke();
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }
        public void DeleteMe()
        {
            var response = ExecutePostWithToken("users/deleteMe");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeleteMe();
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }
        //Update
        public void UpdateMe(UpdateUserModel update)
        {
            var response = ExecutePostWithToken("users/updateUser", JsonConvert.SerializeObject(update));
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        UpdateMe(update);
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    {
                        onUpdateUser?.Invoke(GetMe());
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }
        public void UpdatePassword(UpdatePasswordModel updatePassword)
        {
            var response = ExecutePostWithToken("users/updatePassword", JsonConvert.SerializeObject(updatePassword));
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        UpdatePassword(updatePassword);
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }
        public void UpdateLastOnline()
        {
            var response = ExecutePostWithToken("users/setOnline");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        UpdateLastOnline();
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }
        //Close
        public void CloseCurrentSession()
        {
            var response = ExecutePostWithToken("users/closeCurrentSession");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        CloseCurrentSession();
                        break;
                    }
                    else throw new UnauthorizedException();
            }
        }
        public void CloseAllSessions()
        {
            var response = ExecutePostWithToken("users/CloseAllSessions");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        CloseAllSessions();
                        break;
                    }
                    else throw new UnauthorizedException();
            }
        }
        //Other
        public void AddFriend(string secondUsername)
        {
            var response = ExecutePostWithToken("users/addFriend", null,
                new Dictionary<string, string> { { "secondUsername", secondUsername } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        AddFriend(secondUsername);
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    {
                        onUpdateFriendList?.Invoke();
                        onUpdateIncomingFriendRequestList?.Invoke(true);
                        onUpdateOutgoingFriendRequestList?.Invoke();
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }
        public void UpdateToken()
        {
            var response = ExecutePost(@"auth/updateToken", headers: new Dictionary<string, string>
            {
                {"token", Token}
            });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
                case HttpStatusCode.OK: Token = JsonConvert.DeserializeObject<string>(response.Content); break;
                default: throw new Exception(response.Content);
            }
        }
    }
}