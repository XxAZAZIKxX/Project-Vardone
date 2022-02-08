using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core.CreateHelpers;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneEntities.Models.TcpModels;

namespace VardoneApi.Tcp
{
    public class TcpServerObject
    {
        private readonly TcpListener _listener = new(IPAddress.Any, 34000);
        private readonly List<TcpClientObject> _clients = new();

        private bool _disposed;
        public TcpServerObject()
        {
            _listener.Start();
            new Thread(Listen) { IsBackground = true }.Start();
        }
        ~TcpServerObject() => Disconnect();

        private void Listen()
        {
            try
            {
                while (!_disposed)
                {
                    var client = _listener.AcceptTcpClient();
                    string id;
                    do
                    {
                        id = Guid.NewGuid().ToString();
                    } while (_clients.Any(p => p.Id == id));

                    var _ = new TcpClientObject(id, client, this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Disconnect();
            }
        }

        public void AddConnection(TcpClientObject clientObject)
        {
            Console.WriteLine($"Added tcp client: {clientObject.Id}");
            _clients.Add(clientObject);
            SetUserOnline(clientObject.Token.UserId, true);
        }
        public void RemoveConnection(string id)
        {
            Console.WriteLine($"Removed tcp client: {id}");
            var clientObject = _clients.FirstOrDefault(p => p.Id == id);
            if (clientObject is null) return;
            clientObject.Close();
            _clients.Remove(clientObject);
            var userId = clientObject.Token.UserId;
            if (_clients.Select(p => p.Token.UserId).All(uid => uid != userId)) SetUserOnline(userId, false);
            Task.Run(() =>
            {
                var closedClients = _clients.Where(p => !p.IsConnected()).ToArray();
                foreach (var client in closedClients) RemoveConnection(client.Id);
            });
        }
        public void RemoveConnection(UserTokenModel token)
        {
            foreach (var id in _clients.Where(c => c.Token.Equals(token)).Select(c => c.Id).ToArray()) RemoveConnection(id);
        }
        public void RemoveConnection(long userId)
        {
            foreach (var id in _clients.Where(c => c.Token.UserId == userId).Select(c => c.Id).ToArray()) RemoveConnection(id);
        }
        public void SendMessageTo(long userId, TcpResponseModel message)
        {
            foreach (var client in _clients.Where(c => c.Token.UserId == userId && c.IsConnected()))
            {
                client.SendMessage(message);
            }
        }
        private void SetUserOnline(long userId, bool online)
        {
            var dataContext = Program.DataContext;
            var users = dataContext.Users;
            var usersOnline = dataContext.UsersOnline;
            usersOnline.Include(p => p.User).Load();
            var guildMembers = dataContext.GuildMembers;
            guildMembers.Include(p => p.Guild).Load();
            guildMembers.Include(p => p.User).Load();
            var friendsList = dataContext.FriendsList;
            friendsList.Include(p => p.FromUser).Load();
            friendsList.Include(p => p.ToUser).Load();
            try
            {
                var user = users.FirstOrDefault(p => p.Id == userId);
                if (user is null) return;
                var onlineTable = usersOnline.FirstOrDefault(p => p.User.Id == userId);
                if (onlineTable is not null)
                {
                    onlineTable.IsOnline = online;
                }
                else
                {
                    usersOnline.Add(new UsersOnlineTable { IsOnline = online, User = user });
                }

                dataContext.SaveChanges();
            }
            catch
            {
                // ignored
            }

            var tcpResponseModel = new TcpResponseModel
            {
                type = TypeTcpResponse.UpdateUserOnline,
                data = UserCreateHelper.GetUser(userId, true)
            };
            if (tcpResponseModel.data is null) return;
            var guildIds = guildMembers.Where(p => p.User.Id == userId).Select(p => p.Guild.Id).ToArray();
            var userIds = guildMembers.Where(p => guildIds.Contains(p.Guild.Id) && p.User.Id != userId).Select(p => p.User.Id).ToArray();
            foreach (var id in userIds) SendMessageTo(id, tcpResponseModel);
            foreach (var friendsListTable in friendsList.Where(p => p.FromUser.Id == userId || p.ToUser.Id == userId))
            {
                var u = friendsListTable.FromUser.Id == userId
                    ? friendsListTable.ToUser.Id
                    : friendsListTable.FromUser.Id;
                if (userIds.Contains(u)) continue;
                SendMessageTo(u, tcpResponseModel);
            }
        }

        private void Disconnect()
        {
            _disposed = true;
            foreach (var tcpClientObject in _clients) tcpClientObject.Close();
            _listener.Stop();
        }
    }
}