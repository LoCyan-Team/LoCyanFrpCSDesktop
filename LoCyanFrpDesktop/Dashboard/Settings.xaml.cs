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
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace LoCyanFrpDesktop.Dashboard
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UiPage
    {
        private static int i = 0;
        public Settings()
        {   

            InitializeComponent();
            Access.Settings = this;
            _Version.Text = $"版本: Ver {Global.Version}-{Global.Branch}{Global.Revision}";
            _BuildInfo.Text = Global.BuildInfo.ToString();
            _Developer.Text = $"开发者: {Global.Developer}";
            _Copyright.Text = Global.Copyright;
            FrpcPath.Text = Global.Config.FrpcPath;
            AppliedTheme.SelectedIndex = Global.Config.AppliedTheme;
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
                Global.Config.FrpcPath = FrpcPath.Text;

            }
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            if (!Logger.MsgBox("您确定要退出登录吗?", "退出登录", 1, 47, 1))
            {
                return;
            }
            Access.DashBoard.Close();
            
            Global.Config.FrpToken = null;
            Global.Config.Token = null;
            Global.Config.LoginToken = null;
            Global.Password = null;
            new ConfigManager(FileMode.Create);
            MainWindow.islogin = false;
            Access.MainWindow.Width = double.NaN;
            Access.MainWindow.Show();
        }


        private void AppliedTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if(i > 0)
            {
                Global.Config.AppliedTheme = AppliedTheme.SelectedIndex;
                switch (AppliedTheme.SelectedIndex)
                {
                    case 0:
                        MainWindow.IsDarkThemeEnabled();

                        break;
                    case 1:
                        Global.isDarkThemeEnabled = true;
                        Theme.Apply(ThemeType.Dark);
                        break;
                    case 2:
                        Global.isDarkThemeEnabled = false;
                        Theme.Apply(ThemeType.Light);
                        break;
                    default:
                        throw new IndexOutOfRangeException();

                }
                Access.DashBoard.ChangeColor();
            }
            i++;
        }
    }
}
