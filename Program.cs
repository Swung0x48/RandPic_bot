using System;
using cqhttp.Cyan;
using cqhttp.Cyan.Clients;
using cqhttp.Cyan.Enums;
using cqhttp.Cyan.Events.CQResponses;
using RandPic_bot.Misc;


namespace RandPic_bot {
    class Program {
        static void Main(string[] args)
        {
            Vars.Stopwatch.Start();
            
            CQApiClient instance = new CQWebsocketClient("","","");
            
            Console.WriteLine(
                $"QQ:{instance.self_id}\nnickname:{instance.self_nick}"
            );
            
            instance.OnEvent += (client, e) => 
            {
                return new EmptyResponse ();
            };
            
            instance.SendTextAsync(
                MessageType.private_,
                1000000,
                "Hello World!"
            );
            Console.ReadLine();
        }
    }
}