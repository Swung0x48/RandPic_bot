using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using cqhttp.Cyan.Clients;
using cqhttp.Cyan.Enums;
using cqhttp.Cyan.Events.CQEvents;
using cqhttp.Cyan.Events.CQResponses;
using RandPic_bot.Misc;
using static RandPic_bot.Misc.BilbiliDynamic;
using Timer = RandPic_bot.Misc.Timer;


namespace RandPic_bot {
    
    public class Program {
        
        static async Task Main(string[] args)
        {
            Vars.Stopwatch.Start();                                                // Start stopwatch to record uptime.

            if (!File.Exists(Vars.ConfFile))                                       // Check whether config file exists.
            {
                Console.WriteLine("Config file not found.");
                Console.WriteLine("Seems it's the first boot");
                Console.WriteLine("Enter owner's QQ number:");
                Vars.CurrentConf.OwnerQqNumber = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter pulling interval, in seconds:");
                Vars.CurrentConf.Interval = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter QQ group number:");
                Vars.CurrentConf.QqGroupId = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter CQHTTP url:");
                Vars.CurrentConf.HttpUrl = Console.ReadLine();
                Console.WriteLine("Enter CQHTTP POST port:");
                Vars.CurrentConf.ListenPort = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter Bilibili UID, use comma(,) to separate, enter to finish:");
                var rawUid = Console.ReadLine();
                string[] uid = rawUid?.Split(',');
                if (uid != null)
                    foreach (var i in uid)
                    {
                        Vars.CurrentConf.BilibiliUid.Add(i);
                    }
                await IO.File.WriteConf(Vars.ConfFile, Vars.CurrentConf);           // Save conf to file.
            }
            else
            {
                Vars.CurrentConf = await IO.File.ReadConf<Config>(Vars.ConfFile);
            }

            while (Vars.CurrentConf.BilibiliUid.Count != Vars.CurrentConf.LastTimeStamp.Count)
            {
                Vars.CurrentConf.LastTimeStamp.Add("0");
            }
            await IO.File.WriteConf(Vars.ConfFile, Vars.CurrentConf);
            
            try
            {
                await BotInstance.CqHttpClient.SendTextAsync(
                    MessageType.private_,
                    Vars.CurrentConf.OwnerQqNumber,
                    "Started"
                );
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
            BotInstance.CqHttpClient.OnEventAsync += async (client, e) => 
            {
                Console.Write("Rcvd msg: ");
                if (e is GroupMessageEvent me)
                {
                    Console.WriteLine(me.message);
                }
                return new EmptyResponse();
            };
            
            var cts = new CancellationToken();
            var interval = TimeSpan.FromSeconds(Vars.CurrentConf.Interval);
            
            Timer timer = new Timer(interval);
            await timer.Start(
                Vars.CurrentConf.BilibiliUid,
                interval,
                cts);
            
            Console.ReadLine();
        }
    }
}