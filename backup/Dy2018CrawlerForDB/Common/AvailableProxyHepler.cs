using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dy2018CrawlerForDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dy2018CrawlerForDB.Helper
{

    /// <summary>
    /// 代理IP数据
    /// </summary>
    public class ProxyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("disable")]
        public string Disable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("failedCount")]
        public int FailedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("init")]
        public string Init { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("referCount")]
        public int ReferCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("score")]
        public Score Score { get; set; }
    }

    public class Score
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("avgScore")]
        public int AvgScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("failedCount")]
        public int FailedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("referCount")]
        public int ReferCount { get; set; }
    }

    public class AvailableProxy
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("btdytt520")]
        public List<ProxyInfo> Btdytt520 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("dy2018")]
        public List<ProxyInfo> Dy2018 { get; set; }

          }

    public static class AvailableProxyHepler
    {
        private static AvailableProxy _availableProxy;


        public static AvailableProxy GetAvailableProxy()
        {
            if (_availableProxy != null)
                return _availableProxy;
            try
            {
                string jsonFilePath = Path.Combine(ConstsConf.WWWRootPath, "availableProxy.json");
                string json = File.ReadAllText(jsonFilePath);
                _availableProxy = JsonConvert.DeserializeObject<AvailableProxy>(json, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = { new JavaScriptDateTimeConverter() }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAvailableProxy Exception", ex);
            }
            return _availableProxy;
        }

    }

}
