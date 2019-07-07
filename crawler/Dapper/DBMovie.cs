using System;
using System.Collections.Generic;
using Nest;
using Newtonsoft.Json.Linq;

namespace MovieCrawler.Dao
{
    [ElasticsearchType(IdProperty = "Link")]
    public class DBMovie
    {
        public string Name { get; set; }

        public string Intro { get; set; }

        public string Cover { get; set; }

        public string Link { get; set; }

        public string Type { get; set; }


        public DateTime PublishTime { get; set; }


        public DateTime UpdateTime { get; set; }

         public DateTime CreateTime { get; set; }

        public List<Resource> DownResources { get; set; }

        public string Resources {
            get
            {
                return JToken.FromObject(this.DownResources).ToString();
            }}
    }

    public class Resource
    {
        public string Description { get; set; }

        public string Link { get; set; }
    }


}
