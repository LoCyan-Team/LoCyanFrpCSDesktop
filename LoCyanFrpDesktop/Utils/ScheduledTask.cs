using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoCyanFrpDesktop.Utils
{
    class ScheduledTask
    {
        public static void Init()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    using (ConfigManager ConfigManager = new(FileMode.Create))
                    {
                        Console.WriteLine("Saving...");
                    }
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
