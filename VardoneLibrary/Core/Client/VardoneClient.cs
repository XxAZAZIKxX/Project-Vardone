using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
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

        public VardoneClient(long userId, string token) : base(userId, token) =>
            _clientBackground = new VardoneClientBackground(this);
        private void StopClient()
        {
            _clientBackground.StopThreads();
            _clientBackground = null;
            UserId = long.MinValue;
            Token = null;
        }
        ~VardoneClient() => StopClient();

        //Get
        public User GetMe()
        {
            var response = ExecutePostWithToken("users/getMe");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException(),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<User>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }
        public User GetUser(long id)
        {
            var response = ExecutePostWithToken("users/getUser", null, new Dictionary<string, string> { { "secondId", id.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException(),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<User>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }
        public List<User> GetFriends()
        {
            var response = ExecutePostWithToken("users/getFriends");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException(),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<User>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }
        public List<User> GetIncomingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getIncomingFriendRequests");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException(),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<User>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }
        public List<User> GetOutgoingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getOutgoingFriendRequests");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException(),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<User>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }
        public bool GetOnlineUser(long userId)
        {
            var response = ExecutePostWithToken("users/getUserOnline", null,
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException(),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<bool>((response.Content)),
                _ => throw new Exception(response.Content)
            };
        }
        //Delete
        public void DeleteFriend(long idUser)
        {
            var response = ExecutePostWithToken("users/deleteFriend", null,
                new Dictionary<string, string> { { "secondId", idUser.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
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
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
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
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
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
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }
        public void UpdateLastOnline()
        {
            var response = ExecutePostWithToken("users/setOnline");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
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
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
            }
        }
        public void CloseAllSessions()
        {
            var response = ExecutePostWithToken("users/CloseAllSessions");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
            }
        }
        //Other
        public void AddFriend(string secondUsername)
        {
            var response = ExecutePostWithToken("users/addFriend", null,
                new Dictionary<string, string> { { "secondUsername", secondUsername } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
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
    }
}