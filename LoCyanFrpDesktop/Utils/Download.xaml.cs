using Downloader;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms.Design;
using System.Linq.Expressions;

namespace LoCyanFrpDesktop.Utils
{
    /// <summary>
    /// Interaction logic for Downloader.xaml
    /// </summary>
    public partial class Download : UiWindow
    {   
        bool isDownloadFinished = false;
        public static Snackbar Snackbar = new Snackbar();
        public static DownloadService DownloadService;
        public static string APIInfo;
        public static string TheFuckingLink;
        public static string DownloadPath = AppDomain.CurrentDomain.BaseDirectory;
        public string DownloadUnit = "KB/s";
        public static string FolderName;
        public Download()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            InitDownloader();
            //this.Owner = this;
            Tips.Text = Global.Tips[Random.Shared.Next(0, Global.Tips.Count - 1)];
            StartDownload();
            
        }

        public void InitDownloader()
        {
            var DownloadOption = new DownloadConfiguration()
            {
                BufferBlockSize = 10240,
                // file parts to download, the default value is 1
                ChunkCount = 8,
                // download speed limited to 2MB/s, default values is zero or unlimited
                MaximumBytesPerSecond = 1024 * 1024 * 2,
                // the maximum number of times to fail
                MaxTryAgainOnFailover = 5,
                // release memory buffer after each 50 MB
                MaximumMemoryBufferBytes = 1024 * 1024 * 50,
                // download parts of the file as parallel or not. The default value is false
                ParallelDownload = true,
                // number of parallel downloads. The default value is the same as the chunk count
                ParallelCount = 4,
                // timeout (millisecond) per stream block reader, default values is 1000
                Timeout = 3000,
                // clear package chunks data when download completed with failure, default value is false
                ClearPackageOnCompletionWithFailure = true,
                ReserveStorageSpaceBeforeStartingDownload = true,
                RequestConfiguration = {
                    Accept = "*/*",
                    //CookieContainer = cookies,
                    Headers = new WebHeaderCollection(), // { your custom headers }
                    KeepAlive = true, // default value is false
                    ProtocolVersion = HttpVersion.Version11, // default value is HTTP 1.1
                    UseDefaultCredentials = false,
                    // your custom user agent or your_app_name/app_version.
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 Edg/124.0.0.0",
                }
            };
            DownloadService = new DownloadService(DownloadOption);

            // Provide any information about chunker downloads, 
            // like progress percentage per chunk, speed, 
            // total received bytes and received bytes array to live streaming.
            //DownloadService.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;

            // Provide any information about download progress, 
            // like progress percentage of sum of chunks, total speed, 
            // average speed, total received bytes and received bytes array 
            // to live streaming.


            // Download completed event that can include occurred errors or 
            // cancelled or download completed successfully.

            DownloadService.DownloadStarted += OnDownloadStarted;
            DownloadService.DownloadProgressChanged += OnDownloadProgressChanged;
            DownloadService.DownloadFileCompleted += OnDownloadFileCompleted;
        }

        private void OnDownloadStarted(object? sender, DownloadStartedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DownloadProgress.IsIndeterminate = false;
            });
            
        }

        private void OnDownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
        {   
            Dispatcher.Invoke(() => 
            {
                double speed = Math.Round(e.BytesPerSecondSpeed / 1024, 2);
                if (e.BytesPerSecondSpeed > 1024 * 1024)
                {
                    speed = Math.Round(e.BytesPerSecondSpeed / 1024 / 1024, 2);
                    DownloadUnit = "MB/s";
                }
                else
                {
                    speed = Math.Round(e.BytesPerSecondSpeed / 1024, 2);
                    DownloadUnit = "KB/s";
                }
                DownloadProgress.Value = (int) e.ProgressPercentage;
                
                DownloadSpeed.Text = $"下载速度: {speed} {DownloadUnit}";
            });
            
        }

        private void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Cancelled)
                {
                    Console.WriteLine("CANCELED");
                    return;
                }
                else if (e.Error != null)
                {
                    Console.Error.WriteLine(e.Error);
                    CrashInterception.ShowException(e.Error);
                    this.Owner = null;
                    Close();
                    return;
                }
                else
                {
                    Console.WriteLine("DONE");
                    DownloadProgress.Visibility = Visibility.Collapsed;
                    DownloadProgressRing.Visibility = Visibility.Visible;
                    DownloadSpeed.Visibility = Visibility.Collapsed;
                    Notice.Text = "正在解压......";
                    Thread.Sleep(1000);
                    UnpackAndAutoSetup();
                }
                
            });
            
        }
        public void UnpackAndAutoSetup()
        {
            try
            {   
                if (Directory.Exists(Path.Combine(DownloadPath, "Temp")))
                {
                    Directory.Delete(Path.Combine(DownloadPath, "Temp"),true);
                    
                    Directory.CreateDirectory(Path.Combine(DownloadPath, "Temp"));
                }
                
                ZipFile.ExtractToDirectory(Path.Combine(DownloadPath, "frpc.temp"), Path.Combine(DownloadPath, "Temp"));
                if(File.Exists(Path.Combine(DownloadPath, "frpc.exe")))
                {
                    File.Delete(Path.Combine(DownloadPath, "frpc.exe"));
                    File.Move(Path.Combine(DownloadPath, "Temp", FolderName, "frpc.exe"), Path.Combine(DownloadPath, "frpc.exe"));
                }
                else
                {
                    File.Move(Path.Combine(DownloadPath, "Temp", FolderName, "frpc.exe"), Path.Combine(DownloadPath, "frpc.exe"));
                }
                string path = Path.Combine(DownloadPath, "frpc.exe");
                try
                {
                    Access.Settings.FrpcPath.Text = path;
                }catch (Exception ex)
                {
                    //CrashInterception.ShowException(ex);
                }
                Properties.Settings.Default.FrpcPath = path;
                var ConfigPath = Path.Combine(DownloadPath, "FrpcPath.conf");

                if (File.Exists(ConfigPath))
                {
                    StreamReader sr = new StreamReader(ConfigPath);
                    if (string.IsNullOrEmpty(sr.ReadLine()))
                    {
                        sr.Close();
                        StreamWriter sw = new StreamWriter(ConfigPath);
                        sw.WriteLine(path);
                        sw.Close();
                    }
                    else
                    {
                        sr.Close();
                        File.Delete(ConfigPath);
                        File.Create(ConfigPath);
                        StreamWriter sw = new StreamWriter(ConfigPath);
                        sw.WriteLine(path);
                        sw.Close();
                    }
                }
                else
                {
                    File.Create(ConfigPath);
                    StreamWriter sw = new StreamWriter(ConfigPath);
                    sw.WriteLine(path);
                    sw.Close();

                }


                
                Close();
            }
            catch (Exception ex) {
                this.Close();
                CrashInterception.ShowException(ex);
                
            }
            
            



            
        }
        public static async Task<IDownloadService> DownloadFile(string url,string path)
        {
            try
            {
                await DownloadService.DownloadFileTaskAsync(url, Path.Combine(path + "frpc.temp"));
            }catch(Exception ex)
            {
                CrashInterception.ShowException(ex);
            }
            

            return DownloadService;
        }
        private async void StartDownload()
        {
            await RequestAPIandParse("https://api-gh.1l1.icu/repos/LoCyan-Team/LoCyanFrpPureApp/releases/latest");

        }
        private static async Task RequestAPIandParse(string url)
        {
            try
            {
                var HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36 Edg/115.0.1901.188");
                var Response = await HttpClient.GetAsync(url);
                if (Response.IsSuccessStatusCode)
                {
                    var APIResponse = Response.Content.ReadAsStringAsync();
                    APIInfo = APIResponse.Result;
                    JObject ParsedAPIInfo = JObject.Parse(APIInfo);
                    string DownloadLink = ParsedAPIInfo["assets"][0]["browser_download_url"].ToString();
                    string DownloadVersion = ParsedAPIInfo["tag_name"].ToString();
                    string pattern = @"v(\d+\.\d+\.\d+)-\d+";
                    Match match = Regex.Match(DownloadVersion, pattern);
                    var Version = match.Groups[1].Value;
                    string architecture = RuntimeInformation.OSArchitecture.ToString();
                    if (architecture == "X86")
                    {
                        TheFuckingLink = $"https://proxy-gh.1l1.icu/https://github.com/LoCyan-Team/LoCyanFrpPureApp/releases/download/{DownloadVersion}/frp_LoCyanFrp-{Version}_windows_386.zip";
                    }
                    else
                    {
                        TheFuckingLink = $"https://proxy-gh.1l1.icu/https://github.com/LoCyan-Team/LoCyanFrpPureApp/releases/download/{DownloadVersion}/frp_LoCyanFrp-{Version}_windows_amd64.zip";
                    }
                    FolderName = $"frp_LoCyanFrp-{Version}_windows_amd64";
                    Console.WriteLine(TheFuckingLink);
                    await DownloadFile(TheFuckingLink, DownloadPath);

                }
                else
                {

                    Logger.MsgBox("唔......好像您的网络出了点问题唉，要去检查一下哦", "出现了点小问题", 2, 47, 1);
                    return;
                }
            }
            catch(Exception ex)
            {
                CrashInterception.ShowException(ex);
                return;
            }
            
        }
        public void OpenSnackbar(string title, string message, SymbolRegular icon)
        {
            Dispatcher.Invoke(() =>
            {
                Snackbar.Show(title, message, icon);
            });
        }

        private void _TitleBar_CloseClicked(object sender, RoutedEventArgs e)
        {   
            
            e.Handled = true;
            if (!isDownloadFinished)
            {
                bool isUserWannaClose = Logger.MsgBox("您确定不需要下载Frpc吗? 如果不需要的话请前往设置指定Frpc路径", "小小的警告", 2, 47, 1);
                if (isUserWannaClose)
                {
                    DownloadService.CancelAsync();
                    //Access.DashBoard.Show();
                    this.Owner = null;
                    Close();
                    Access.DashBoard.Navigation.Navigate(2);
                    
                }

            }
        }
    }
}
