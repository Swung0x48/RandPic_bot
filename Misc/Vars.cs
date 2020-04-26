using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Timers;
using cqhttp.Cyan.Clients;

namespace RandPic_bot.Misc
{
    public class Vars
    {
        public static Stopwatch Stopwatch = new Stopwatch();

        
        

        public readonly static string AppExecutable = 
            Assembly.GetExecutingAssembly().Location;
        public readonly static string AppDirectory = 
            (new FileInfo(AppExecutable)).DirectoryName;
        public static string ConfFile = 
            Path.Combine(AppDirectory, "config.json");
        public static string LangFile = 
            Path.Combine(AppDirectory, "locale.json");
        
        public static Config CurrentConf = new Config();

        public static string Uptime => Stopwatch.Elapsed.ToString();
        
        
    }
}