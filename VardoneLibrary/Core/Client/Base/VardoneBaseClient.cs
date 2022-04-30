using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;
using VardoneEntities.Models.TcpModels;

namespace VardoneLibrary.Core.Client.Base
{
    public abstract class VardoneBaseClient : VardoneBaseApi
    {
        public string Token { get; protected set; }
        private readonly TcpClient _tcpClient = new();
        private readonly NetworkStream _stream;
        private bool _tcpDisposed;
        protected VardoneBaseClient(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            if (!CheckToken(ref token)) throw new Exception("Invalid token");
            Token = token;
            //
            _tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 34000);
            _stream = _tcpClient.GetStream();
            TcpSendMessage(Token);
            var r = TcpGetMessage();
            if (r?.type != TypeTcpResponse.Connected) throw new Exception("Tcp connection was not established");
            TcpSendMessageAccepted();
            new Thread(TcpListener) { IsBackground = true }.Start();
            Console.WriteLine("Connected");
        }
        ~VardoneBaseClient() => TcpClientClose();

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

        private void TcpListener()
        {
            while (!_tcpDisposed)
            {
                var message = TcpGetMessage();
                if (message is null || message.type == TypeTcpResponse.Disconnected) break;
                TcpMessageHandler(message);
                TcpSendMessageAccepted();
            }
            TcpClientClose();
        }
        private void TcpMessageHandler(TcpResponseModel message)
        {
            Task.Run(() =>
            {
                if (message?.data is null or not JObject) return;
                File.WriteAllText("log.txt", message.ToString());
                switch (message.type)
                {
                    case TypeTcpResponse.NewPrivateMessage:
                        {
                            var data = ((JObject)message.data).ToObject<PrivateMessage>();
                            if (data is null) return;
                            OnNewPrivateMessage?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeletePrivateMessage:
                        {
                            var data = ((JObject)message.data).ToObject<PrivateMessage>();
                            if (data is null) return;
                            OnDeletePrivateChatMessage?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewPrivateChat:
                        {
                            var data = ((JObject)message.data).ToObject<PrivateChat>();
                            if (data is null) return;
                            OnNewPrivateChat?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeletePrivateChat:
                        {
                            var data = ((JObject)message.data).ToObject<PrivateChat>();
                            if (data is null) return;
                            OnDeletePrivateChat?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.UpdateUser:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnUpdateUser?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.UpdateUserOnline:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnUpdateUserOnline?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewIncomingFriendRequest:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnNewIncomingFriendRequest?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewOutgoingFriendRequest:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnNewOutgoingFriendRequest?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewFriend:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnNewFriend?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeleteIncomingFriendRequest:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnDeleteIncomingFriendRequest?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeleteOutgoingFriendRequest:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnDeleteOutgoingFriendRequest?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeleteFriend:
                        {
                            var data = ((JObject)message.data).ToObject<User>();
                            if (data is null) return;
                            OnDeleteFriend?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.GuildJoin:
                        {
                            var data = ((JObject)message.data).ToObject<Guild>();
                            if (data is null) return;
                            OnGuildJoin?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.GuildLeave:
                        {
                            var data = ((JObject)message.data).ToObject<Guild>();
                            if (data is null) return;
                            OnGuildLeave?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.UpdateGuild:
                        {
                            var data = ((JObject)message.data).ToObject<Guild>();
                            if (data is null) return;
                            OnGuildUpdate?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewChannel:
                        {
                            var data = ((JObject)message.data).ToObject<Channel>();
                            if (data is null) return;
                            OnNewChannel?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeleteChannel:
                        {
                            var data = ((JObject)message.data).ToObject<Channel>();
                            if (data is null) return;
                            OnDeleteChannel?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.UpdateChannel:
                        {
                            var data = ((JObject)message.data).ToObject<Channel>();
                            if (data is null) return;
                            OnUpdateChannel?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewChannelMessage:
                        {
                            var data = ((JObject)message.data).ToObject<ChannelMessage>();
                            if (data is null) return;
                            OnNewChannelMessage?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeleteChannelMessage:
                        {
                            var data = ((JObject)message.data).ToObject<ChannelMessage>();
                            if (data is null) return;
                            OnDeleteChannelMessage?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewGuildInvite:
                        {
                            var data = ((JObject)message.data).ToObject<GuildInvite>();
                            if (data is null) return;
                            OnNewGuildInvite?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeleteGuildInvite:
                        {
                            var data = ((JObject)message.data).ToObject<GuildInvite>();
                            if (data is null) return;
                            OnDeleteGuildInvite?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.NewMember:
                        {
                            var data = ((JObject)message.data).ToObject<Member>();
                            if (data is null) return;
                            OnNewMember?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.DeleteMember:
                        {
                            var data = ((JObject)message.data).ToObject<Member>();
                            if (data is null) return;
                            OnDeleteMember?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.BanMember:
                        {
                            var data = ((JObject)message.data).ToObject<BannedMember>();
                            if (data is null) return;
                            OnBanMember?.Invoke(data);
                            break;
                        }
                    case TypeTcpResponse.UnbanMember:
                        {
                            var data = ((JObject)message.data).ToObject<BannedMember>();
                            if (data is null) return;
                            OnUnbanMember?.Invoke(data);
                            break;
                        }
                    default: return;
                }
            });
        }
        private TcpResponseModel TcpGetMessage()
        {
            try
            {
                var bytes = new byte[256];
                var sb = new StringBuilder();
                do
                {
                    var count = _stream.Read(bytes, 0, bytes.Length);
                    sb.Append(Encoding.UTF8.GetString(bytes, 0, count));
                } while (_stream.DataAvailable);
                return string.IsNullOrEmpty(sb.ToString()) ? null : JsonConvert.DeserializeObject<TcpResponseModel>(sb.ToString());
            }
            catch
            {
                return null;
            }
        }
        private void TcpSendMessageAccepted() => TcpSendMessage(JsonConvert.SerializeObject(new TcpResponseModel { type = TypeTcpResponse.Accepted }));
        private void TcpSendMessage(string message)
        {
            if (message is null) return;
            var bytes = Encoding.UTF8.GetBytes(message);
            try
            {
                _stream.Write(bytes, 0, bytes.Length);
            }
            catch
            {
                // ignored
            }
        }
        protected void TcpClientClose()
        {
            _tcpDisposed = true;
            try
            {
                _stream.Close();
                _tcpClient.Close();
            }
            catch
            {
                // ignored
            }
        }

        public event Func<Task> OnDisconnect;
        //
        public event Func<PrivateMessage, Task> OnNewPrivateMessage;
        public event Func<PrivateMessage, Task> OnDeletePrivateChatMessage;
        public event Func<ChannelMessage, Task> OnNewChannelMessage;
        public event Func<ChannelMessage, Task> OnDeleteChannelMessage;
        //
        public event Func<PrivateChat, Task> OnNewPrivateChat;
        public event Func<PrivateChat, Task> OnDeletePrivateChat;
        public event Func<Channel, Task> OnNewChannel;
        public event Func<Channel, Task> OnUpdateChannel;
        public event Func<Channel, Task> OnDeleteChannel;
        //
        public event Func<User, Task> OnUpdateUser;
        public event Func<User, Task> OnUpdateUserOnline;
        //
        public event Func<User, Task> OnNewIncomingFriendRequest;
        public event Func<User, Task> OnDeleteIncomingFriendRequest;
        public event Func<User, Task> OnNewOutgoingFriendRequest;
        public event Func<User, Task> OnDeleteOutgoingFriendRequest;
        public event Func<User, Task> OnNewFriend;
        public event Func<User, Task> OnDeleteFriend;
        //
        public event Func<Guild, Task> OnGuildJoin;
        public event Func<Guild, Task> OnGuildLeave;
        public event Func<Guild, Task> OnGuildUpdate;
        //
        public event Func<GuildInvite, Task> OnNewGuildInvite;
        public event Func<GuildInvite, Task> OnDeleteGuildInvite;
        //
        public event Func<Member, Task> OnNewMember;
        public event Func<Member, Task> OnDeleteMember;
        //
        public event Func<BannedMember, Task> OnBanMember;
        public event Func<BannedMember, Task> OnUnbanMember;
        //
        internal void EventDisconnectInvoke() => OnDisconnect?.Invoke();
    }
}