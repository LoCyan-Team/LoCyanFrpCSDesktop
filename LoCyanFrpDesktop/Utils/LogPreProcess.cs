﻿using HandyControl.Interactivity;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using RegExp = System.Text.RegularExpressions;

namespace LoCyanFrpDesktop.Utils
{
    internal static class LogPreProcess
    {
        public static string Filter(string input)
        {
            string result = RegExp.Regex.Replace(input, @"\x1b\[.*?m", string.Empty);
            result = RegExp.Regex.Replace(result, @"\x1b", string.Empty);
            //Console.WriteLine(result);
            StringBuilder stringBuilder = new();
            for (int i = 0; i < result.Length; i++)
            {
                int unicode = result[i];
                if (unicode > 31 && unicode != 127)
                {
                    stringBuilder.Append(result[i].ToString());
                }
            }
            return stringBuilder.ToString();
        }
        public static Paragraph Color(LogType type, string text)
        {
            Paragraph paragraph = new()
            {
                Margin = new(0, 0, 0, 0),
                FontFamily = new("Consolas,微软雅黑")
            };
            paragraph.Inlines.Add(text);
            return paragraph.Highlight(text);

        }
        public static Paragraph Color((LogType, string) line) => Color(line.Item1, line.Item2);
        //public static Paragraph Color((LogType, string) line) => Color(line.Item1, line.Item2);

        private static Paragraph Highlight(this Paragraph paragraph, string line)
        {
            foreach (string words in RegExp.Regex.Split(Filter(line), @"(?<=\b)(info|warn(ing)?|error|debug|\d{5,}|true|false)(?=\b)", RegExp.RegexOptions.IgnoreCase))
            {
                Run run = new(words);
                run.Foreground = words.ToLowerInvariant() switch
                {
                    "info" => Brushes.MediumTurquoise,
                    "warn" => Brushes.Gold,
                    "warning" => Brushes.Gold,
                    "error" => Brushes.Crimson,
                    "debug" => Brushes.DarkOrchid,
                    "true" => Brushes.YellowGreen,
                    "false" => Brushes.Tomato,
                    _ => RegExp.Regex.IsMatch(words, @"^\d+$") ? Brushes.Teal : MainWindow.DarkThemeEnabled ? Brushes.White : Brushes.Black,
                    //_ => Global.Settings.Serein.UseDarkTheme ? Brushes.White : Brushes.Black ,
                };
                paragraph.Inlines.Add(run);
            }
            return paragraph;
        }
    }
    internal enum LogType
    {
        Undefined,
        Info,
        Warn,
        Error,
        Debug,
        DetailDebug
    }
}
