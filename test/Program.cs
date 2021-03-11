using System;
using System.ComponentModel.DataAnnotations;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Base;
using VardoneLibrary.Core.Events;

namespace testLibrary
{
    internal class Program
    {
        private static void Main()
        {
            Console.ReadKey(false);
            var client = new VardoneClient(1, "3FBA7EC5CF079C7437A519C3BB958562");
            Console.WriteLine("Started...");
            Events.newPrivateMessage += ClientOnAddPrivateMessage;
            client.StartReceiving();
            Console.ReadKey(false);
        }

        private static void ClientOnAddPrivateMessage(PrivateMessage message) => Console.WriteLine("["+message.Chat.ChatId +"]" + " " + message.Author.Username + " - " + message.Text);
    }
}
