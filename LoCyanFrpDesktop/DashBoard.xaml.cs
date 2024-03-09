using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Diagnostics;
using Wpf.Ui.Controls;
using LoCyanFrpDesktop;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using LoCyanFrpDesktop.Utils;
using Wpf.Ui.Common;
using Microsoft.VisualBasic;

namespace LoCyanFrpDesktop
{
    /// <summary>
    /// DashBoard.xaml 的交互逻辑
    /// </summary>
    public partial class DashBoard : UiWindow
    {
        public static bool isFrpcInstalled;
        //public static Snackbar Snackbar = new Snackbar();
        public static string FrpcPathConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FrpcPath.conf");
        public double originalCenterX;
        public double originalCenterY;
        /// <summary>
        /// 
        /// </summary>
        public DashBoard()
        {
            InitializeComponent();
            Uri iconUri = new Uri("pack://application:,,,/LoCyanFrpDesktop;component/Resource/favicon.ico", UriKind.RelativeOrAbsolute);
            this.Icon = new BitmapImage(iconUri);
            Access.DashBoard = this;
        }
        public bool CheckIfFrpcInstalled()
        {
            if (!File.Exists(FrpcPathConfig))
            {
                File.Create(Path.Combine(FrpcPathConfig));
                bool isConfirmed = Logger.MsgBox("您需要我们自动为您安装frpc吗?", "未检测到您安装的frpc", 1, 47, 1);
                if(isConfirmed)
                {
                    DownloadFrpc();
                }
                return false;
            }else
            {
                StreamReader PathConfig = new StreamReader(FrpcPathConfig);
                string line = PathConfig.ReadLine();
                PathConfig.Close();
                if (line != null)
                {
                    Properties.Settings.Default.FrpcPath = line;
                    return true;
                }
                bool isConfirmed = Logger.MsgBox("您需要我们自动为您安装frpc吗?", "未检测到您安装的frpc", 1, 47, 1);
                if (isConfirmed)
                {
                    DownloadFrpc();
                }
                return false;
            }
            
            return false;
        }
        public void DownloadFrpc()
        {
            //double screenWidth = SystemParameters.WorkArea.Width;
            //double screenHeight = SystemParameters.WorkArea.Height;

            // 获取原窗口的中心点
            //originalCenterX = Owner.Left + Owner.Width / 2;
            //originalCenterY = Owner.Top + Owner.Height / 2;
            //Left = originalCenterX - Width / 2;
            //Top = originalCenterY - Height / 2;
            Download downloader = new Download();
            downloader.Owner = this;
            downloader.Show();
        }
        public void OpenSnackbar(string title, string message, SymbolRegular icon)
        {
            Dispatcher.Invoke(() =>
            {   
                Snackbar.Show(title, message, icon);
            });
        }
        private void UiWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowInTaskbar = false;
            Hide();
        }
        public void UiWindow_StateChanged(object sender, EventArgs e)
        {/*
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth; */
        }
        public void UiWindow_ContentRendered(object sender, EventArgs e)
        {

        }
        public void UiWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            => ShowInTaskbar = IsVisible;
        public void Hide_Click(object sender, RoutedEventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
        }

        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        public void UiWindow_Loaded(object sender, EventArgs e)
        {
            Resources["ShadowColor"] = MainWindow.DarkThemeEnabled ? Colors.White : Colors.LightGray; ;
            Resources["MainBackgroundColor"] = new SolidColorBrush(MainWindow.DarkThemeEnabled ? Colors.LightGray : Colors.WhiteSmoke);
        }
        
        
    }
}
