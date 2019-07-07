
using System.Collections.Generic;

namespace MovieCrawler
{
    public class AppSettings
    {
        public string MySQLString {get;set;}
        public List<CrawlerConfig> CrawlerConfigs { get; set; }


    }

    public class CrawlerConfig
    {
        public string Name { get; set; }

        public List<string> Hosts { get; set; }
    }

}
