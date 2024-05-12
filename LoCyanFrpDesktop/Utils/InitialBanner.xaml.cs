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
using Wpf.Ui;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Appearance;
using System.Threading;

namespace LoCyanFrpDesktop.Utils
{
    /// <summary>
    /// Interaction logic for Init.xaml
    /// </summary>
    public partial class InitialBanner : UiWindow
    {
        public InitialBanner()
        {
            InitializeComponent();
            Access.InitialBanner = this;
            int a;
            a = Random.Shared.Next(0, Global.Tips.Count);
            if (a < Global.Tips.Count)
            {
                Tips.Text = Global.Tips[a];
            }
            string? Status;
            if (CheckForUpdate(out Status))
            {
                Output.Text = "检测到更新，如果没有自动更新记得自己去更新哦~";
                Update.Init();
            }
            else
            {
                if (string.IsNullOrEmpty(Status))
                {
                    Output.Text = "没有检查到更新哦,";
                }
                else
                {
                    Output.Text = $"更新失败, 原因: {Status}";
                }


            }



        }
        public bool CheckForUpdate(out string reason)
        {   bool a = false;
            string Reason;
            Update.CheckVersion(out a,out Reason);
            reason = Reason;
            return a;
        }
        
    }
}
