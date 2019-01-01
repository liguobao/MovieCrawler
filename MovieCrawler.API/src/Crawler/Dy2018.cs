using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.API.Model;
using MovieCrawler.API.Service;
using RestSharp;

namespace MovieCrawler.API.Crawler
{
    public class Dy2018 : BaseCrawler
    {

        public Dy2018(IOptions<AppSettings> options, ElasticService elasticService)
        : base(options, elasticService)
        {

        }
        protected static HtmlParser htmlParser = new HtmlParser();

        public override string LoadHTML(string url)
        {
            try
            {
                System.Net.WebRequest wRequest = System.Net.WebRequest.Create(url);
                wRequest.Headers.Add("authority", "www.dy2018.com");
                wRequest.Headers.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
                wRequest.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
                wRequest.ContentType = "text/html; charset=gb2312";
                wRequest.Method = "get";
                wRequest.UseDefaultCredentials = true;
                var task = wRequest.GetResponseAsync();
                System.Net.WebResponse wResp = task.Result;
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding("GB2312")))
                {
                    return reader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"url:{url},ex:{ex.ToString()}");
                return string.Empty;
            }

        }

        public override List<MovieDetail> ParseMovies(string html)
        {
            var dom = htmlParser.Parse(html);
            var aList = dom.QuerySelectorAll("div.co_content222")?.SelectMany(div => div.QuerySelectorAll("a"))
                .Where(a => a.GetAttribute("href").StartsWith("/i/"));
            var movies = aList?.Select(a =>
            {
                var onlineURL = "https://www.dy2018.com" + a.GetAttribute("href");
                var movie = new MovieDetail()
                {
                    Name = a.TextContent,
                    Link = onlineURL,
                };

                FillMovieDetail(onlineURL, movie);
                return movie;
            }).ToList();
            return movies;
        }

        protected void FillMovieDetail(string onlineURL, MovieDetail movie)
        {
            var movieHTML = LoadHTML(onlineURL);
            if (!string.IsNullOrEmpty(movieHTML))
            {
                var htmlDoc = htmlParser.Parse(movieHTML);
                movie.UpdateTime = htmlDoc?.QuerySelector("span.updatetime")?.TextContent?.Replace("发布时间：", "");
                movie.Cover = htmlDoc?.QuerySelector("div.co_content8")?.QuerySelector("img")?.GetAttribute("src");
                movie.Intro = htmlDoc?.QuerySelector("#Zoom")?.InnerHtml;
                if (htmlDoc.QuerySelectorAll("table").Any())
                {
                    List<string> resources = FindResources(htmlDoc);
                    movie.Resources = resources;
                }
            }
        }

        private static List<string> FindResources(AngleSharp.Dom.Html.IHtmlDocument htmlDoc)
        {
            var resources = new List<string>();
            foreach (var tb in htmlDoc.QuerySelectorAll("table"))
            {
                if (tb.QuerySelector("anchor") != null)
                {
                    resources.Add(tb.QuerySelector("anchor").GetAttribute("pkqdhpef"));
                    resources.Add(tb.QuerySelector("anchor").TextContent);
                }
                else if (tb.QuerySelector("a") != null && !tb.QuerySelector("a").GetAttribute("href").Contains("html"))
                {
                    resources.Add(tb.QuerySelector("a").TextContent);
                }
            }

            return resources;
        }
    }
}