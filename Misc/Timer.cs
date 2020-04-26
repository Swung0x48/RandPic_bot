using System;
using System.Threading;
using System.Threading.Tasks;

namespace RandPic_bot.Misc
{
    public class Timer
    {
        TimeSpan _interval;
        public Timer(TimeSpan interval)
        {
            _interval = interval;
        }
        
        
        public async Task<bool> Start(Task<bool> task, CancellationToken cancellationToken)
        {
            
            await Task.Run(async () =>
            {
                while (true)
                {
                    task.Start();    
                    await Task.Delay(_interval, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            }, cancellationToken);
            return task.Result;
        }
    }
}