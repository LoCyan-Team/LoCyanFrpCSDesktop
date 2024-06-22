﻿using Newtonsoft.Json;
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
using System.Drawing.Printing;
using System.Xml.Linq;
using HandyControl.Tools.Extension;
using MenuItem = Wpf.Ui.Controls.MenuItem;
using Wpf.Ui.Appearance;
using System.Reflection;
using Wpf.Ui.Styles.Controls;
using ContextMenu2 = Wpf.Ui.Styles.Controls.ContextMenu;
using ContextMenu = System.Windows.Controls.ContextMenu;
using System.DirectoryServices.ActiveDirectory;
using System.Printing;
using Microsoft.Exchange.WebServices.Data;
using System.Windows.Media.Animation;
using System.Drawing;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Brush = System.Windows.Media.Brush;
using System.Runtime.CompilerServices;
using LoCyanFrpDesktop.Extensions;
using Microsoft.Win32.SafeHandles;


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
        public static string SelectedProxy { get; set; }
        public static string lineFiltered;
        public static object BackgroundColor;
        public static ContextMenu BackgroundMenu;
        public static List<ProxyCard> Cards = new();
        public ProxyList()
        {
            InitializeComponent();
            InitializeProxiesAsync();
            //Wait For Rewrite.
            //InitializeAutoLaunch();
            DataContext = this;
            title_username.Text += Properties.Settings.Default.username;
            Resources["BorderColor"] = MainWindow.DarkThemeEnabled ? Colors.White : Colors.LightGray;
            //BackgroundColor = Resources["ControlFillColorDefaultBrush"];
            BackgroundMenu = new();
            //Inbound.Text += MainWindow.Inbound;
            //OutBound.Text += MainWindow.Outbound;
            Traffic.Text += (MainWindow.Traffic / 1024) + "GB";
            RefreshAvatar();
        }
        private async void RefreshAvatar()
        {
            try
            {
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri(MainWindow.Avatar);
                    var Avatar = await client.GetAsync(client.BaseAddress).Await().Content.ReadAsStreamAsync();
                    var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar.png");
                    
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using(FileStream fileStream = new(path, FileMode.Create))
                    {
                        byte[] bytes = new byte[Avatar.Length];
                        Avatar.Read(bytes, 0, bytes.Length);
                        // 设置当前流的位置为流的开始
                        Avatar.Seek(0, SeekOrigin.Begin);

                        // 把 byte[] 写入文件
                        
                        BinaryWriter bw = new BinaryWriter(fileStream);
                        bw.Write(bytes);
                        bw.Close();
                        fileStream.Close();
                    }
                    BitmapImage bitmap = new BitmapImage();

                    // 设置 BitmapImage 的 UriSource 属性为图片文件的路径
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                    var a = new ImageBrush(bitmap)
                    {
                        Stretch = Stretch.UniformToFill,

                    };

                    Dispatcher.Invoke(() =>
                    {
                        this.Avatar.Background = a;
                    });
                    

                }

            }
            catch (Exception ex)
            {
                Logger.MsgBox("无法获取您的头像, 请稍后重试", "LocyanFrp", 0, 48, 1);
            }
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
                Logger.MsgBox("获取隧道失败，请重启软件重新登陆账号", "LocyanFrp", 0, 48, 1);
                return null;
            }

            // 初始化列表 proxiesListInName
            List<string> proxiesListInName = new List<string>();
            for (int i = 0; i < responseObject.Proxies.Count; i++)
            {
                Console.WriteLine(i);
                Dispatcher.Invoke(() =>
                {
                    ListPanel.Children.Add(new ProxyCard(responseObject.Proxies[i], i));
                    /*ListPanel.Children.Add(new Card()
                    {
                        //Background = new SolidColorBrush(Colors.White),
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 0, 10, 0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        MinHeight = 50,
                        MinWidth = 100
                    });*/

                });

            }
            Proxieslist = responseObject.Proxies;

            var proxies = new ObservableCollection<string>(proxiesListInName);
            return proxies;
        }



        private static int ProxyStarter(string SelectedProxy,int SelectedIndex)
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
                return 0;
            }
            Access.DashBoard.Navigation.Navigate(1);
            // 运行frp
            try
            {
                if (PNAP.PNAPList.Any(prcs => prcs.ProcessName == proxy_id))
                {
                    Logger.MsgBox("这个隧道已经启动了哦", "LocyanFrp", 0, 48, 1);
                    return 0;
                }

            }
            catch (Exception ex)
            {

                return RunCmdCommand($" -u {Properties.Settings.Default.FrpToken} -p ", proxy_id, SelectedIndex);
            }
            return RunCmdCommand($" -u {Properties.Settings.Default.FrpToken} -p ",proxy_id, SelectedIndex);

        }


        /*private void Proxies_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }*/

        private static int RunCmdCommand(string command, int ProxyID, int SelectionIndex)
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
            }
            catch (Exception ex)
            {
                PNAPList.Add(new PNAPListComp() { ProcessName = ProxyID, IsRunning = true, Pid = _FrpcProcess.Id, ListIndex = SelectionIndex });
            }

            _FrpcProcess.OutputDataReceived += SortOutputHandler;
            _FrpcProcess.EnableRaisingEvents = true;
            _FrpcProcess.Exited += new EventHandler((sender, e) => _FrpcProcess_Exited(sender, e, SelectionIndex));
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
            return _FrpcProcess.Id;
        }

        private static void _FrpcProcess_Exited(object? sender, EventArgs e, int index)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Cards[index].IndicatorLight.Stroke = Brushes.Gray;
            });
        }
        private void NonStaticExitedEventHandler(int index) {
            Dispatcher.Invoke(() =>
            {
                Cards[index].IndicatorLight.Stroke = Brushes.Gray;
            });
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
        
        private void ListCardClickHandler(object sender, MouseButtonEventArgs e)
        {
            //new Card.ContextMenu();
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
        public class ProxyCard : Border
        {
            public int IndexID { get; set; }
            public Ellipse IndicatorLight = new Ellipse()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 8,
                Width = 8,
                Stroke = new SolidColorBrush(Colors.Gray)

            };
            public ProxyCard(Proxy ProxyInfo, int CardIndex)
            {   
                IndexID = CardIndex;
                Cards.Add(this);
                //DefaultStyleKeyProperty.OverrideMetadata(typeof(ProxyCard), new FrameworkPropertyMetadata(typeof(ProxyCard)));
                //this.Style.BasedOn = (Style)Application.Current.Resources["Card"];
                string Name = ProxyInfo.ProxyName;
                //this.Name = Name;
                //this.OverridesDefaultStyle = true;
                //this.Style = ProxyList.card.Style;
                //Theme.Apply(ThemeType.Light);
                this.Background = new SolidColorBrush(MainWindow.DarkThemeEnabled ? Color.FromRgb(53, 53, 53) : Color.FromRgb(210, 210, 210));
                //this.Background = new SolidColorBrush(!string.IsNullOrEmpty((string)ProxyList.BackgroundColor) ? (Color)ProxyList.BackgroundColor : Colors.Gray);
                //this.Background = new SolidColorBrush((Color)ProxyList.BackgroundColor);
                this.BorderThickness = new Thickness(2);
                this.CornerRadius = new CornerRadius(5);
                this.Padding = new Thickness(10);
                this.Margin = new Thickness(0, 0, 10, 0);
                this.HorizontalAlignment = HorizontalAlignment.Left;
                this.VerticalAlignment = VerticalAlignment.Stretch;
                this.MinHeight = 50;
                this.Width = 200;
                this.BorderBrush = new SolidColorBrush();
                StackPanel stackPanel = new StackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                DockPanel dockPanel = new DockPanel()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment= HorizontalAlignment.Stretch,
                    MinWidth = 100,
                };
                this.Child = stackPanel;
                //this.AddChild(stackPanel);
                //this.AddChild();
                stackPanel.Children.Add(dockPanel);
                dockPanel.Children.Add(new TextBlock()
                {
                    Text = $"{ProxyInfo.ProxyName}",

                });
                
                dockPanel.Children.Add(IndicatorLight);
                stackPanel.Children.Add(new TextBlock()
                {
                    Text = $"{ProxyInfo.LocalIp}:{ProxyInfo.LocalPort} --> Node{ProxyInfo.Node}:{ProxyInfo.RemotePort}",
                });

                //dockPanel.Children.Add();

                ProxyMenu temp = new ProxyMenu(ProxyInfo.ProxyName, CardIndex,this);
                temp.ID = ProxyInfo.Id;
                this.ContextMenu = temp;
                //dockPanel.Children.Add(temp);
                //this.AddChild(temp);

                this.MouseRightButtonDown += this.OnMouseRightButtonDown;
                this.MouseEnter += this.OnMouseEnter;
                this.MouseLeave += this.OnMouseLeave;
            }
            private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
            {
                
                
            }
            private void ProxyCard_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                //StartProxy_Click(sender, e);
            }
            private void OnMouseEnter(object sender, EventArgs e)
            {
                Border border = (Border)sender;
                ColorAnimation colorAnimation = new ColorAnimation(Colors.Transparent, Colors.Aqua, TimeSpan.FromMilliseconds(200));
                try
                {
                    border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                }
                catch(Exception ex) { CrashInterception.ShowException(ex); }
                
            }
            private void OnMouseLeave(object sender, EventArgs e)
            {
                Border border = (Border)sender;
                ColorAnimation colorAnimation = new ColorAnimation(Colors.Aqua, Colors.Transparent, TimeSpan.FromMilliseconds(200));
                try
                {
                    border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                }
                catch { }


            }
        }
        public class ProxyMenu : ContextMenu
        {
            public int ID { get; set; }
            public int Pid { get; set; }
            public string ProxyName { get; set; }
            public int IndexID = -1;
            public ProxyCard Card { get; set; }
            public ProxyMenu(string proxyName,int IndexID, ProxyCard card)
            {   
                this.Card = card;
                this.IndexID = IndexID;
                Style roundedContextMenuStyle = new Style();

                // 设置样式的 TargetType
                roundedContextMenuStyle.TargetType = typeof(ContextMenu);
                
                // 定义样式的模板
                ControlTemplate template = new ControlTemplate(typeof(ContextMenu));
                FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.SetValue(Border.BackgroundProperty, new SolidColorBrush(MainWindow.DarkThemeEnabled ? Color.FromRgb(53, 53, 53) : Color.FromRgb(210, 210, 210)));
                borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
                borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(5));
                this.Foreground = new SolidColorBrush(MainWindow.DarkThemeEnabled ? Colors.White : Colors.Black);
                FrameworkElementFactory stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
                stackPanelFactory.SetValue(StackPanel.IsItemsHostProperty, true);

                borderFactory.AppendChild(stackPanelFactory);
                template.VisualTree = borderFactory;

                roundedContextMenuStyle.Setters.Add(new Setter(ContextMenu.TemplateProperty, template));

                // 应用样式
                this.Style = roundedContextMenuStyle;
                //this.Style = ProxyList.DefaultStyleKeyProperty;
                //this.OverridesDefaultStyle = true;
                /*
                 <ContextMenu Name="ListMenu">
                                <ui:MenuItem Header="刷新" Click="Refresh_Click"/>
                                <Separator/>
                                <ui:MenuItem Header="新建隧道" Click="CreateNewProxy_Click"/>
                                <ui:MenuItem Header="删除隧道" Click="DeleteProxy_Click"/>
                                <Separator/>
                                <ui:MenuItem Header="启动隧道" Click="StartProxy_Click"/>
                                <ui:MenuItem Header="停止隧道" Click="StopProxy_Click"/>
                            </ContextMenu>

                 */
                //this.Background = new SolidColorBrush(Color.FromRgb(45,45,45));

                MenuItem Refresh = new MenuItem()
                {
                    Header = "刷新",


                };
                MenuItem CreateNewProxy = new MenuItem()
                {
                    Header = "新建隧道"
                };
                MenuItem DeleteProxy = new MenuItem()
                {
                    Header = "删除隧道"
                };
                MenuItem StartProxy = new MenuItem()
                {
                    Header = "启动隧道"
                };
                MenuItem StopProxy = new MenuItem()
                {
                    Header = "停止隧道"
                };
                Refresh.Click += Refresh_Click;
                CreateNewProxy.Click += CreateNewProxy_Click;
                DeleteProxy.Click += DeleteProxy_Click;
                StartProxy.Click += StartProxy_Click;
                StopProxy.Click += StopProxy_Click;
                this.Items.Add(Refresh);
                this.Items.Add(new Separator());
                this.Items.Add(CreateNewProxy);
                this.Items.Add(DeleteProxy);
                this.Items.Add(new Separator());
                this.Items.Add(StartProxy);
                this.Items.Add(StopProxy);
                this.BorderBrush = Brushes.Transparent;
                this.BorderThickness = new Thickness(1);
                Refresh.IsEnabled = false;
                CreateNewProxy.IsEnabled = false;
                DeleteProxy.IsEnabled = false;
                ProxyName = proxyName;
                //contextMenu.Margin = new Thickness(5);
                //this.CornerRadius = new CornerRadius(5);

            }
            private void StartProxy_Click(object sender, RoutedEventArgs e)
            {
                
                try
                {
                    
                    int Index = PNAPList.FindIndex(Process => Process.ListIndex == (IndexID != -1 ? IndexID : throw new Exception()));
                    if (!(bool)PNAPList[Index].IsRunning)
                    {
                        Pid = ProxyStarter(this.ProxyName,IndexID);
                        if (Pid != 0)
                        {
                            Card.IndicatorLight.Stroke = Brushes.LightGreen;
                        }
                    }
                    else
                    {
                        Logger.MsgBox("这个隧道已经启动了哦", "LocyanFrp", 0, 48, 1);
                    }
                }
                catch (Exception ex)
                {
                    Pid = ProxyStarter(this.ProxyName, IndexID);
                    if (Pid != 0)
                    {
                        Card.IndicatorLight.Stroke = Brushes.LightGreen;
                    }
                }
                


            }
            
            private void Refresh_Click(object sender, RoutedEventArgs e)
            {
                throw new NotImplementedException();
            }

            private void StopProxy_Click(Object sender, RoutedEventArgs e)
            {
                //我在写什么，我不知道我在写些什么w
                
                try
                {
                    int Index = PNAPList.FindIndex(Process => Process.ListIndex == IndexID);
                    if (!(bool)PNAPList[Index].IsRunning)
                    {   

                        Logger.MsgBox("这个隧道并没有启动哦", "LocyanFrp", 0, 48, 1);
                    }
                    else
                    {
                        try
                        {
                            Process.GetProcessById(Pid).Kill();
                        }catch (Exception ex)
                        {
                            CrashInterception.ShowException(ex);
                        }
                        Logger.MsgBox("这个隧道成功关闭了哦", "LocyanFrp", 0, 48, 1);

                        PNAPList[Index].IsRunning = false;
                        Card.IndicatorLight.Stroke = Brushes.Gray;
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
        }
    }
    public class GetProxiesResponseObject
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public List<Proxy> Proxies { get; set; }
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
        public int UseCompression { get; set; }

        [JsonProperty("use_encryption")]
        public int UseEncryption { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        public int Node { get; set; }

        [JsonProperty("icp")]
        public object Icp { get; set; } // icp 字段可能为 null，使用 object 类型表示
    }

    
    
}


//BreakAutoCompileBecauseTheRewriteIsNOTFinished
