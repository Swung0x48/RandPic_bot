using cqhttp.Cyan.Clients;
using RandPic_bot.Misc;

namespace RandPic_bot
{
    public class BotInstance
    {
        public static CQHTTPClient CqHttpClient = 
            new CQHTTPClient
            (
                Vars.CurrentConf.HttpUrl, 
                Vars.CurrentConf.AccessToken, 
                Vars.CurrentConf.ListenPort
            );
    }
}