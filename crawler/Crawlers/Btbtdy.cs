using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;
using RestSharp;

namespace MovieCrawler.Crawlers
{
    public class Btbtdy : BaseCrawler
    {

        public Btbtdy(IOptions<AppSettings> options, MovieDapper movieDapper)
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
            var aList = dom.QuerySelectorAll("div.liimg")?.SelectMany(div => div.QuerySelectorAll("a"))
                .Where(a => a.GetAttribute("href").StartsWith("/btdy/"));
            var movies = aList?.Select(a =>
            {
                var onlineURL = "http://www.btbtdy.tv" + a.GetAttribute("href");
                var movie = new DBMovie()
                {
                    Name = a.GetAttribute("title"),
                    Type = dom.QuerySelector("p.des").TextContent,
                    Link = onlineURL,
                    UpdateTime = DateTime.Now
                };
                FillMovieDetail(onlineURL, movie);
                return movie;
            }).ToList();
            return movies;
        }

        protected void FillMovieDetail(string onlineURL, DBMovie movie)
        {
            var movieHTML = LoadHTML(onlineURL);
            if (!string.IsNullOrEmpty(movieHTML))
            {
                var htmlDoc = htmlParser.Parse(movieHTML);
                movie.PublishTime = DateTime.Parse(htmlDoc?.QuerySelector("div.vod_intro").QuerySelector("dd").TextContent);
                movie.Cover = htmlDoc?.QuerySelector("div.vod_img")?.QuerySelector("img")?.GetAttribute("src");
                movie.Intro = htmlDoc?.QuerySelector("div.des")?.InnerHtml;
                if (htmlDoc.QuerySelectorAll("div.p_list").Any())
                {
                    var downURL = new Uri(onlineURL).PathAndQuery.Replace("btdy", "vidlist").Replace("dy", "");
                    var downHTML = LoadHTML("http://www.btbtdy.tv" + downURL);
                    movie.DownResources = FindResources(htmlParser.Parse(downHTML));
                }
            }
        }

        private static List<Resource> FindResources(AngleSharp.Dom.Html.IHtmlDocument htmlDoc)
        {
            var resources = new List<Resource>();
            foreach (var li in htmlDoc.QuerySelectorAll("div.p_list").SelectMany(l => l.QuerySelectorAll("li")))
            {
                if (li.QuerySelector("a") != null && li.QuerySelector("span") != null)
                {
                    resources.Add(new Resource()
                    {
                        Description = li.QuerySelector("a").GetAttribute("title"),
                        Link = li.QuerySelector("a.d1").GetAttribute("href"),
                    });
                }
            }

            return resources;
        }
    }
}