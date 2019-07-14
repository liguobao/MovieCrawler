using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;
using RestSharp;

namespace MovieCrawler.Crawlers
{
    public class Btkat : BaseCrawler
    {

        public Btkat(IOptions<AppSettings> options, MovieDapper movieDapper)
        : base(options, movieDapper)
        {

        }
        protected static HtmlParser htmlParser = new HtmlParser();

        public override string LoadHTML(string url)
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                client.Timeout = 100 * 1000;
                request.AddHeader("cache-control", "no-cache");
                 request.AddHeader("Accept-Language", "zh-CN,zh;q=0.9");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Referer", "http://www.btkat.com/list/");
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
            var lis = dom.QuerySelectorAll("div.ml")?.SelectMany(div => div.QuerySelectorAll("div.title"));
            foreach (var li in lis)
            {
                if (li.QuerySelector("a") == null )
                {
                    continue;
                }
                var onlineURL = "http:"+li.QuerySelector("a").GetAttribute("href");
                DateTime.TryParse(li.QuerySelector("des")?.TextContent, out var publishTime);
                var movie = new DBMovie()
                {
                    Name = li.QuerySelector("b").TextContent,
                    Link = onlineURL,
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
                var li=htmlDoc?.QuerySelector("ul.moviedteail_list").QuerySelectorAll("li");
                movie.Type=li[1].TextContent;
                movie.Intro=htmlDoc?.QuerySelector("div.ysinfo").TextContent;
                movie.Cover = htmlDoc?.QuerySelector("div.moviedteail_img")?.QuerySelector("a")?.QuerySelector("img")?.GetAttribute("src");
                var vodplayinfos = htmlDoc?.QuerySelectorAll("div.tinfo");
                movie.DownResources = new List<Resource>(); 
                if(vodplayinfos.Count()>1)
                {
                    movie.DownResources.Add(new Resource()
                    {
                        Link=vodplayinfos[0]?.QuerySelector("a").GetAttribute("href"),
                        Description=vodplayinfos[0].QuerySelector("p").TextContent,
                    });
                    movie.DownResources.Add(new Resource()
                    {
                        Link=vodplayinfos[1]?.QuerySelector("a").GetAttribute("href"),
                        Description=vodplayinfos[1].QuerySelector("p").TextContent,
                    });
                }
                else if(vodplayinfos.Count()>1)
                {
                    movie.DownResources.Add(new Resource()
                    {
                        Link=vodplayinfos[0]?.QuerySelector("a").GetAttribute("href"),
                        Description=vodplayinfos[0].QuerySelector("p").TextContent,
                    });
                }
                
            }
        }

    }
}