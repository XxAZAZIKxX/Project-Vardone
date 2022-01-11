using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;
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

        protected IRestResponse ExecutePostWithToken(string resource, string json = null, Dictionary<string, string> queryParameters = null, bool onlyId = false)

        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {Token}");
            if (onlyId) request.AddHeader("onlyId", true.ToString());
            if (json != null) request.AddParameter("application/json", json, ParameterType.RequestBody);
            if (queryParameters == null) return REST_CLIENT.Execute(request);
            foreach (var (key, value) in queryParameters) request.AddQueryParameter(key, value);
            return REST_CLIENT.Execute(request);
        }
        
        public event Func<PrivateMessage, Task> OnNewPrivateMessage;
        public event Func<ChannelMessage, Task> OnNewChannelMessage;
        public event Func<User,Task> OnUpdateUser;
        public event Func<Task> OnUpdateChatList;
        public event Func<Task> OnUpdateFriendList;
        public event Func<bool,Task> OnUpdateIncomingFriendRequestList;
        public event Func<Task> OnUpdateOutgoingFriendRequestList;
        public event Func<User,Task> OnUpdateOnline;
        public event Func<Task> OnUpdateGuildList;
        public event Func<Guild,Task> OnUpdateChannelList;
        public event Func<Channel,Task> OnDeleteChannelMessage;
        public event Func<PrivateChat,Task> OnDeletePrivateChatMessage;

        internal void NewPrivateMessage(PrivateMessage arg) => OnNewPrivateMessage?.Invoke(arg);
        internal void NewChannelMessage(ChannelMessage arg) => OnNewChannelMessage?.Invoke(arg);
        internal void UpdateUser(User arg) => OnUpdateUser?.Invoke(arg);
        internal void UpdateChatList() => OnUpdateChatList?.Invoke();
        internal void UpdateFriendList() => OnUpdateFriendList?.Invoke();
        internal void UpdateIncomingFriendRequestList(bool arg) => OnUpdateIncomingFriendRequestList?.Invoke(arg);
        internal void UpdateOutgoingFriendRequestList() => OnUpdateOutgoingFriendRequestList?.Invoke();
        internal void UpdateOnline(User arg) => OnUpdateOnline?.Invoke(arg);
        internal void UpdateGuildList() => OnUpdateGuildList?.Invoke();
        internal void UpdateChannelList(Guild arg) => OnUpdateChannelList?.Invoke(arg);
        internal void DeleteChannelMessage(Channel arg) => OnDeleteChannelMessage?.Invoke(arg);
        internal void DeletePrivateChatMessage(PrivateChat arg) => OnDeletePrivateChatMessage?.Invoke(arg);
    }
}