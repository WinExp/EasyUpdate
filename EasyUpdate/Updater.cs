using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace EasyUpdate
{
    public static class Updater
    {
        public static async Task<UpdateInfo> GetUpdateInfoAsync(string url)
        {
            UpdateInfo updateInfo = (UpdateInfo)new XmlSerializer(typeof(UpdateInfo))
                .Deserialize(new StringReader(await WebRequests.GetStringAsync(url)));
            //updateInfo.IsUpdateAvailable = Version.Parse(updateInfo.Version) > Assembly.GetEntryAssembly().GetName().Version;
            return updateInfo;
        }

        public static async Task<DownloadInfo> DownloadUpdate(UpdateInfo updateInfo)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "EasyUpdate");
            return await WebRequests.DownloadFile(updateInfo.Url.Url, tempPath);
        }

        public static async Task StartUpdateAsync(UpdateInfo updateInfo, bool AutoExit = true)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "EasyUpdate");
            (await WebRequests.DownloadFile("https://we-bucket.oss-cn-shenzhen.aliyuncs.com/Project/Download/EasyUpdate/ZipExtractor/EasyUpdate.ZipExtractor.exe",
    tempPath)).Wait();
            Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = Path.Combine(tempPath, "EasyUpdate.ZipExtractor.exe"),
                Arguments = $"\"{Path.Combine(tempPath, Path.GetFileName(HttpUtility.UrlDecode(updateInfo.Url.Url)))}\" \"{AppDomain.CurrentDomain.BaseDirectory}\""
            });
            if (AutoExit)
            {
                Exit();
            }
        }

        private static void Exit()
        {
            var currentProcess = Process.GetCurrentProcess();
            foreach (var process in Process.GetProcessesByName(currentProcess.ProcessName))
            {
                string processPath;
                try
                {
                    processPath = process.MainModule?.FileName;
                }
                catch (Win32Exception)
                {
                    // Current process should be same as processes created by other instances of the application so it should be able to access modules of other instances. 
                    // This means this is not the process we are looking for so we can safely skip this.
                    continue;
                }

                //get all instances of assembly except current
                if (process.Id != currentProcess.Id && currentProcess.MainModule?.FileName == processPath)
                {
                    if (process.CloseMainWindow())
                    {
                        process.WaitForExit(1000);
                    }

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
            }
        }
    }
}
