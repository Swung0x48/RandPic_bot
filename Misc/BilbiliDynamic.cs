using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RandPic_bot.IO;

namespace RandPic_bot.Misc
{
    public class BilbiliDynamic
    {
        public static async Task<string> Poll(int index)
        {
            string uid = Vars.CurrentConf.BilibiliUid[index];
            try
            {
                string rawDynamic = await Networking.MakeHttpRequestAsync(
                    $"https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/space_history" +
                    $"?host_uid={uid}" +
                    $"&offset_dynamic_id=0" +
                    $"&page=0"
                );

                
                
                var dynamicObj = JObject.Parse(rawDynamic);
                var firstCard = dynamicObj["data"]?["cards"]?[0];

                var type = firstCard?["desc"]?["type"];
                Console.WriteLine(type);                                                    // get content type

                var dynamicId = firstCard?["desc"]?["dynamic_id_str"]?.ToString();
                var link = $"https://t.bilibili.com/{dynamicId}";
                var timestamp = firstCard?["desc"]?["timestamp"]?.ToString();
                var userProfile = firstCard?["desc"]?["user_profile"]?["info"];
                var username = userProfile?["uname"];
                string rawFirstCardStr = firstCard?["card"]?.ToString();
                var firstCardObj = JObject.Parse(rawFirstCardStr ?? "");

                if (timestamp != Vars.CurrentConf.LastTimeStamp[index])
                {
                    Vars.CurrentConf.LastTimeStamp[index] = timestamp;
                    await IO.File.WriteConf(Vars.ConfFile, Vars.CurrentConf);
                }
                else
                {
                    return "";
                }

                if (type?.ToString() == "1")
                {

                    //Console.WriteLine(firstCardObj);
                    //var username = firstCardObj["user"]?["uname"]?.ToString();
                    var content = firstCardObj["item"]?["content"]?.ToString();

                    content = Chop(content);

                    var rawOrigin = JObject.Parse(firstCardObj["origin"]?.ToString() ?? "");
                    //Console.WriteLine(rawOrigin);
                    var forwardedText = rawOrigin["item"]?["description"]?.ToString();

                    forwardedText = Chop(forwardedText);

                    string output = 
                        $"{link}\n" +
                        $"{username} 转发了一条动态：\n" +
                        $"{content}\n" +
                        $"↪️\n" +
                        $"[{forwardedText}]";
                    // Console.WriteLine(output);
                    return output;
                }
                else if (type?.ToString() == "2")
                {
                    //Console.WriteLine(firstCardObj);
                    
                    //var username = firstCardObj["user"]?["name"]?.ToString();
                    var description = firstCardObj["item"]?["description"]?.ToString();

                    description = Chop(description);

                    string output =
                        $"{link}\n" +
                        $"{username} 发布了一条动态：\n" +
                        $"{description}\n";
                    // Console.WriteLine(output);
                    return output;
                }
                else if (type?.ToString() == "4")
                {
                    //Console.WriteLine(firstCardObj);
                    var content = firstCardObj["item"]?["content"]?.ToString();
                    content = Chop(content);
                    
                    string output =
                        $"{link}\n" +
                        $"{username} 发布了一条动态：\n" +
                        $"{content}";
                    
                    // Console.WriteLine(output);
                    return output;
                }
                else if (type?.ToString() == "8")
                {
                    //Console.WriteLine(firstCardObj);
                    //var username = "";

                    var bvid = firstCard["desc"]?["bvid"]?.ToString();
                    var videoLink = $"https://www.bilibili.com/video/{bvid}";

                    var description = firstCardObj["desc"]?.ToString();
                    description = Chop(description);
                    
                    string output =
                        $"{link}\n" +
                        $"{username} 投稿了一个视频：\n" +
                        $"{description}\n" +
                        $"🎬{videoLink}\n";
                        
                    // Console.WriteLine(output);
                    return output;
                }
                else if (type?.ToString() == "64")
                {
                    var rid = firstCard["desc"]?["rid"]?.ToString();
                    var readLink = $"https://www.bilibili.com/read/cv{rid}";
                    //Console.WriteLine(firstCardObj);
                    var content = firstCardObj["dynamic"]?.ToString();
                    var summary = firstCardObj["summary"]?.ToString();
                    content = Chop(content);
                    summary = Chop(summary);
                        
                    string output =
                        $"{link}\n" +
                        $"{username} 投稿了一篇专栏文章：\n" +
                        $"{content}\n" +
                        $"↪️\n" +
                        $"[{readLink}\n" +
                        $"{summary}]";
                    // Console.WriteLine(output);
                    return output;
                }
                else if (type?.ToString() == "2048")
                {
                    return "";
                }

                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Chop(e.ToString());
            }

            return "";
        }
        
        public static string Chop(string input)
            => (input != null && input.Length > 200)
                ? input.Substring(0, 200) + "..."
                : input;
    }
}