﻿using LoCyanFrpDesktop.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoCyanFrpDesktop.Utils
{
    internal static class Access
    {
        public static Status? Status { get; set; }
        public static MainWindow? MainWindow { get; set; }
        public static DashBoard? DashBoard { get; set; }
        public static Settings? Settings { get; set; }
        public static Download? Download { get; set; }
    }
}
