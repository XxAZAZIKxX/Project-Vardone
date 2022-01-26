using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;

namespace VardoneLibrary.Core.Client.Base
{
    public abstract class VardoneBaseClient : VardoneBaseApi
    {
        public string Token { get; protected set; }

        protected VardoneBaseClient(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            if (!CheckToken(ref token)) throw new Exception("Invalid token");
            Token = token;
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
        public event Func<long,Task> OnDeleteChannelMessage;
        public event Func<long,Task> OnDeletePrivateChatMessage;
        public event Func<Task> OnDisconnect; 

        internal void EventNewPrivateMessageInvoke(PrivateMessage arg) => OnNewPrivateMessage?.Invoke(arg);
        internal void EventNewChannelMessageInvoke(ChannelMessage arg) => OnNewChannelMessage?.Invoke(arg);
        internal void EventUpdateUserInvoke(User arg) => OnUpdateUser?.Invoke(arg);
        internal void EventUpdateChatListInvoke() => OnUpdateChatList?.Invoke();
        internal void EventUpdateFriendListInvoke() => OnUpdateFriendList?.Invoke();
        internal void EventUpdateIncomingFriendRequestListInvoke(bool arg) => OnUpdateIncomingFriendRequestList?.Invoke(arg);
        internal void EventUpdateOutgoingFriendRequestListInvoke() => OnUpdateOutgoingFriendRequestList?.Invoke();
        internal void EventUpdateOnlineInvoke(User arg) => OnUpdateOnline?.Invoke(arg);
        internal void EventUpdateGuildListInvoke() => OnUpdateGuildList?.Invoke();
        internal void EventUpdateChannelListInvoke(Guild arg) => OnUpdateChannelList?.Invoke(arg);
        internal void EventDeleteChannelMessageInvoke(long arg) => OnDeleteChannelMessage?.Invoke(arg);
        internal void EventDeletePrivateChatMessageInvoke(long arg) => OnDeletePrivateChatMessage?.Invoke(arg);
        internal void EventDisconnectInvoke() => OnDisconnect?.Invoke();
    }
}