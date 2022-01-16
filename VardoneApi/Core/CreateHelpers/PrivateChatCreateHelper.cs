using VardoneApi.Entity.Models.PrivateChats;
using VardoneEntities.Entities.Chat;

namespace VardoneApi.Core.CreateHelpers
{
    public static class PrivateChatCreateHelper
    {
        public static PrivateChat GetPrivateChat(PrivateChatsTable chat, long userId)
        {
            if (chat is null) return null;

            var user1 = chat.FromUser.Id == userId ? chat.FromUser : chat.ToUser;
            var user2 = chat.ToUser.Id == userId ? chat.FromUser : chat.ToUser;

            var chatReturn = new PrivateChat
            {
                ChatId = chat.Id,
                FromUser = UserCreateHelper.GetUser(user1, true),
                ToUser = UserCreateHelper.GetUser(user2, true),
            };
            
            return chatReturn;
        }
    }
}