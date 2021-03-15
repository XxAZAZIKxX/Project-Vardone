using VardoneEntities.Entities;

namespace VardoneLibrary.VardoneEvents
{
    public class VardoneDelegates
    {
        public delegate void NewPrivateMessageHandler(PrivateMessage message);

        public delegate void UpdateUserHandler(User user);

        public delegate void UpdateOnlineHandler(User user);

        public delegate void UpdateChatListHandler();

        public delegate void UpdateOutgoingFriendRequestListHandler();

        public delegate void UpdateIncomingFriendRequestListHandler();

        public delegate void UpdateFriendListHandler();
    }
}