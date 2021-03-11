using VardoneEntities.Entities;

namespace VardoneLibrary.Core.Events
{
    public class Delegates
    {
        public delegate void NewPrivateMessageHandler(PrivateMessage message);
    }
}