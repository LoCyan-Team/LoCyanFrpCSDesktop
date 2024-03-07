using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoCyanFrpDesktop.Extensions
{
    internal static partial class JToken
    {
        public static string ToJson(this object obj, Formatting formatting)
            => obj != null ? JsonConvert.SerializeObject(obj, formatting) : string.Empty;

        public static string ToJson(this object obj)
            => obj?.ToJson(Formatting.None) ?? string.Empty;
    }
}
