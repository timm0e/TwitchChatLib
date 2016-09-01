using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitchChatLib
{
    public class Chat
    {
        private const string newline = "\r\n";

        private readonly string _username;
        private readonly string _password;
        private readonly string _channel;

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;

        private readonly UserMap _userMap;

        private readonly MessageStore _messageStore;

        public readonly TwitchUser Me;

        public Chat(string username, string password, string channel, bool login = true)
        {
            _username = username.ToLower();
            _password = password;
            _channel = channel.ToLower();

            Me = TwitchAPI.getTwitchUser(_username);

            _messageStore = new MessageStore();

            _tcpClient = new TcpClient("irc.twitch.tv", 6667);
            _networkStream = _tcpClient.GetStream();

            _userMap = new UserMap();

            if (login)
            {
                Login();
            }
        }

        public void Login()
        {
            Sendcmd("PASS " + _password);
            Sendcmd("NICK " + _username); //Login
            Sendcmd("CAP REQ :twitch.tv/membership"); //Request Membership
            Sendcmd("JOIN #" + _channel); //Join Channel
        }

        private static byte[] encode(string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        private static string Decode(byte[] input)
        {
            return Encoding.ASCII.GetString(input);
        }

        private void Sendcmd(string message)
        {
            var data = encode(message + newline);
            _networkStream.Write(data, 0, data.Length);
        }

        public void SendMessage(string message)
        {
            Sendcmd(String.Format(":{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #{1} :{2}", _username, _channel, message));
                //Channel
        }

        public void Tick()
        {
            if (_networkStream.DataAvailable)
            {
                StringBuilder stringBuilder = new StringBuilder();

                while (_networkStream.DataAvailable)
                {
                    byte[] data = new byte[1024];
                    _networkStream.Read(data, 0, data.Length);
                    stringBuilder.Append(Decode(data));
                }

                string raw = stringBuilder.ToString();
                stringBuilder.Clear();


#if DEBUG

                foreach (Match match in Regex.Matches(raw, "^(.*)$", RegexOptions.Multiline))
                {
                    Debug.WriteLine(match.Groups[1]);
                }
#endif

                //Catch Messages

                foreach (
                    Match match in
                        Regex.Matches(raw, "^:(.*)!.*@.*\\.tmi\\.twitch\\.tv PRIVMSG #.* :(.*)$", RegexOptions.Multiline)
                    )
                {
                    _messageStore.AddMessage(new ChatMessage(_userMap.GetTwitchUser(match.Groups[1]), match.Groups[2]));
                }


                foreach (
                    Match match in
                        Regex.Matches(raw, "^:(.*)!.*@.*\\.tmi\\.twitch\\.tv JOIN #.*$", RegexOptions.Multiline))
                {
                    _messageStore.AddMessage(new JoinPartMessage(_userMap.GetTwitchUser(match.Groups[1]), true));
                }

                foreach (
                    Match match in
                        Regex.Matches(raw, "^:(.*)!.*@.*\\.tmi\\.twitch\\.tv PART #.*$", RegexOptions.Multiline))
                {
                    _messageStore.AddMessage(new JoinPartMessage(_userMap.GetTwitchUser(match.Groups[1]), false));
                    _userMap.RemoveUser(match.Groups[1]);
                }


                if (Regex.Matches(raw, "PING :tmi\\.twitch\\.tv", RegexOptions.Multiline).Count > 0)
                {
                    Sendcmd("PONG :tmi.twitch.tv");
                    _messageStore.AddMessage(new PingMessage());
                }
            }
        }

        private string NameResolve(Group name)
        {
            return _userMap.GetTwitchUser(name.Value).display_name;
        }

        public bool NewMessagesAvailible()
        {
            return _messageStore.NewMessagesAvailible();
        }

        public List<Message> GetMessages()
        {
            return _messageStore.GetMessages();
        }

        public List<TwitchUser> GetUsers()
        {
            return _userMap.GetUsers();
        }
    }
}

/* Dictionary name -> User              OK
 * Regex Fixen                          OK
 * Objekt für Messages                  OK
 * Objekt für User                      OK
 * Cache für Überlauf                   OK
 * Channel unabhängig machen            OK
 * 
 * 
*/