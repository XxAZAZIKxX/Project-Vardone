using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        ~VardoneBaseClient()
        {
            TcpClientClose();
        }
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
                if (message?.data is null) return;
                Console.WriteLine(message);
                switch (message.type)
                {
                    case TypeTcpResponse.NewPrivateMessage:
                        OnNewPrivateMessage?.Invoke(message.data as PrivateMessage);
                        break;
                    case TypeTcpResponse.DeletePrivateMessage:
                        OnDeletePrivateChatMessage?.Invoke(message.data as PrivateMessage);
                        break;
                    case TypeTcpResponse.NewPrivateChat:
                        OnNewPrivateChat?.Invoke(message.data as PrivateChat);
                        break;
                    case TypeTcpResponse.DeletePrivateChat:
                        OnDeletePrivateChat?.Invoke(message.data as PrivateChat);
                        break;
                    case TypeTcpResponse.UpdateUser:
                        OnUpdateUser?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.UpdateUserOnline:
                        OnUpdateUserOnline?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.NewIncomingFriendRequest:
                        OnNewIncomingFriendRequest?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.NewOutgoingFriendRequest:
                        OnNewOutgoingFriendRequest?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.NewFriend:
                        OnNewFriend?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.DeleteIncomingFriendRequest:
                        OnDeleteIncomingFriendRequest?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.DeleteOutgoingFriendRequest:
                        OnDeleteOutgoingFriendRequest?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.DeleteFriend:
                        OnDeleteFriend?.Invoke(message.data as User);
                        break;
                    case TypeTcpResponse.GuildJoin:
                        OnGuildJoin?.Invoke(message.data as Guild);
                        break;
                    case TypeTcpResponse.GuildLeave:
                        OnGuildLeave?.Invoke(message.data as Guild);
                        break;
                    case TypeTcpResponse.UpdateGuild:
                        OnGuildUpdate?.Invoke(message.data as Guild);
                        break;
                    case TypeTcpResponse.NewChannel:
                        OnNewChannel?.Invoke(message.data as Channel);
                        break;
                    case TypeTcpResponse.DeleteChannel:
                        OnDeleteChannel?.Invoke(message.data as Channel);
                        break;
                    case TypeTcpResponse.UpdateChannel:
                        OnUpdateChannel?.Invoke(message.data as Channel);
                        break;
                    case TypeTcpResponse.NewChannelMessage:
                        OnNewChannelMessage?.Invoke(message.data as ChannelMessage);
                        break;
                    case TypeTcpResponse.DeleteChannelMessage:
                        OnDeleteChannelMessage?.Invoke(message.data as ChannelMessage);
                        break;
                    case TypeTcpResponse.NewGuildInvite:
                        OnNewGuildInvite?.Invoke(message.data as GuildInvite);
                        break;
                    case TypeTcpResponse.DeleteGuildInvite:
                        OnDeleteGuildInvite?.Invoke(message.data as GuildInvite);
                        break;
                    case TypeTcpResponse.NewMember:
                        OnNewMember?.Invoke(message.data as Member);
                        break;
                    case TypeTcpResponse.DeleteMember:
                        OnDeleteMember?.Invoke(message.data as Member);
                        break;
                    case TypeTcpResponse.BanMember:
                        OnBanMember?.Invoke(message.data as BannedMember);
                        break;
                    case TypeTcpResponse.UnbanMember:
                        OnUnbanMember?.Invoke(message.data as BannedMember);
                        break;
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
            Console.WriteLine("Closed");
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