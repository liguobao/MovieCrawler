using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieCrawler.API.Model;
using MovieCrawler.API.Service;

namespace MovieCrawler.API.Crawler
{
    public class BaseCrawler
    {
        private readonly AppSettings _appsetgins;

        private readonly ElasticService _elasticService;
        public BaseCrawler(IOptions<AppSettings> options, ElasticService elasticService)
        {
            _appsetgins = options.Value;
            _elasticService = elasticService;
        }

        public virtual string LoadHTML(string url)
        {
            throw new NotImplementedException();
        }

        public virtual List<MovieDetail> ParseMovies(string html)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            var name = this.GetType().Name.ToLower();
            foreach (var config in _appsetgins.CrawlerConfigs.Where(c => c.Name == name.ToLower()))
            {
                foreach (var host in config.Hosts)
                {
                    Console.WriteLine($"crawl {host} start.");
                    var html = this.LoadHTML(host);
                    if (string.IsNullOrEmpty(html))
                    {
                        continue;
                    }
                    var movies = this.ParseMovies(html);
                    _elasticService.Save(movies);
                    Console.WriteLine($"crawl {host} end.");

                }
            }
            Console.WriteLine(name);
        }

    }
}