using System.Collections.Generic;

namespace BilibiliForwarder_bot.Misc
{
    public class Config
    {
        public long OwnerQqNumber;
        public List<string> BilibiliUid = new List<string>();
        public string WsUrl;
        public string EventUrl;
        
        public string HttpUrl;
        public string AccessToken = "";
        public int ListenPort;

        public List<string> LastTimeStamp = new List<string>();
        public long QqGroupId;
        public int Interval;
    }
}