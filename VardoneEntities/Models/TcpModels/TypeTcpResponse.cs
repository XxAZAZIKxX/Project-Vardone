namespace VardoneEntities.Models.TcpModels
{
    public enum TypeTcpResponse
    {
        Connected, Disconnected, Accepted,
        
        NewPrivateMessage, DeletePrivateMessage, 
        NewPrivateChat, DeletePrivateChat, 
        
        UpdateUser, UpdateUserOnline,
        
        NewIncomingFriendRequest, NewOutgoingFriendRequest, NewFriend,
        DeleteIncomingFriendRequest, DeleteOutgoingFriendRequest, DeleteFriend,
        
        GuildJoin, GuildLeave, UpdateGuild, 
        NewChannel, DeleteChannel, UpdateChannel, 
        NewChannelMessage, DeleteChannelMessage,
        NewGuildInvite, DeleteGuildInvite,
        NewMember, DeleteMember, BanMember, UnbanMember
    }
}