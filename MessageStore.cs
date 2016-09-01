using System.Collections.Generic;

namespace TwitchChatLib
{
    internal class MessageStore
    {
        private readonly List<Message> _chatList;

        public MessageStore()
        {
            _chatList = new List<Message>();
        }

        public void AddMessage(Message message)
        {
            _chatList.Add(message);
        }

        public List<Message> GetMessages()
        {
            List<Message> returnval = _chatList.GetRange(0, _chatList.Count);
            _chatList.Clear();
            return returnval;
        }

        public bool NewMessagesAvailible()
        {
            return _chatList.Count > 0;
        }
    }
}