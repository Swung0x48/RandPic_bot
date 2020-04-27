using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cqhttp.Cyan.Enums;

namespace RandPic_bot.Misc
{
    public class Timer
    {
        TimeSpan _interval;
        public Timer(TimeSpan interval)
        {
            _interval = interval;
        }
        
        public async Task<bool> Start(List<string> uid, TimeSpan timeSpan, CancellationToken cts)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var i in uid)
                    {
                        string msgToSend = await BilbiliDynamic.Poll(uid.IndexOf(i));
                        
                        if (msgToSend != "")
                        {
                            Console.WriteLine($"{Vars.Uptime}:\n {msgToSend}");

                            await BotInstance.CqHttpClient.SendTextAsync(
                                MessageType.group_,
                                Vars.CurrentConf.QqGroupId,
                                msgToSend
                            );
                        }
                        else
                        {
                            Console.WriteLine($"{Vars.Uptime}:\n No updates. Skipping.");
                        }
                        
                        await Task.Delay(timeSpan / uid.Count, cts);
                        if (cts.IsCancellationRequested)
                            break;
                    }

                    // await Task.Delay(_interval, cts);
                    if (cts.IsCancellationRequested)
                        break;
                }
            }, cts);
            return true;
        }
    }
}