using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VardoneEntities.Entities.User;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core.Client.Base;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient : VardoneBaseClient
    {
        public VardoneClient(string token) : base(token)
        {
            OnDisconnect += () => Task.Run(StopClient);
        }

        private void StopClient()
        {
            Token = null;
            TcpClientClose();
        }
        ~VardoneClient() => StopClient();


        //===============================[GET]===============================
        /// <summary>
        /// Получить объект текущего пользователя
        /// </summary>
        /// <returns></returns>
        public User GetMe()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/getMe");
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<User>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Получить объект пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        public User GetUser(long id)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/getUser", null, new Dictionary<string, string> { { "secondId", id.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<User>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Получить список друзей текущего пользователя
        /// </summary>
        /// <returns></returns>
        public  User[] GetFriends()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/getFriends");
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<User[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        return GetFriends();
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary>
        /// Получить входящие запросы в друзья текущего пользователя
        /// </summary>
        /// <returns></returns>
        public User[] GetIncomingFriendRequests()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/getIncomingFriendRequests");
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<User[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        return GetIncomingFriendRequests();
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Получить исходящие запросы в друзья текущего пользователя
        /// </summary>
        /// <returns></returns>
        public  User[] GetOutgoingFriendRequests()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/getOutgoingFriendRequests");
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<User[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        return GetOutgoingFriendRequests();
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Получить статус "в сети" пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
        public bool GetOnlineUser(long userId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/getUserOnline", queryParameters: new Dictionary<string, string> { { "secondId", userId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<bool>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return false;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[DELETE]===============================
        /// <summary>
        /// Удалить пользователя с друзей
        /// </summary>
        /// <param name="idUser">Id пользователя</param>
        public void DeleteFriend(long idUser)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/deleteFriend", queryParameters: new Dictionary<string, string> { { "secondId", idUser.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Удалить текущего пользователя
        /// </summary>
        public void DeleteMe()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/deleteMe");
                StopClient();
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[UPDATE]===============================
        /// <summary>
        /// Обновить текущего пользователя
        /// </summary>
        /// <param name="update"></param>
        public void UpdateMe(UpdateUserModel update)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/updateUser", JsonConvert.SerializeObject(update));
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Изменить пароль текущего пользователя
        /// </summary>
        /// <param name="updatePassword"></param>
        public void UpdatePassword(UpdatePasswordModel updatePassword)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/updatePassword", JsonConvert.SerializeObject(updatePassword));
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Обновить свой статус "в сети"
        /// </summary>
        public void UpdateLastOnline()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/setOnline");
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[CLOSE]===============================
        /// <summary>
        /// Закрыть текущую сессию
        /// </summary>
        public void CloseCurrentSession()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/closeCurrentSession");
                StopClient();
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Закрыть сессии на всех устройствах
        /// </summary>
        public void CloseAllSessions()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/CloseAllSessions");
                StopClient();
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[OTHER]===============================
        /// <summary>
        /// Добавить друга
        /// </summary>
        /// <param name="secondUsername">Username пользователя</param>
        public void AddFriend(string secondUsername)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/addFriend", null, new Dictionary<string, string> { { "secondUsername", secondUsername } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Обновить токен Jwt
        /// </summary>
        private void UpdateToken()
        {
            var response = ExecutePost(@"auth/updateToken",
                headers: new Dictionary<string, string> { { "token", Token } });
            switch (ResponseHandler.GetResponseStatus(response))
            {
                case ResponseStatus.Ok: return;

                case ResponseStatus.UpdateToken:
                case ResponseStatus.InvalidToken:
                    EventDisconnectInvoke();
                    return;
                case ResponseStatus.Error: throw new Exception(response.ErrorMessage);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}