using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dy2018CrawlerWithDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dy2018Crawler.Helper
{

    
    public class ProxyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string disable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int failedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string init { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int referCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Score score { get; set; }
    }

    public class Score
    {
        /// <summary>
        /// 
        /// </summary>
        public int avgScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int failedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int referCount { get; set; }
    }

    public class AvailableProxy
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ProxyInfo> btdytt520 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ProxyInfo> dy2018 { get; set; }

          }

    public static class AvailableProxyHepler
    {
        private static AvailableProxy _availableProxy;


        public static AvailableProxy GetAvailableProxy()
        {
            if (_availableProxy != null)
                return _availableProxy;
            string jsonFilePath = Path.Combine(ConstsConf.WWWRootPath, "availableProxy.json");
            using (var stream = new FileStream(jsonFilePath, FileMode.OpenOrCreate))
            {
                try
                {
                    StreamReader sr = new StreamReader(stream);
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new JavaScriptDateTimeConverter() }
                    };
                    //构建Json.net的读取流  
                    using (var reader = new JsonTextReader(sr))
                    {
                        _availableProxy = serializer.Deserialize<AvailableProxy>(reader);

                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error("GetAvailableProxy Exception", ex);
                }
            }
            return _availableProxy;
        }

    }

}
