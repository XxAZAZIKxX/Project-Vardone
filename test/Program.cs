using System;
using System.ComponentModel.DataAnnotations;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Base;

namespace testLibrary
{
    internal class Program
    {
        private static void Main()
        {
            var client = new VardoneClient(1, "3FBA7EC5CF079C7437A519C3BB958562");
            Console.WriteLine("Started...");
            client.AddPrivateMessage += ClientOnAddPrivateMessage;
            client.StartReceiving();
            Console.ReadKey(false);
        }

        private static void ClientOnAddPrivateMessage(PrivateMessage message) => Console.WriteLine(message.Chat + " " + message.Author.Username + " - " + message.Text);
    }
}
