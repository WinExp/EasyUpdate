using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyUpdate.ZipExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(1500);
                    string[] inputArgs = Environment.GetCommandLineArgs();
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (string arg in inputArgs)
                    {
                        stringBuilder.Append(arg);
                        stringBuilder.Append(' ');
                    }
                    string[] args = stringBuilder.ToString().Trim().Split('|');
                    string zipPath = args[1];
                    string extractDirectory = args[2];
                    string startFilePath = args[3];
                    if (!Directory.Exists(extractDirectory))
                    {
                        Directory.CreateDirectory(extractDirectory);
                    }
                    using (ZipInputStream zip = new ZipInputStream(File.OpenRead(zipPath)))
                    {
                        ZipEntry theEntry;
                        while ((theEntry = zip.GetNextEntry()) != null)
                        {
                            string fileName = Path.GetFileName(theEntry.Name);
                            if (fileName != string.Empty)
                            {
                                string directoryName = Path.Combine(extractDirectory, Path.GetDirectoryName(theEntry.Name));
                                if (!Directory.Exists(directoryName))
                                {
                                    Directory.CreateDirectory(directoryName);
                                }
                                using (FileStream streamWriter = File.Create(Path.Combine(extractDirectory, theEntry.Name)))
                                {
                                    int size = 2048;
                                    byte[] data = new byte[2048];
                                    while (true)
                                    {
                                        size = zip.Read(data, 0, data.Length);
                                        if (size > 0)
                                        {
                                            streamWriter.Write(data, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    File.Delete(zipPath);

                    Process.Start(startFilePath);

                    Environment.Exit(0);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($@"文件 {ex.FileName} 不存在。");
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($@"在解压过程中出现错误。
错误信息：{ex.Message}");
                    Environment.Exit(-1);
                }
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("解压任务未完成，请问你确定要关闭吗？", "提示", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
