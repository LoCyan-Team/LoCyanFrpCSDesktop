using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

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
            _Version.Text = $"版本: Ver {Global.Version}-{Global.Branch}{Global.Revision}";
            _BuildInfo.Text = Global.BuildInfo.ToString();
            _Developer.Text = $"开发者: {Global.Developer}";
            _Copyright.Text = Global.Copyright;
            
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
    }
}
