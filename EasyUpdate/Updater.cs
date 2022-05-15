using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
            updateInfo.IsUpdateAvailable = Version.Parse(updateInfo.Version) > Assembly.GetEntryAssembly().GetName().Version;
            return updateInfo;
        }

        public static async Task<DownloadInfo> DownloadUpdate(UpdateInfo updateInfo)
        {
            return await WebRequests.DownloadFile(updateInfo.Url.Url, "downloads");
        }

        public static async Task StartUpdateAsync(UpdateInfo updateInfo)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "EasyUpdate");
            (await WebRequests.DownloadFile("https://we-bucket.oss-cn-shenzhen.aliyuncs.com/Project/Download/EasyUpdate/ZipExtractor/EasyUpdate.ZipExtractor.exe",
    tempPath)).Wait();
            string startFilePath = Process.GetCurrentProcess().MainModule.FileName;
            string extractPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads"), Path.GetFileName(HttpUtility.UrlDecode(updateInfo.Url.Url)));
            string arguments = '|' + extractPath +
                '|' + AppDomain.CurrentDomain.BaseDirectory +
                '|' + startFilePath;
            Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = Path.Combine(tempPath, "EasyUpdate.ZipExtractor.exe"),
                Arguments = arguments
            });
            Exit();
        }

        private static async Task Exit()
        {
            await Task.Delay(0);
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
                    continue;
                }

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
