using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;
using Newtonsoft.Json.Linq;

namespace MovieCrawler.Crawlers
{
    public class CommonCrawler
    {
        private readonly AppSettings _appsetgins;

        private readonly MovieDapper _movieDapper;

        protected static HtmlParser htmlParser = new HtmlParser();

        public CommonCrawler(IOptions<AppSettings> options, MovieDapper movieDapper)
        {
            _appsetgins = options.Value;
            _movieDapper = movieDapper;
        }

        public virtual string LoadHTML(string url)
        {
            throw new NotImplementedException();
        }

        public virtual List<DBMovie> ParseMovies(string html, string resultConfigs="")
        {
           var htmlDoc = htmlParser.Parse(html);
           
            throw new NotImplementedException();
        }

        public void Run()
        {

            var name = this.GetType().Name.ToLower();
            Console.WriteLine($"{name}爬虫开始");
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
                    Console.WriteLine($"crawl {host} end,movies count:{movies.Count}");
                }
            }
            // 每天隔着6个小时Job执行
            if (DateTime.Now.Hour % 6 == 0)
            {
                var result = _movieDapper.SyncMovieTypes();
                Console.WriteLine($"SyncMovieTypes success, count:{result}");
            }


        }

    }
}