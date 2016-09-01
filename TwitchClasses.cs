namespace TwitchChatLib
{
    public class UserLinks
    {
        public string self { get; set; }
    }

    public class TwitchUser
    {
        public string display_name { get; set; }
        public int _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string bio { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string logo { get; set; }
        public UserLinks _links { get; set; }
    }
}