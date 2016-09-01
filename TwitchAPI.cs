using System;
using System.Net;
using Newtonsoft.Json;

namespace TwitchChatLib
{
    internal static class TwitchAPI
    {
        public static TwitchUser getTwitchUser(string username)
        {
            WebClient webClient = new WebClient();

            string apiaddress = String.Format("https://api.twitch.tv/kraken/users/{0}", username);

            return JsonConvert.DeserializeObject<TwitchUser>(webClient.DownloadString(apiaddress));
        }
    }
}