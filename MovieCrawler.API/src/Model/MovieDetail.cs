using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieCrawler.API.Crawler;
using Nest;

namespace MovieCrawler.API.Model
{
    [ElasticsearchType(IdProperty = "Link")]
    public class MovieDetail
    {
        public string Name { get; set; }

        public string Intro { get; set; }

        public string Cover { get; set; }

        public string Link { get; set; }


        public DateTime PublishTime { get; set; }


        public DateTime UpdateTime { get; set; }

        public List<Resource> DownResources { get; set; }
    }

    public class Resource
    {
        public string Description { get; set; }

        public string Link { get; set; }
    }


}
