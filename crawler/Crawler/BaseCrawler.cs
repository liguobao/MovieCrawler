using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;

namespace MovieCrawler.Crawlers
{
    public class BaseCrawler
    {
        private readonly AppSettings _appsetgins;

        private readonly MovieDapper _movieDapper;

        public BaseCrawler(IOptions<AppSettings> options, MovieDapper movieDapper)
        {
            _appsetgins = options.Value;
            _movieDapper = movieDapper;
        }

        public virtual string LoadHTML(string url)
        {
            throw new NotImplementedException();
        }

        public virtual List<DBMovie> ParseMovies(string html)
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
                        return;
                    }
                    var movies = this.ParseMovies(html);
                    _movieDapper.BulkInsert(movies);
                    Console.WriteLine($"crawl {host} end.");
                }
            }
            Console.WriteLine(name);
        }

    }
}