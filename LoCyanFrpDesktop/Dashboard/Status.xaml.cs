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
        
    }
}
