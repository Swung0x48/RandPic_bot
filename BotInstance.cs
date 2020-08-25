using BilibiliForwarder_bot.Misc;
using cqhttp.Cyan.Clients;

namespace BilibiliForwarder_bot
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