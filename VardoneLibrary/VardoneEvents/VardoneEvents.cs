namespace VardoneLibrary.VardoneEvents
{
    public static class VardoneEvents
    {
        public static VardoneDelegates.NewPrivateMessageHandler onNewPrivateMessage;

        public static VardoneDelegates.NewChannelMessageHandler onNewChannelMessage;

        public static VardoneDelegates.UpdateUserHandler onUpdateUser;
        
        public static VardoneDelegates.UpdateChatListHandler onUpdateChatList;
        
        public static VardoneDelegates.UpdateFriendListHandler onUpdateFriendList;
        
        public static VardoneDelegates.UpdateIncomingFriendRequestListHandler onUpdateIncomingFriendRequestList;
        
        public static VardoneDelegates.UpdateOutgoingFriendRequestListHandler onUpdateOutgoingFriendRequestList;
        
        public static VardoneDelegates.UpdateOnlineHandler onUpdateOnline;
        
        public static VardoneDelegates.UpdateGuildListHandler onUpdateGuildList;
        
        public static VardoneDelegates.UpdateChannelListHandler onUpdateChannelList;

        public static VardoneDelegates.DeleteChannelMessageHandler onDeleteChannelMessage;

        public static VardoneDelegates.DeletePrivateChatMessageHandler onDeletePrivateChatMessage;
    }
}