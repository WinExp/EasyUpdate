using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            UpdateAsync().Wait();
        }

        private static async Task UpdateAsync()
        {
            var updateInfo = await EasyUpdate.Updater.GetUpdateInfoAsync("https://we-bucket.oss-cn-shenzhen.aliyuncs.com/App/Test/updateInfo.xml");
            if (updateInfo.IsUpdateAvailable)
            {
                Console.Write($"有更新，版本 {updateInfo.Version}，当前版本 {Assembly.GetEntryAssembly().GetName().Version}，请问是否更新？(Y)");
                if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
                {
                    (await EasyUpdate.Updater.DownloadUpdate(updateInfo)).Wait();
                    await EasyUpdate.Updater.StartUpdateAsync(updateInfo);
                }
            }
        }
    }
}
