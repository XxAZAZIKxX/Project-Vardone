using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;

namespace VardoneApi.Core.CreateHelpers
{
    public static class MessageCreateHelper
    {
        public static ChannelMessage GetChannelMessage(long messageId, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var channelMessages = dataContext.ChannelMessages;
            channelMessages.Include(p => p.Author).Load();
            channelMessages.Include(p => p.Channel).Load();
            var message = channelMessages.FirstOrDefault(p => p.Id == messageId);
            if (message == null) return null;
            var channelMessage = new ChannelMessage
            {
                MessageId = message.Id,
                Author = UserCreateHelper.GetUser(message.Author.Id, true),
                Channel = GuildCreateHelper.GetChannel(message.Channel.Id),
                CreatedTime = message.CreatedTime
            };
            if (!onlyId)
            {
                channelMessage.Text = message.Text;
                channelMessage.Base64Image = message.Image is null ? null : Convert.ToBase64String(message.Image);
            }
            return channelMessage;
        }

        public static PrivateMessage GetPrivateMessage(long messageId, long userId, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var privateMessages = dataContext.PrivateMessages;
            privateMessages.Include(p => p.Author).Load();
            privateMessages.Include(p => p.Chat).Load();
            var message = privateMessages.FirstOrDefault(p => p.Id == messageId);
            if (message is null) return null;
            var privateMessage = new PrivateMessage
            {
                MessageId = message.Id,
                Author = UserCreateHelper.GetUser(message.Author.Id),
                Chat = PrivateChatCreateHelper.GetPrivateChat(message.Chat.Id, userId),
                CreatedTime = message.CreatedTime
            };
            if (!onlyId)
            {
                privateMessage.Text = message.Text;
                privateMessage.Base64Image = message.Image is null ? null : Convert.ToBase64String(message.Image);
            }
            return privateMessage;
        }
    }
}