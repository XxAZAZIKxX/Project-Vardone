using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using VardoneLibrary.Core.Client;
using VardoneLibrary.Core.Client.Base;

namespace TestLibrary;

internal static class Program
{
    private static readonly object Locker = new();
    public static void Main()
    {
        //Console.CursorVisible = false;
        //Console.SetWindowSize(50, 20);
        //Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

        //Task.Run(() =>
        //{
        //    while (true)
        //    {
        //        var s = "Memory usage: ";
        //        var workingSet64 = Process.GetCurrentProcess().WorkingSet64;
        //        var mb = workingSet64 / 1024d / 1024;
        //        var gb = mb / 1024;
        //        if (gb > 1) s += $"{Math.Round((decimal)gb, 4)} gb";
        //        else s += $"{Math.Round((decimal)mb, 4)} mb";
                
        //        Write(s);

        //        Thread.Sleep(500);
        //    }
        //});

        //var client = new VardoneClient(VardoneBaseApi.GetUserToken("az195753@gmail.com", "q"));

        //client.OnNewPrivateMessage += (_) => Task.Run(() => { Print("OnNewPrivateMessage"); });
        //client.OnNewChannelMessage += (_) => Task.Run(() => { Print("OnNewChannelMessage"); });
        //client.OnUpdateUser += (_) => Task.Run(() => { Print("OnUpdateUser"); });
        //client.OnUpdateChatList += () => Task.Run(() => { Print("OnUpdateChatList"); });
        //client.OnUpdateFriendList += () => Task.Run(() => { Print("OnUpdateFriendList"); });
        //client.OnUpdateIncomingFriendRequestList += (_) => Task.Run(() => { Print("OnUpdateIncomingFriendRequestList"); });
        //client.OnUpdateOutgoingFriendRequestList += () => Task.Run(() => { Print("OnUpdateOutgoingFriendRequestList"); });
        //client.OnUpdateOnline += (_) => Task.Run(() => { Print("OnUpdateOnline"); });
        //client.OnUpdateGuildList += () => Task.Run(() => { Print("OnUpdateGuildList"); });
        //client.OnUpdateChannelList += (_) => Task.Run(() => { Print("OnUpdateChannelList"); });
        //client.OnDeleteChannelMessage += (_) => Task.Run(() => { Print("OnDeleteChannelMessage"); });
        //client.OnDeletePrivateChatMessage += (_) => Task.Run(() => { Print("OnDeletePrivateChatMessage"); });
        //client.OnDisconnect += () => Task.Run(() => { Print("OnDisconnect"); });

        //a:
        //var key = Console.ReadKey(true);
        //if(key.Key is ConsoleKey.R)Console.Clear();
        //if(key.Key is ConsoleKey.Escape) Environment.Exit(0);
        //goto a;


        using var sha512 = SHA512.Create();
        var computeHash = sha512.ComputeHash(Encoding.Default.GetBytes("q"));
        var sb = new StringBuilder();
        foreach (var b in computeHash) sb.Append(b.ToString("X"));
        Console.WriteLine(sb.ToString());
    }

    private static void Print(string s) => WriteLine(s);

    private static void WriteLine(string s) => Write(s + '\n');

    private static void Write(string s)
    {
        lock (Locker)
        {
            ClearLine();
            Console.Write(s);
        }
    }

    private static void ClearLine()
    {
        Console.SetCursorPosition(0,Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth-1));
        Console.SetCursorPosition(0,Console.CursorTop);
    }
}