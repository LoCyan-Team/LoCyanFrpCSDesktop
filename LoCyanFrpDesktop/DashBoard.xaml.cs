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

namespace LoCyanFrpDesktop
{
    /// <summary>
    /// DashBoard.xaml 的交互逻辑
    /// </summary>
    public partial class DashBoard : UiWindow
    {
        bool isFrpcInstalled;
        public static Snackbar Snackbar = new Snackbar();
        /// <summary>
        /// 
        /// </summary>
        public DashBoard()
        {
            InitializeComponent();
            Uri iconUri = new Uri("pack://application:,,,/LoCyanFrpDesktop;component/Resource/favicon.ico", UriKind.RelativeOrAbsolute);
            this.Icon = new BitmapImage(iconUri);
            isFrpcInstalled = CheckIfFrpcInstalled();
            Access.DashBoard = this;
        }
        private bool CheckIfFrpcInstalled()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.FrpcPath))
            {
                if (!File.Exists(Properties.Settings.Default.FrpcPath))
                {
                    File.Create(Path.Combine(Directory.GetDirectoryRoot(Properties.Settings.Default.FrpcPath), "FrpcPath.conf"));
                    return false;
                }
            }
            else { return false; }
            
            return false;
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
