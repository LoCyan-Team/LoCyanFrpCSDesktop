﻿using System;
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
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Path = System.IO.Path;
using System.Security;
using MessageBox = HandyControl.Controls.MessageBox;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Appearance;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Media.Effects;
using LoCyanFrpDesktop.Utils;
using HandyControl.Tools.Extension;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LoCyanFrpDesktop
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public static bool DarkThemeEnabled;
        private InfoResponseObjectt UserInfo;
        string username_auto;
        string token_auto;
        private static bool islogin = false;
        public static DashBoard DashBoard;
        //public static Snackbar Snackbar = new Snackbar();
        public MainWindow()
        {
            
            InitializeComponent();
            Uri iconUri = new Uri("pack://application:,,,/LoCyanFrpDesktop;component/Resource/favicon.ico", UriKind.RelativeOrAbsolute);
            this.Icon = new BitmapImage(iconUri);
            if(Properties.Settings.Default.username != null && Properties.Settings.Default.password != null)
            {
                Login(Properties.Settings.Default.username, Properties.Settings.Default.password);
            }
            InitializeAutoLogin();
            DataContext = this;
            Access.MainWindow = this;
            
        }

        public void OpenSnackbar(string title, string message, SymbolRegular icon)
        {
            Dispatcher.Invoke(() =>
                {
                    
                    Snackbar.Show(title, message, icon);
                });
        }

        private async void InitializeAutoLogin()
        {
            islogin = await CheckLogined();
            if (islogin)
            {
                Properties.Settings.Default.LoginToken = token_auto;
                Properties.Settings.Default.username = username_auto;
                Properties.Settings.Default.FrpToken = UserInfo.Token;
                DashBoard = new DashBoard();
                DashBoard.Show();
                Close();
                Access.DashBoard.CheckIfFrpcInstalled();
            }
        }

        private async Task<bool> CheckLogined()
        {
            string path = ".//session.token";
            if (!File.Exists(path))
            {
                return false;
            }
            else
            {
                string[] token_split;
                try
                {
                    char[] delimiters = { '|' };
                    token_split = File.ReadAllText(path).Split(delimiters);
                    username_auto = token_split[0];
                    token_auto = token_split[1];
                }
                catch
                {
                    return false;
                }
                using (var HttpClient = new HttpClient())
                {
                    string url = $"https://api.locyanfrp.cn/Account/info?username={username_auto}&token={token_auto}";
                    HttpResponseMessage response = await HttpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var InfoResponseObjectt = JsonConvert.DeserializeObject<InfoResponseObjectt>(jsonString);
                    UserInfo = InfoResponseObjectt;
                    if (InfoResponseObjectt.Status == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            SecureString secure_password = Password.SecurePassword;
            string password = ConvertToUnsecureString(secure_password);
            await Login(username, password);
            
        }
        public async Task Login(string username,string password)
        {
            // 使用密码，例如验证或其他操作
            if (string.IsNullOrEmpty(password))
            {
                Logger.MsgBox("解析密码的过程中发生错误, 请联系开发者!", "错误", 0, 48, 0);
                return;
            }


            if (username != "" && password != "")
            {
                using (var httpClient = new HttpClient())
                {
                    string url = $"https://api.locyanfrp.cn/User/DoLogin?username={username}&password={password}";
                    try
                    {
                        // 发起 GET 请求并获取响应
                        HttpResponseMessage response = await httpClient.GetAsync(url);

                        // 确保请求成功
                        response.EnsureSuccessStatusCode();

                        // 将 JSON 数据读取为字符串
                        string jsonString = await response.Content.ReadAsStringAsync();

                        // 将 JSON 字符串反序列化为对象
                        var responseObject = JsonConvert.DeserializeObject<ResponseObject>(jsonString);

                        if (responseObject.Status != 0)
                        {
                            Logger.MsgBox("账号或密码错误！", "警告", 0, 48, 0);
                        }
                        else
                        {
                            Logger.MsgBox($"登录成功\n获取到登录Token: {responseObject.Token}", "提示", 0, 47, 0);
                            Properties.Settings.Default.LoginToken = responseObject.Token;
                            Properties.Settings.Default.username = responseObject.UserData.Username;
                            Properties.Settings.Default.FrpToken = responseObject.UserData.FrpToken;
                            string path = ".//session.token";
                            string text = $"{responseObject.UserData.Username}|{responseObject.Token}";
                            File.WriteAllText(path, text);
                            DashBoard = new DashBoard();
                            DashBoard.Show();
                            Close();
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.MsgBox($"请求API的过程中出错 \n 报错信息: {ex.Message}", "错误", 0, 48, 0);
                    }
                }
            }
            else
            {
                Logger.MsgBox("用户名 / 密码不能为空!", "警告", 0, 48, 0);
            }
        }

        // 将 SecureString 转化为 string
        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return System.Runtime.InteropServices.Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        private void Register_Navigate(object sender, RequestNavigateEventArgs e)
        {
            var url = e.Uri.ToString();
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
        private void ForgetPassword_Navigate(object sender, RequestNavigateEventArgs e)
        {
            var url = e.Uri.ToString();
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
        

        public bool IsDarkThemeEnabled()
        {
        const string RegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        // 从注册表中获取“AppsUseLightTheme”值
        int value = (int)Registry.GetValue(RegistryKey, "AppsUseLightTheme", 1);

        // 如果值为0，则深色主题已启用
        return value == 0;
        }
        public void UiWindow_Loaded(object sender, RoutedEventArgs e)
        { /*
            Catalog.Notification ??= new();
            if (Global.Settings.Serein.ThemeFollowSystem)
            {
                Watcher.Watch(this, BackgroundType.Tabbed, true);
            }
            Theme.Apply(Global.Settings.Serein.UseDarkTheme ? ThemeType.Dark : ThemeType.Light);*/
            DarkThemeEnabled = IsDarkThemeEnabled();
            //DarkThemeEnabled = false;
            Theme.Apply(DarkThemeEnabled ? ThemeType.Dark : ThemeType.Light, WindowBackdropType = BackgroundType.Mica);
            
            //MainForm.Background = new SolidColorBrush(DarkThemeEnabled ? Colors.LightGray : Colors.WhiteSmoke);
            Color newColor = DarkThemeEnabled ? Colors.White : Colors.LightGray;
            Resources["ShadowColor"] = newColor;

        }


        public void UiWindow_Closing(object sender, CancelEventArgs e)
        {
                e.Cancel = true;
                ShowInTaskbar = true;
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
            DashBoard.Hide();
            Hide();
        }

        public void Exit_Click(object sender, RoutedEventArgs e)
        {
                Environment.Exit(0);
        }

        private void NotifyIcon_LeftClick(Wpf.Ui.Controls.NotifyIcon sender, RoutedEventArgs e)
        {
            if (islogin)
            {
                DashBoard.Show();
            }
            else
            {
                Show();
            }
        }
    }

    public class InfoResponseObjectt
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("traffic")]
        public long Traffic { get; set; }

        [JsonProperty("inbound")]
        public int Inbound { get; set; }

        [JsonProperty("outbound")]
        public int Outbound { get; set; }

        [JsonProperty("proxies_num")]
        public int ProxiesNum { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }

    public class ResponseObject
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UserData UserData { get; set; }
    }

    public class UserData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FrpToken { get; set; }
        public long Traffic { get; set; }
        public int Inbound { get; set; }
        public int Outbound { get; set; }
        public string Avatar { get; set; }
    }

}