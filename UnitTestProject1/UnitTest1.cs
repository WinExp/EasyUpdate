using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GetUpdateInfoAsync()
        {
            var updateInfo = await EasyUpdate.Updater.GetUpdateInfoAsync("https://we-bucket.oss-cn-shenzhen.aliyuncs.com/App/Test/updateInfo.xml");
        }

        [TestMethod]
        public async Task StartUpdateAsync()
        {
            var updateInfo = await EasyUpdate.Updater.GetUpdateInfoAsync("https://we-bucket.oss-cn-shenzhen.aliyuncs.com/App/Test/updateInfo.xml");
            (await EasyUpdate.Updater.DownloadUpdate(updateInfo)).Wait();
            await EasyUpdate.Updater.StartUpdateAsync(updateInfo);
        }
    }
}
