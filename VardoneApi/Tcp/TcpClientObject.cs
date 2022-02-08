using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VardoneApi.Core;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneEntities.Models.TcpModels;

namespace VardoneApi.Tcp
{
    public class TcpClientObject
    {
        public string Id { get; }
        public UserTokenModel Token { get; }
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly TcpServerObject _serverObject;
        private readonly object _sendLocker = new();
        private bool _disposed;

        public TcpClientObject(string id, TcpClient client, TcpServerObject serverObject)
        {
            Id = id;
            _client = client;
            _serverObject = serverObject;
            _stream = client.GetStream();
            //
            if (!CheckToken(GetMessage(), out var token))
            {
                SendMessage(new TcpResponseModel { type = TypeTcpResponse.Disconnected });
                Close();
                return;
            }

            Token = token;
            //
            _serverObject.AddConnection(this);
            SendMessage(new TcpResponseModel { type = TypeTcpResponse.Connected });
            new Thread(CheckConnection) { IsBackground = true }.Start();
        }

        private void CheckConnection()
        {
            while (IsConnected()) Thread.Sleep(TimeSpan.FromSeconds(5));
            _serverObject.RemoveConnection(Id);
            Close();
        }
        private string GetMessage()
        {
            try
            {
                var data = new byte[256];
                var sb = new StringBuilder();
                do
                {
                    var bytes = _stream.Read(data, 0, data.Length);
                    sb.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (_stream.DataAvailable);

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }
        public void SendMessage(TcpResponseModel message)
        {
            Task.Run(() =>
            {
                lock (_sendLocker)
                {
                    while (IsConnected())
                    {
                        SendBytes(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
                        message = JsonConvert.DeserializeObject<TcpResponseModel>(GetMessage());
                        if (message?.type == TypeTcpResponse.Accepted) break;
                    }
                }
            });
        }
        private void SendBytes(byte[] bytes)
        {
            try
            {
                _client.GetStream().Write(bytes, 0, bytes.Length);
            }
            catch
            {
                // ignored
            }
        }
        public void Close()
        {
            SendMessage(new TcpResponseModel { type = TypeTcpResponse.Disconnected });
            _disposed = true;
            try
            {
                _stream.Close();
                _client.Close();
            }
            catch
            {
                // ignored
            }
        }
        //
        public bool IsConnected()
        {
            if (_disposed) return false;
            var client = _client.Client;
            var blockingState = client.Blocking;
            try
            {
                var tmp = new byte[1];
                client.Blocking = false;
                client.Send(tmp, 0, 0);
                return true;
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                return e.NativeErrorCode.Equals(10035);
            }
            finally
            {
                if (!_disposed) client.Blocking = blockingState;
            }
        }
        private static bool CheckToken(string jwtToken, out UserTokenModel token)
        {
            if (!JwtTokenWorker.CheckJwtToken(jwtToken, out var jwt))
            {
                token = null;
                return false;
            }

            token = JwtTokenWorker.GetUserToken(jwt.Claims);
            return true;
        }
    }
}