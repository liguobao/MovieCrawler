using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dy2018CrawlerForJson.Models
{
    public class MovieInfo
    {
        public string MovieName { get; set; }

        public string Dy2018OnlineUrl { get; set; }

        [JsonIgnoreAttribute]
        public string MovieIntro { get; set; }

        public DateTime PubDate { get; set; }

        public List<string> XunLeiDownLoadURLList { get; set; }

    }
}
