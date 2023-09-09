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
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace LoCyanFrpDesktop
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private InfoResponseObjectt UserInfo;
        string username_auto;
        string token_auto;

        public MainWindow()
        {
            InitializeComponent();
            Uri iconUri = new Uri("pack://application:,,,/LoCyanFrpDesktop;component/Resource/favicon.ico", UriKind.RelativeOrAbsolute);
            this.Icon = new BitmapImage(iconUri);
            InitializeAutoLaunch();
            InitializeAutoLogin();
        }

        private void InitializeAutoLaunch()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, "auto_launch.ini");

            if (File.Exists(filePath))
            {
                string url = File.ReadAllText(filePath);

                // 使用正则表达式提取token和id
                Match match = Regex.Match(url, @"locyanfrp://([^/]+)/([^/]+)");

                if (match.Success && match.Groups.Count == 3)
                {
                    string token = match.Groups[1].Value;
                    string id = match.Groups[2].Value;
                    string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    RunCmdCommand($"\"{appDirectory}/frpc.exe\" -u {token} -p {id}");
                    File.Delete(filePath );
                    Close();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show($"自动启动出错 \n url: {url} \n filepath: {filePath}", "警告");
                }
            }
        }

        private void RunCmdCommand(string command)
        {
            // 创建一个 ProcessStartInfo 对象
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe", // 指定要运行的命令行程序
                Arguments = "/k " + command, // 使用 /k 参数保持 cmd 窗口打开，显示输出内容
                UseShellExecute = true, // 设置为 true 以便在新窗口中显示命令行窗口
                CreateNoWindow = false // 设置为 false 以显示命令行窗口
            };

            // 创建一个 Process 对象并启动进程
            Process.Start(psi);
        }

        private async void InitializeAutoLogin() {
            bool islogin = await CheckLogined();
            if (islogin)
            {
                Properties.Settings.Default.LoginToken = token_auto;
                Properties.Settings.Default.username = username_auto;
                Properties.Settings.Default.FrpToken = UserInfo.Token;
                new DashBoard().Show();
                Close();
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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string username = txtusername.Text;
            string password = txtpassword.Text;

            if (username != "" && password != "")
            {
                using (var httpClient = new HttpClient()) {
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
                            System.Windows.Forms.MessageBox.Show("账号或密码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else {
                            System.Windows.Forms.MessageBox.Show($"登录成功, 获取到登录Token: {responseObject.Token}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Properties.Settings.Default.LoginToken = responseObject.Token;
                            Properties.Settings.Default.username = responseObject.UserData.Username;
                            Properties.Settings.Default.FrpToken = responseObject.UserData.FrpToken;
                            string path = ".//session.token";
                            string text = $"{responseObject.UserData.Username}|{responseObject.Token}";
                            File.WriteAllText(path, text);
                            new DashBoard().Show();
                            Close();
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"请求失败: {ex.Message}");
                    }
                }
            } else
            {
                System.Windows.Forms.MessageBox.Show("用户名 / 密码不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

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