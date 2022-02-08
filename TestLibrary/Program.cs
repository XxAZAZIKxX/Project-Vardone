using VardoneLibrary.Core.Client;
using VardoneLibrary.Core.Client.Base;

namespace TestLibrary;

internal static class Program
{
    public static void Main()
    {
        var client = new VardoneClient(VardoneBaseApi.GetUserToken("az195753@gmail.com", "q"));
        client.OnDisconnect += () => Task.Run(() => { Console.WriteLine("OnDisconnect"); });
        //
        client.OnNewPrivateMessage += (_) => Task.Run(() => { Console.WriteLine("OnNewPrivateMessage"); });
        client.OnDeletePrivateChatMessage += (_) => Task.Run(() => { Console.WriteLine("OnDeletePrivateChatMessage"); });
        client.OnNewChannelMessage += (_) => Task.Run(() => { Console.WriteLine("OnNewChannelMessage"); });
        client.OnDeleteChannelMessage += (_) => Task.Run(() => { Console.WriteLine("OnDeleteChannelMessage"); });
        //
        client.OnNewPrivateChat += (_) => Task.Run(() => { Console.WriteLine("OnNewPrivateChat"); });
        client.OnDeletePrivateChat += (_) => Task.Run(() => { Console.WriteLine("OnDeletePrivateChat"); });
        client.OnNewChannel += (_) => Task.Run(() => { Console.WriteLine("OnNewChannel"); });
        client.OnUpdateChannel += (_) => Task.Run(() => { Console.WriteLine("OnUpdateChannel"); });
        client.OnDeleteChannel += (_) => Task.Run(() => { Console.WriteLine("OnDeleteChannel"); });
        //
        client.OnUpdateUser += (_) => Task.Run(() => { Console.WriteLine("OnUpdateUser"); });
        client.OnUpdateUserOnline += (_) => Task.Run(() => { Console.WriteLine("OnUpdateUserOnline"); });
        //
        client.OnNewIncomingFriendRequest += (_) => Task.Run(() => { Console.WriteLine("OnNewIncomingFriendRequest"); });
        client.OnDeleteIncomingFriendRequest += (_) => Task.Run(() => { Console.WriteLine("OnDeleteIncomingFriendRequest"); });
        client.OnNewOutgoingFriendRequest += (_) => Task.Run(() => { Console.WriteLine("OnNewOutgoingFriendRequest"); });
        client.OnDeleteOutgoingFriendRequest += (_) => Task.Run(() => { Console.WriteLine("OnDeleteOutgoingFriendRequest"); });
        client.OnNewFriend += (_) => Task.Run(() => { Console.WriteLine("OnNewFriend"); });
        client.OnDeleteFriend += (_) => Task.Run(() => { Console.WriteLine("OnDeleteFriend"); });
        //
        client.OnGuildJoin += (_) => Task.Run(() => { Console.WriteLine("OnGuildJoin"); });
        client.OnGuildLeave += (_) => Task.Run(() => { Console.WriteLine("OnGuildLeave"); });
        client.OnGuildUpdate += (_) => Task.Run(() => { Console.WriteLine("OnGuildUpdate"); });
        //
        client.OnNewGuildInvite += (_) => Task.Run(() => { Console.WriteLine("OnNewGuildInvite"); });
        client.OnDeleteGuildInvite += (_) => Task.Run(() => { Console.WriteLine("OnDeleteGuildInvite"); });
        //
        client.OnNewMember += (_) => Task.Run(() => { Console.WriteLine("OnNewMember"); });
        client.OnDeleteMember += (_) => Task.Run(() => { Console.WriteLine("OnDeleteMember"); });
        client.OnBanMember += (_) => Task.Run(() => { Console.WriteLine("OnBanMember"); });
        client.OnUnbanMember += (_) => Task.Run(() => { Console.WriteLine("OnUnbanMember"); });
        while (true)
        {
        }
    }
}