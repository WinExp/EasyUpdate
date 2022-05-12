using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyUpdate.ZipExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread.Sleep(1500);
            string[] args = Environment.GetCommandLineArgs();
            try
            {
                if (!Directory.Exists(args[2].Trim('\"')))
                {
                    Directory.CreateDirectory(args[2].Trim('\"'));
                }
                ZipFile.ExtractToDirectory(args[1].Trim('\"'), args[2].Trim('\"'));
                try
                {
                    File.Delete(args[1].Trim('\"'));
                }
                catch { }
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
错误信息：{ex}");
                Environment.Exit(-1);
            }
        }
    }
}
