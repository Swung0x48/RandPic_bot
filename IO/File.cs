using System;
using System.Text;
using System.Threading.Tasks;
using BilibiliForwarder_bot.Misc;
using Newtonsoft.Json;

namespace BilibiliForwarder_bot.IO
{
    public class File
    {
        public static async Task WriteConf(string filename, object obj)
        {
            try
            {
                await System.IO.File.WriteAllTextAsync(
                    filename, 
                    JsonConvert.SerializeObject(obj, Formatting.Indented)
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task<T> ReadConf<T>(string filename, bool isAutoSave = false)
        {
            try
            {
                if (isAutoSave)
                {
                    Console.WriteLine("Auto saving configuration file...");
                }

                return JsonConvert.DeserializeObject<T>(
                    await System.IO.File.ReadAllTextAsync(
                        filename,
                        Encoding.UTF8
                    )
                );
            }
            catch (Exception e)
            {
                Console.WriteLine("Config file seems corrupted. A new one will be created.");
                System.IO.File.Delete(filename);
                await System.IO.File.WriteAllTextAsync(
                    filename, 
                    JsonConvert.SerializeObject(Vars.CurrentConf, Formatting.Indented)
                );
                
                return JsonConvert.DeserializeObject<T>(
                    await System.IO.File.ReadAllTextAsync(
                        filename,
                        Encoding.UTF8
                    )
                );
            }
        }
    }
}