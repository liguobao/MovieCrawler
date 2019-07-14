using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;
using RestSharp;

namespace MovieCrawler.Crawlers
{
    public class Okzyco : BaseCrawler
    {

        public Okzyco(IOptions<AppSettings> options, MovieDapper movieDapper)
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
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
                request.AddHeader("accept-encoding", "gzip, deflate");
                request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
                request.AddHeader("upgrade-insecure-requests", "1");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("pragma", "no-cache");
                request.AddHeader("connection", "keep-alive");
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
                var onlineURL = "http://www.okzy.co" + li.QuerySelector("a").GetAttribute("href");
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