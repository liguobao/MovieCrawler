using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;
using RestSharp;

namespace MovieCrawler.Crawlers
{
    public class Btrenren : BaseCrawler
    {

        public Btrenren(IOptions<AppSettings> options, MovieDapper movieDapper)
        : base(options, movieDapper)
        {

        }
        protected static HtmlParser htmlParser = new HtmlParser();

        public override string LoadHTML(string url)
        {
            try
            {                
                var client = new RestClient(url);
                client.Timeout = 100 * 1000;
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Cookie", "bdshare_firstime=1547013531108; PHPSESSID=skp6k9v4on7mo0u9v15146d1u6; cck_lasttime=1547274547972; cck_count=0");
                request.AddHeader("Accept-Language", "zh-CN,zh;q=0.9");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");
                request.AddHeader("Upgrade-Insecure-Requests", "1");
                request.AddHeader("Cache-Control", "max-age=0");
                request.AddHeader("Connection", "keep-alive");
                IRestResponse response = client.Execute(request);   
                return response.IsSuccessful ? response.Content : "";

            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadHTML fail,url:{url},ex:{ex.ToString()}");
               
                return string.Empty;
            }

        }

        public override List<DBMovie> ParseMovies(string html)
        {
            var dom = htmlParser.Parse(html);
            var movies = new List<DBMovie>();
            var lis = dom.QuerySelectorAll("div.xing_vb")?.SelectMany(div => div.QuerySelectorAll("li"));
            foreach (var li in lis)
            {
                if (li.QuerySelector("a") == null || string.IsNullOrEmpty(li.QuerySelector(".xing_vb5")?.TextContent))
                {
                    continue;
                }
                var onlineURL = "http://www.btrenren.com" + li.QuerySelector("a").GetAttribute("href");
                DateTime.TryParse(li.QuerySelector(".xing_vb6")?.TextContent, out var publishTime);
                var movie = new DBMovie()
                {
                    Name = li.QuerySelector("a").TextContent,
                    Link = onlineURL,
                    Type = li.QuerySelector(".xing_vb5")?.TextContent,
                    PublishTime = publishTime,
                    UpdateTime = DateTime.Now
                };
                FillMovieDetail(onlineURL, movie);
                movies.Add(movie);
            }
            return movies;
        }

        protected void FillMovieDetail(string onlineURL, DBMovie movie)
        {
            var movieHTML = LoadHTML(onlineURL);
            if (!string.IsNullOrEmpty(movieHTML))
            {
                var htmlDoc = htmlParser.Parse(movieHTML);

                movie.Cover = htmlDoc?.QuerySelector("div.vodImg")?.QuerySelector("img")?.GetAttribute("src");
                var vodplayinfos = htmlDoc?.QuerySelectorAll("div.vodplayinfo");
                if (vodplayinfos.Count() > 2)
                {
                    movie.Intro = vodplayinfos[1]?.TextContent;
                    movie.DownResources = new List<Resource>();
                    if (vodplayinfos[2].QuerySelectorAll("li").Any())
                    {
                        movie.DownResources.AddRange(GetResources(vodplayinfos[2]));
                    }
                    if (vodplayinfos[3].QuerySelectorAll("li").Any())
                    {
                        movie.DownResources.AddRange(GetResources(vodplayinfos[3]));
                    }
                }

            }
        }

        private static List<Resource> GetResources(AngleSharp.Dom.IElement item)
        {
            return item.QuerySelectorAll("li").Select(li =>
            new Resource()
            {
                Link = li.QuerySelector("input").GetAttribute("value"),
                Description = li.TextContent,
            }).ToList();
        }
    }
}