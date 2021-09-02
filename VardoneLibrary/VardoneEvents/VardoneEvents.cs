namespace VardoneLibrary.VardoneEvents
{
    public static class VardoneEvents
    {
        /// <summary>
        /// При новом сообщении текущему пользователю
        /// </summary>
        public static VardoneDelegates.NewPrivateMessageHandler onNewPrivateMessage;
        /// <summary>
        /// При обновлении пользователя
        /// </summary>
        public static VardoneDelegates.UpdateUserHandler onUpdateUser;
        /// <summary>
        /// При обновлении списка чатов
        /// </summary>
        public static VardoneDelegates.UpdateChatListHandler onUpdateChatList;
        /// <summary>
        /// При обновлении списка друзей
        /// </summary>
        public static VardoneDelegates.UpdateFriendListHandler onUpdateFriendList;
        /// <summary>
        /// При обновлении списка входящих запросов в друзья
        /// </summary>
        public static VardoneDelegates.UpdateIncomingFriendRequestListHandler onUpdateIncomingFriendRequestList;
        /// <summary>
        /// При обновлении списка исходящих запросов в друзья 
        /// </summary>
        public static VardoneDelegates.UpdateOutgoingFriendRequestListHandler onUpdateOutgoingFriendRequestList;
        /// <summary>
        /// При обновлении статуса
        /// </summary>
        public static VardoneDelegates.UpdateOnlineHandler onUpdateOnline;
        /// <summary>
        /// При обновлении в списке серверов
        /// </summary>
        public static VardoneDelegates.UpdateGuildListHandler onUpdateGuildList;

        public static VardoneDelegates.UpdateChannelListHandler onUpdateChannelList;
    }
}