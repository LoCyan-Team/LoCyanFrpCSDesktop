using LoCyanFrpDesktop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoCyanFrpDesktop
{
    internal static class Global
    {
        public static readonly bool DebugMode = Properties.Settings.Default.DebugMode;
        public static bool LoginedByConsole = false;
        public const string Version = "2.1.0";
        public const string Branch = "Beta";
        public const int Revision = 1;
        public static readonly BuildInfo BuildInfo = new();
        public const string Developer = "Shiroiame-Kusu & Daiyangcheng";
        public const string Copyright = "Copyright © 2021 - 2024 杭州樱芸网络科技有限公司 All Rights Reserved";
    }
}
