﻿using LoCyanFrpDesktop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

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
        public static List<string> Tips = new() {
            "Tips:他们说下载的时候把电脑抱起来摇匀, 下载速度会更快哦",
            "Tips:LocyanFrp永远不会跑路, 就像你家楼下清仓甩卖的店一样",
            "Tips:有的时候其实都不算是bug, 其实是我们特意写的特性 (确信",
            "Tips:你说的对,但是LocyanFrp是由Daiyangcheng女士主导的一款...",
            "Tips:如果你遇到了连不上API的情况, 那这边建议先换台新电脑呢亲",
            "Tips:你需要客服? 你是指LCF做开发的这几个暴躁老姐吗",
            "Tips:你染上LCF了? 你给哥们说实话, 你真的染上LCF了?",
            "Tips:我们要组一辈子LocyanFrp!",
            "Tips:不是, 哥们, 你确定真的要启动吗",
            "Tips:再急, 再急就给你Crash了"

        };
    }
}
