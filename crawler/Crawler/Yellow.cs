using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;
using RestSharp;

namespace MovieCrawler.Crawlers
{
    public class Yellow : BaseCrawler
    {

        public Yellow(IOptions<AppSettings> options, MovieDapper movieDapper)
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
                request.AddHeader("cookie", "__cfduid=d1f999af24142fe052d324bc084d4b0581547477703; _ga=GA1.2.855729766.1547477711; _gid=GA1.2.415865825.1547477711; Hm_lvt_427f72ce75b0677eb10f24419484eb80=1547477711,1547477844; playss=7; Hm_lpvt_427f72ce75b0677eb10f24419484eb80=1547478646");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");
                request.AddHeader("upgrade-insecure-requests", "1");
                request.AddHeader("referer", "https://www.617ii.com/");
                request.AddHeader("cache-control", "max-age=0,no-cache");
                request.AddHeader("authority", "www.617ii.com");
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
            var lis = dom.QuerySelectorAll("#tpl-img-content")?.SelectMany(div => div.QuerySelectorAll("li"));
            foreach (var li in lis)
            {
              
                var onlineURL = "https://www.617ii.com" + li.QuerySelector("a").GetAttribute("href");
                DateTime.TryParse(li.QuerySelector("span.down_date c_red")?.TextContent, out var publishTime);
                var movie = new DBMovie()
                {
                    Name = li.QuerySelector("h3").TextContent,
                    Link = onlineURL,
                    Type = "18禁",
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

                movie.Cover = htmlDoc?.QuerySelector("img.lazy")?.GetAttribute("data-original");
                var vodplayinfos = htmlDoc?.QuerySelectorAll("tr");
                if (vodplayinfos.Count() > 2)
                {
                    movie.DownResources = new List<Resource>();
                    movie.DownResources.AddRange(GetResources(vodplayinfos[0])); 
                    movie.DownResources.AddRange(GetResources(vodplayinfos[1]));
                    movie.DownResources.AddRange(GetResources(vodplayinfos[2]));
                    // movie.DownResources.AddRange(GetResourcesLINK(vodplayinfos[3]));   
                    // movie.DownResources.AddRange(GetResourcesLINK(vodplayinfos[4]));
                    
                }
            }
        }
        private static List<Resource> GetResources(AngleSharp.Dom.IElement item)
        {
            return item.QuerySelectorAll("td").Select(td =>
            new Resource()
            {
                Description ="https://www.617ii.com"+ td.QuerySelector("a").GetAttribute("href"),
                //Link=""+td.QuerySelector("input").GetAttribute("value"),
            }).ToList();
        }
         private static List<Resource> GetResourcesLINK(AngleSharp.Dom.IElement item)
        {
            return item.QuerySelectorAll("td").Select(td =>
            new Resource()
            {
                Link=""+td.QuerySelector("input").GetAttribute("value"),
            }).ToList();
        }
         

     
    }
}