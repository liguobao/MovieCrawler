
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace MovieCrawler.API.Model
{
    public class AppSettings
    {
        public string ESUserName { get; set; }
        public string ESPassword { get; set; }
        public string ESURL { get; set; }

        public List<CrawlerConfig> CrawlerConfigs { get; set; }


    }

    public class CrawlerConfig
    {
        public string Name { get; set; }

        public List<string> Hosts { get; set; }
    }

}
