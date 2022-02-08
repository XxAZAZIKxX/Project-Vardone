using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models.PrivateChats;
using VardoneEntities.Entities.Chat;

namespace VardoneApi.Core.CreateHelpers
{
    public static class PrivateChatCreateHelper
    {
        public static PrivateChat GetPrivateChat(long chatId, long userId)
        {
            var dataContext = Program.DataContext;
            var privateChats = dataContext.PrivateChats;
            privateChats.Include(p=>p.FromUser).Load();
            privateChats.Include(p=>p.ToUser).Load();
            var messages = dataContext.PrivateMessages;
            messages.Include(p => p.Chat).Load();
            messages.Include(p => p.Author).Load();
            var chat = privateChats.FirstOrDefault(p => p.Id == chatId);
            if (chat is null) return null;

            var user1 = chat.FromUser.Id == userId ? chat.FromUser : chat.ToUser;
            var user2 = chat.FromUser.Id == userId ? chat.ToUser : chat.FromUser;
            var lastReadTime = chat.FromUser.Id == user1.Id ? chat.FromLastReadTimeMessages : chat.ToLastReadTimeMessages;

            var chatReturn = new PrivateChat
            {
                ChatId = chatId,
                FromUser = UserCreateHelper.GetUser(user1.Id, true),
                ToUser = UserCreateHelper.GetUser(user2.Id, true),
                UnreadMessages = messages.Count(p =>
                    p.Chat.Id == chat.Id && p.Author.Id != user1.Id &&
                    DateTime.Compare(p.CreatedTime, lastReadTime) > 0)
            };
            return chatReturn;
        }
    }
}