using System.Text.RegularExpressions;

namespace TwitchChatLib
{
    public interface Message
    {
    }


    public class ChatMessage : Message
    {
        public TwitchUser Sender { get; private set; }
        public string Message { get; private set; }


        public ChatMessage(TwitchUser sender, Group message)
        {
            Sender = sender;
            Message = message.ToString();
        }
    }

    public class JoinPartMessage : Message
    {
        public TwitchUser User { get; private set; }
        public bool Joined { get; private set; }

        public JoinPartMessage(TwitchUser user, bool joined)
        {
            this.User = user;
            this.Joined = joined;
        }
    }

    public class PingMessage : Message
    {
        public PingMessage()
        {
        }
    }
}