using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.API.Common;
using MovieCrawler.API.Model;
using MovieCrawler.API.Service;
using RestSharp;

namespace MovieCrawler.API.Crawler
{
    public class Zyvip : BaseCrawler
    {

        public Zyvip(IOptions<AppSettings> options, ElasticService elasticService)
        : base(options, elasticService)
        {

        }
        protected static HtmlParser htmlParser = new HtmlParser();

        public override string LoadHTML(string url)
        {
            try
            {
                var client = new RestClient(url);
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
                LogHelper.Error(url, ex);
                return string.Empty;
            }

        }

        public override List<MovieDetail> ParseMovies(string html)
        {
            var dom = htmlParser.Parse(html);
            var movies = new List<MovieDetail>();
            var lis = dom.QuerySelectorAll("div.xing_vb")?.SelectMany(div => div.QuerySelectorAll("li"));
            foreach (var li in lis)
            {
                if (li.QuerySelector("a") == null || string.IsNullOrEmpty(li.QuerySelector(".xing_vb5")?.TextContent))
                {
                    continue;
                }
                var onlineURL = "http://131zy.vip" + li.QuerySelector("a").GetAttribute("href");
                DateTime.TryParse(li.QuerySelector(".xing_vb6")?.TextContent, out var publishTime);
                var movie = new MovieDetail()
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

        protected void FillMovieDetail(string onlineURL, MovieDetail movie)
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