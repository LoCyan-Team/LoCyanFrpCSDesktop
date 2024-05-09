using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
using Wpf.Ui.Controls;
using LoCyanFrpDesktop.Utils;
using LoCyanFrpDesktop.Dashboard;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.IO;
using static LoCyanFrpDesktop.Utils.PNAP;

namespace LoCyanFrpDesktop.Dashboard
{
    /// <summary>
    /// Interaction logic for ProxyList.xaml
    /// </summary>
    public partial class ProxyList : UiPage
    {
        public ObservableCollection<string> Proxies { get; set; }

        public static List<Proxy> Proxieslist { get; set; }
       
        //public PNAPListComp ListComponents = new PNAPListComp();
        public static List<PNAPListComp> PNAPList = new List<PNAPListComp>();
        public string SelectedProxy { get; set; }
        public static string lineFiltered;
        public ProxyList()
        {
            InitializeComponent();
            InitializeProxiesAsync();
            //Wait For Rewrite.
            //InitializeAutoLaunch();
            DataContext = this;
            title_username.Text = $"欢迎回来，{Properties.Settings.Default.username}";
            Resources["BorderColor"] = MainWindow.DarkThemeEnabled ? Colors.White : Colors.LightGray;
            
        }

        private async void InitializeProxiesAsync()
        {
            await GetProxiesListAsync();
            // 若返回 null，则表示无隧道或请求失败
            if (Proxies == null)
            {
                Proxies = new ObservableCollection<string>();
            }
            /*
            // 使用 await 关键字确保数据加载完成后再更新 DataContext
            await Dispatcher.InvokeAsync(() =>
            {
                proxies_list.DataContext = this;
            });
            proxies_list.Items.Refresh();*/
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
                }
                catch (Exception ex)
                {
                    CrashInterception.ShowException(ex);
                    return null;
                }
            }

            if (responseObject.Status != 0)
            {
                Logger.MsgBox("获取隧道失败，请重启软件重新登陆账号", "LocyanFrp",0,48,1);
                return null;
            }

            // 初始化列表 proxiesListInName
            List<string> proxiesListInName = new List<string>();
            foreach (var proxy in responseObject.Proxies)
            {
                proxiesListInName.Add(proxy.ProxyName);
            }

            Proxieslist = responseObject.Proxies;
            Dispatcher.Invoke(() =>
            {
                proxies_list.ItemsSource = Proxieslist;
            });
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
            [JsonProperty("domain")]
            public string Domain { get; set; }
            public int Node { get; set; }

            [JsonProperty("icp")]
            public object Icp { get; set; } // icp 字段可能为 null，使用 object 类型表示
            public override string ToString()
            {
                return $"{this.ProxyName} {ProxyType} {LocalIp}:{LocalPort}-->{Domain}:{RemotePort}";
            }
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
                if (item.ProxyName == proxy_name)
                {
                    proxy_id = item.Id;
                    break;
                }
            }

            if (proxy_id == 0)
            {
                Logger.MsgBox("无法将隧道名解析为隧道ID，请检查自己的隧道配置", "LocyanFrp", 0, 48, 1);
                return;
            }
            Access.DashBoard.Navigation.Navigate(1);
            // 运行frp
            try
            {
                if (PNAP.PNAPList.Any(prcs => prcs.ProcessName == proxy_id))
                {
                    Logger.MsgBox("这个隧道已经启动了哦", "LocyanFrp", 0, 48, 1);
                    return;
                }

            }
            catch (Exception ex)
            {

            }
            RunCmdCommand($" -u {Properties.Settings.Default.FrpToken} -p ",proxy_id, proxies_list.SelectedIndex);

        }

        private void Proxies_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果需要判断是否有选中项
            if (proxies_list.SelectedItem != null)
            {
                SelectedProxy = Proxieslist[proxies_list.SelectedIndex].ProxyName;
            }
            else
            {
                // 没有选中项的处理逻辑
            }
        }

        private void RunCmdCommand(string command,int ProxyID,int SelectionIndex)
        {
            // 创建一个 ProcessStartInfo 对象
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Properties.Settings.Default.FrpcPath, // 指定要运行的命令行程序
                Arguments = command + ProxyID, // 使用 /k 参数保持 cmd 窗口打开，显示输出内容
                Verb = "runas",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false, // 设置为 true 以便在新窗口中显示命令行窗口
                CreateNoWindow = true, // 设置为 false 以显示命令行窗口
                StandardOutputEncoding = Encoding.UTF8
            };
            // 启动进程
            Process _FrpcProcess = Process.Start(psi);
            _FrpcProcess.BeginOutputReadLine();
            try
            {
                if (!PNAPList.Any(Process => Process.ProcessName == ProxyID))
                {   

                    PNAPList.Add(new PNAPListComp() { ProcessName = ProxyID, IsRunning = true, Pid = _FrpcProcess.Id, ListIndex = SelectionIndex });
                }
                else
                {
                    int Index = PNAPList.FindIndex(Process => Process.ProcessName == ProxyID);
                    PNAPList[Index].IsRunning = true;
                    PNAPList[Index].ListIndex = SelectionIndex;
                    PNAPList[Index].Pid = _FrpcProcess.Id;
                }
            }catch (Exception ex)
            {
                PNAPList.Add(new PNAPListComp() { ProcessName = ProxyID, IsRunning = true, Pid = _FrpcProcess.Id, ListIndex = SelectionIndex });
            }
            
            _FrpcProcess.OutputDataReceived += SortOutputHandler;
            // 读取标准输出和标准错误输出
            //string output = process.StandardOutput.ReadToEnd(); 
            //string error = process.StandardError.ReadToEnd();

            // 等待进程完成
            //process.WaitForExit();

            // 打印输出
            //Console.WriteLine("Output:\n" + output);
            //Console.WriteLine("Error:\n" + error);

            // 可以将输出存储到变量中，以便后续处理
            // string combinedOutput = output + error;

        }
        private static void SortOutputHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Logger.Output(LogType.Info, e.Data);
            }
        }


        private void ListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }
        private void ListView_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void StartProxy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Index = PNAPList.FindIndex(Process => Process.ListIndex == proxies_list.SelectedIndex);
                if (!(bool)PNAPList[Index].IsRunning)
                {
                    Button_Click(sender, e);
                }
                else
                {
                    Logger.MsgBox("这个隧道已经启动了哦", "LocyanFrp", 0, 48, 1);
                }
            }catch(Exception ex)
            {   
                Button_Click(sender, e);
            }
            
            
        }
        private void StopProxy_Click(Object sender, RoutedEventArgs e)
        {
            //我在写什么，我不知道我在写些什么
            try
            {
                int Index = PNAPList.FindIndex(Process => Process.ListIndex == proxies_list.SelectedIndex);
                if (!(bool)PNAPList[Index].IsRunning)
                {   

                    Logger.MsgBox("这个隧道并没有启动哦", "LocyanFrp", 0, 48, 1);
                }
                else
                {
                    try
                    {
                        Process.GetProcessById(PNAPList[Index].Pid).Kill();
                    }catch (Exception ex)
                    {
                        CrashInterception.ShowException(ex);
                    }
                    Logger.MsgBox("这个隧道成功关闭了哦", "LocyanFrp", 0, 48, 1);

                    PNAPList[Index].IsRunning = false;
                }
            }catch( Exception ex)
            {   
                CrashInterception.ShowException(ex);
                Logger.MsgBox("这个隧道并没有启动哦", "LocyanFrp", 0, 48, 1);
            }
            
        }

        private void DeleteProxy_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void CreateNewProxy_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void proxies_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StartProxy_Click(sender, e);
        }
        /*private void InitializeAutoLaunch()
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
           RunCmdCommand($" -u {token} -p {id}");
           File.Delete(filePath);
       }
       else
       {
           System.Windows.MessageBox.Show($"自动启动出错 \n url: {url} \n filepath: {filePath}", "警告");
       }
   }

}*/
    }
}
