using LoCyanFrpDesktop.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace LoCyanFrpDesktop.Dashboard
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UiPage
    {
        public Settings()
        {
            InitializeComponent();
            Access.Settings = this;
            _Version.Text = $"版本: Ver {Global.Version}-{Global.Branch}{Global.Revision}";
            _BuildInfo.Text = Global.BuildInfo.ToString();
            _Developer.Text = $"开发者: {Global.Developer}";
            _Copyright.Text = Global.Copyright;
            FrpcPath.Text = Properties.Settings.Default.FrpcPath;
        }
        public void Select_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = "支持的文件(frpc.exe)|frpc.exe"
            };
            if (dialog.ShowDialog() ?? false)
            {
                FrpcPath.Text = dialog.FileName;
                Properties.Settings.Default.FrpcPath = FrpcPath.Text;

            }
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            Access.DashBoard.Close();
            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session.token"))) {
                File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session.token"));

            }
            
            Properties.Settings.Default.FrpToken = null;
            Properties.Settings.Default.LoginToken = null;
            Properties.Settings.Default.password = null;
            MainWindow.islogin = false;
            Access.MainWindow.Width = double.NaN;
            Access.MainWindow.Show();
        }
    }
}
