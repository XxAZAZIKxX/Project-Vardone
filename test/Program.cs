using System;
using VardoneEntities.Entities;
using VardoneLibrary.Core;
using VardoneLibrary.VardoneEvents;

namespace testLibrary
{
    internal class Program
    {
        private static void Main()
        {
            var client = new VardoneClient(1, "907FFCF82A1E87FE98254AEE12FDE0F0");
            Console.WriteLine("Started...");
            VardoneEvents.onNewPrivateMessage += ClientOnAddPrivateMessage;
            Console.ReadKey(false);
        }

        private static void ClientOnAddPrivateMessage(PrivateMessage message) => Console.WriteLine("["+message.Chat.ChatId +"]" + " " + message.Author.Username + " - " + message.Text);
    }
}
