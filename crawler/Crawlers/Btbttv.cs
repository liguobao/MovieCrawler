using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;
using RestSharp;

namespace MovieCrawler.Crawlers
{
    public class Btbttv : BaseCrawler
    {

        public Btbttv(IOptions<AppSettings> options,MovieDapper movieDapper)
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
                request.AddHeader("Cookie", "UM_distinctid=168463812da34e-0def2a16921313-8383268-144000-168463812db344; CNZZDATA1275862394=159304514-1547364685-^%^7C1547538394");
                request.AddHeader("Accept-Language", "zh-CN,zh;q=0.9");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Referer", "http://m.btbttv.cc/dy1/jiebanren1997/");
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
            var lis = dom.QuerySelectorAll("#data_list")?.SelectMany(div => div.QuerySelectorAll("li"));
            foreach (var li in lis)
            {
               
                var onlineURL = "http://m.btbttv.net/" + li.QuerySelector("a").GetAttribute("href");
                DateTime.TryParse(li.QuerySelector("span.sNum")?.TextContent, out var publishTime);
                var movie = new DBMovie()
                {
                    Cover=li.QuerySelector("img").GetAttribute("data-src"),
                    Name = li.QuerySelector("span.sTit").TextContent,
                    Link = onlineURL,
                    PublishTime = publishTime,
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now,
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
                movie.Intro=htmlDoc?.QuerySelector("p.movieintro").TextContent;
                var DownURL = "http://m.btbttv.net/" + htmlDoc?.QuerySelector(".tabCon")?.QuerySelector("a").GetAttribute("href");
                movie.DownResources= GetResources(DownURL,movie);
            }
        }

        protected List<Resource> GetResources(string onlineURL, DBMovie movie)
        {
            var resources = new List<Resource>();
            var movieHTML = LoadHTML(onlineURL);
             if (!string.IsNullOrEmpty(movieHTML))
            {
                var htmlDoc = htmlParser.Parse(movieHTML);
                resources.Add(new Resource()
                    {
                        Description = htmlDoc.QuerySelector("h1").TextContent,
                        Link = htmlDoc.QuerySelector("span.tab").QuerySelector("a").GetAttribute("href"),
                    });
            }
            return resources;
        }
    }
}