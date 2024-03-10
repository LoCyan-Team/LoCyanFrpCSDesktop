using LoCyanFrpDesktop.Utils;
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
using LoCyanFrpDesktop.Dashboard;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using System.Windows.Threading;
using System.Diagnostics;

namespace LoCyanFrpDesktop.Dashboard
{
    /// <summary>
    /// Interaction logic for Status.xaml
    /// </summary>
    public partial class Status : UiPage
    {
        
        public Status()
        {
            InitializeComponent();
            Access.Status = this;
            
            lock (LogPreProcess.Process.Cache)
            {
                LogPreProcess.Process.Cache.ForEach(
                    (line) => Dispatcher.Invoke(() => Append(LogPreProcess.Color(line)))
                );
            }
            //Append(LogPreProcess.Color(LogType.Info, ProxyList.lineFiltered));
        }
        public void Append(Paragraph paragraph)
        {
            
            Dispatcher.Invoke(() =>
            {
                LogOutput.Document = LogOutput.Document ?? new();
                LogOutput.Document.Blocks.Add(paragraph);
                while (LogOutput.Document.Blocks.Count > 200)
                {
                    LogOutput.Document.Blocks.Remove(LogOutput.Document.Blocks.FirstBlock);
                }
                LogOutput.ScrollToEnd();
            }, System.Windows.Threading.DispatcherPriority.Background);

        }


        private void StopAllProxies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int i = ProxyList.PNAPList.Count();
                for (int j = 0;j < i; j++)
                {
                    Process.GetProcessById(ProxyList.PNAPList[j].Pid).Kill();
                    ProxyList.PNAPList[j].IsRunning = false;
                }
            }catch { 
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    Verb = "runas",
                    Arguments = "taskkill /f /im frpc.exe"
                });
            }
        }
    }
}
