using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.IO;
using LoCyanFrpDesktop.Utils;
using System.Windows.Threading;

namespace LoCyanFrpDesktop
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {   
        protected override void OnStartup(StartupEventArgs e)
        {
            CrashInterception.Init();
            base.OnStartup(e);

            DispatcherUnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += (_, e) => CrashInterception.ShowException(e.Exception);
            
            // 处理启动参数
            string[] args = e.Args;
            if (args.Length > 0)
            {
                // 解析和处理参数
                // 这里可以根据参数的内容执行不同的操作
                ProcessUrlParameters(args);
            }
        }
        private static void CurrentDomain_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
        private void ProcessUrlParameters(string[] args)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, "auto_launch.ini");

            string arguments = string.Join(" ", args);

            try
            {
                File.WriteAllText(filePath, arguments);
            }
            catch (Exception ex)
            {
                // 处理写入文件时可能发生的异常
                MessageBox.Show("写入文件时出现错误：" + ex.Message);
            }
        }
    }
}
