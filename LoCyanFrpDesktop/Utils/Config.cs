using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoCyanFrpDesktop.Utils
{
    
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class Config
    {
        public string Token = "";
        public string LoginToken = "";
        public string Username = "";
        //public string Password = "";
        public string FrpToken = "";
        public string FrpcPath = "";
        public bool DebugMode = false;

    }
}
