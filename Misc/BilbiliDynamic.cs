using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
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
                Console.WriteLine($"{type} from {uid}");                                                    // get content type

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
                    try
                    {
                        //Console.WriteLine(firstCardObj);
                        //var username = firstCardObj["user"]?["uname"]?.ToString();
                        var content = firstCardObj["item"]?["content"]?.ToString();
                        var orig_type = firstCardObj["item"]?["orig_type"]?.ToString();
                        Console.WriteLine(orig_type);
                        
                        content = Chop(content);

                        var rawOrigin = JObject.Parse(firstCardObj["origin"]?.ToString() ?? "");
                        Console.WriteLine(rawOrigin);
                        var forwardedText = rawOrigin["item"]?["description"]?.ToString();
                        var metaString = "\n";

                        if (orig_type == "8")
                        {
                            forwardedText += rawOrigin["dynamic"]?.ToString();
                            var aid = rawOrigin["aid"]?.ToString();            // av number.
                            var vidTitle = rawOrigin["title"]?.ToString();
                            var uperName = rawOrigin["owner"]?["name"]?.ToString();
                            metaString += $"\n🎬 {uperName} 的投稿视频\n" +
                                             $"{vidTitle}\n" +
                                             $"https://www.bilibili.com/video/av{aid}";
                        }


                        forwardedText = Chop(forwardedText);            // TODO : Handle other forward origin types.

                        string output =
                            $"{link}\n" +
                            $"{username} 转发了一条动态：\n" +
                            $"{content}\n" +
                            $"↪️\n" +
                            $"[{forwardedText}" +
                            $"{metaString}]";
                        // Console.WriteLine(output);
                        Console.WriteLine(output);
                        return output;
                    }
                    catch (Exception ex)
                    {
                        string output = $"Exception when parsing item: Type 1 (Forward)\n\n" +
                                        $"{ex}";
                        
                        Console.WriteLine(output);
                        return output;
                    }
                }
                else if (type?.ToString() == "2")
                {
                    try
                    {
                        //Console.WriteLine(firstCardObj);

                        //var username = firstCardObj["user"]?["name"]?.ToString();
                        var description = firstCardObj["item"]?["description"]?.ToString();

                        description = Chop(description);

                        string output =
                            $"{link}\n" +
                            $"{username} 发布了一条动态：\n" +
                            $"{description}";
                        // Console.WriteLine(output);
                        return output;
                    }
                    catch (Exception ex)
                    {
                        string output = $"Exception when parsing item: Type 2 (Album)\n\n" +
                                        $"{ex}";
                        
                        Console.WriteLine(output);
                        return output;
                    }
                }
                else if (type?.ToString() == "4")
                {
                    try
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
                    catch (Exception ex)
                    {
                        string output = $"Exception when parsing item: Type 4 (TextDynamic)\n\n" +
                                        $"{ex}";
                        
                        Console.WriteLine(output);
                        return output;
                    }
                }
                else if (type?.ToString() == "8")
                {
                    try
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
                            $"🎬{videoLink}";

                        // Console.WriteLine(output);
                        return output;
                    }
                    catch (Exception ex)
                    {
                        string output = $"Exception when parsing item: Type 8 (Video)\n\n" +
                                        $"{ex}";
                        
                        Console.WriteLine(output);
                        return output;
                    }
                }
                else if (type?.ToString() == "64")
                {
                    try
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
                            $"↪️{readLink}\n" +
                            $"[{summary}]";
                        // Console.WriteLine(output);
                        return output;
                    }
                    catch (Exception ex)
                    {
                        string output = $"Exception when parsing item: Type 64 (Article)\n\n" +
                                        $"{ex}";
                        
                        Console.WriteLine(output);
                        return output;
                    }
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