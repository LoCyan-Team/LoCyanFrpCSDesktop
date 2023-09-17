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
using System.Windows.Shapes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace LoCyanFrpDesktop
{
    /// <summary>
    /// DashBoard.xaml 的交互逻辑
    /// </summary>
    public partial class DashBoard : Window
    {
        public ObservableCollection<string> Proxies { get; set; }

        public List<Proxy> Proxieslist { get; set; }

        public string SelectedProxy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DashBoard()
        {
            InitializeComponent();
            Uri iconUri = new Uri("pack://application:,,,/LoCyanFrpDesktop;component/Resource/favicon.ico", UriKind.RelativeOrAbsolute);
            this.Icon = new BitmapImage(iconUri);
            title_username.Content = Properties.Settings.Default.username;
            InitializeProxiesAsync();
        }

        private async void InitializeProxiesAsync()
        {
            Proxies = await GetProxiesListAsync();
            // 若返回 null，则表示无隧道或请求失败
            if (Proxies == null)
            {
                Proxies = new ObservableCollection<string>();
            }

            // 使用 await 关键字确保数据加载完成后再更新 DataContext
            await Dispatcher.InvokeAsync(() =>
            {
                proxies_list.DataContext = this;
            });
            proxies_list.Items.Refresh();
        }

        // 封装隧道获取，采用异步请求防止主线程卡死
        private async Task<ObservableCollection<string>> GetProxiesListAsync()
        {
            // 获取用户名和Token
            string username = Properties.Settings.Default.username;
            string token = Properties.Settings.Default.LoginToken;
            // 实例化序列
            GetProxiesResponseObject responseObject;
            // 创建新的 HttpClient 实例
            using (var client = new HttpClient())
            {
                // 定义API链接
                string url = $"https://api.locyanfrp.cn/Proxies/GetProxiesList?username={username}&token={token}";

                // 防止API报错
                try
                {
                    // 等待请求
                    HttpResponseMessage response = await client.GetAsync(url);
                    // 确保请求完成
                    response.EnsureSuccessStatusCode();
                    // 结果转换为字符串
                    string jsonString = await response.Content.ReadAsStringAsync();
                    // 结果序列化
                    responseObject = JsonConvert.DeserializeObject<GetProxiesResponseObject>(jsonString);
                } catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                    return null;
                }
            }

            if (responseObject.Status != 0) {
                System.Windows.Forms.MessageBox.Show("获取隧道失败，请重启软件重新登陆账号", "提示");
                return null;
            }

            // 初始化列表 proxiesListInName
            List<string> proxiesListInName = new List<string>();
            foreach (var proxy in responseObject.Proxies)
            {
                proxiesListInName.Add(proxy.ProxyName);
            }

            Proxieslist = responseObject.Proxies;

            var proxies = new ObservableCollection<string>(proxiesListInName);
            return proxies;
        }

        public class Proxy
        {
            public int Id { get; set; }

            [JsonProperty("proxy_name")]
            public string ProxyName { get; set; }

            [JsonProperty("proxy_type")]
            public string ProxyType { get; set; }

            [JsonProperty("local_ip")]
            public string LocalIp { get; set; }

            [JsonProperty("local_port")]
            public int LocalPort { get; set; }

            [JsonProperty("remote_port")]
            public string RemotePort { get; set; }

            [JsonProperty("use_compression")]
            public bool UseCompression { get; set; }

            [JsonProperty("use_encryption")]
            public bool UseEncryption { get; set; }

            public string Domain { get; set; }
            public int Node { get; set; }

            [JsonProperty("icp")]
            public object Icp { get; set; } // icp 字段可能为 null，使用 object 类型表示
        }

        public class GetProxiesResponseObject
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public int Count { get; set; }
            public List<Proxy> Proxies { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string proxy_name = SelectedProxy;
            int proxy_id = 0;
            foreach (var item in Proxieslist)
            {
                if (item.ProxyName == proxy_name) {
                    proxy_id = item.Id; 
                    break;
                }
            }

            if (proxy_id == 0) {
                System.Windows.Forms.MessageBox.Show("无法将隧道名解析为隧道ID，请检查自己的隧道配置", "警告");
                return;
            }

            // 运行frp
            RunCmdCommand($"frpc.exe -u {Properties.Settings.Default.FrpToken} -p {proxy_id}");

        }

        private void Proxies_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果需要判断是否有选中项
            if (proxies_list.SelectedItem != null)
            {
                SelectedProxy = proxies_list.SelectedItem as string;
            }
            else
            {
                // 没有选中项的处理逻辑
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
    }
}
