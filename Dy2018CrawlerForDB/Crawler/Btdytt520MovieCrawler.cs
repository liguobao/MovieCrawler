using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerForDB.Data;
using Dy2018CrawlerForDB.Models;
using Dy2018CrawlerForDB.Helper;

namespace Dy2018CrawlerForDB
{
    public class Btdytt520MovieCrawler
    {
       
        private static HtmlParser htmlParser = new HtmlParser();

        private static DataContext MovieDataContent { get; } = new DataContext();

        public static void CrawlHostMovieInfo()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    #region
                    int newMovieCount = 0;
                    var indexURL = "http://www.btdytt520.com/movie/";
                    var html = HTTPHelper.GetHTMLByURL(indexURL, true);
                    if (string.IsNullOrEmpty(html))
                        return;
                    var htmlDom = htmlParser.Parse(html);
                    htmlDom.QuerySelector("div.index_Sidebar_cc")
                        .QuerySelectorAll("a")
                        .ForEach(a =>
                        {
                            var onlineURL = "http://www.btdytt520.com" + a.GetAttribute("href");
                            if (!MovieDataContent.Movies.Any(mo => mo.OnlineUrl == onlineURL))
                            {
                                var movieInfo = GetMovieInfoURL(onlineURL);
                                if (movieInfo != null)
                                {
                                    movieInfo.MovieType = MovieType.Latest;
                                    MovieDataContent.Movies.Add(movieInfo);
                                    newMovieCount++;
                                }
                            }
                        });
                    MovieDataContent.SaveChanges();
                    LogHelper.Info($"Finish Btdytt520 CrawlHostMovieInfo,New Data Count:{newMovieCount}");
                    #endregion
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Btdytt520 CrawlHostMovieInfo Exception", ex);
                }
            });
        }

        private static string GetHTMLByHTTPWebRequest(string indexURL)
        {
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(indexURL);
            AddCookies(httpWebRequest);
            var html = HTTPHelper.GetHTML(httpWebRequest);
            return html;
        }

        private static void AddCookies(HttpWebRequest httpWebRequest)
        {
            httpWebRequest.CookieContainer = new CookieContainer() { };
            httpWebRequest.CookieContainer.Add(new Uri("http://www.btdytt520.com"),
                new Cookie() { Name = "JXD705135", Value = "1", Path = "/" });
            httpWebRequest.CookieContainer.Add(new Uri("http://www.btdytt520.com"),
               new Cookie() { Name = "JXD730293", Value = "1", Path = "/" });
            httpWebRequest.CookieContainer.Add(new Uri("http://www.btdytt520.com"),
               new Cookie() { Name = "JXM705135", Value = "1", Path = "/" });
            httpWebRequest.CookieContainer.Add(new Uri("http://www.btdytt520.com"),
               new Cookie() { Name = "JXM730293", Value = "1", Path = "/" });

            //httpWebRequest.CookieContainer.Add(new Uri("http://www.btdytt520.com"),
            //   new Cookie() { Name = "JXS705135	", Value = "1", Path = "/" });
            //httpWebRequest.CookieContainer.Add(new Uri("http://www.btdytt520.com"),
            // new Cookie() { Name = "JXS730293", Value = "1", Path = "/" });

            httpWebRequest.CookieContainer.Add(new Uri("http://www.btdytt520.com"),
            new Cookie() { Name = "CNZZDATA1254168247", Value = "1890928383-1481026152-null%7C1482640111", Path = "/" });
        }



        private static MovieInfo GetMovieInfoURL(string onlineURL)
        {
            try
            {
                var html = GetHTMLByHTTPWebRequest(onlineURL);
                if (string.IsNullOrEmpty(html))
                    return null;
                var htmlDom = htmlParser.Parse(html);
                var nameDom = htmlDom.QuerySelector("h1.font14");
                var introDom = htmlDom.QuerySelector("div.Drama_c");
                var infoTable = htmlDom.QuerySelectorAll("tr.CommonListCell");
                var pubDate = DateTime.Now;
                if (infoTable != null && infoTable.Length > 2 && !string.IsNullOrEmpty(infoTable[1].TextContent))
                {
                    DateTime.TryParse(infoTable[1].TextContent.Replace("发布时间", "").Replace("\n", ""), out pubDate);
                }
                return new MovieInfo()
                {
                    MovieName = nameDom?.TextContent ?? "获取名称失败...",
                    OnlineUrl = onlineURL,
                    MovieIntro = introDom?.TextContent ?? "",
                    PubDate = pubDate,
                    DataCreateTime = DateTime.Now,
                    MovieType = MovieType.Latest,
                    SoureceDomain = SoureceDomainConsts.BTdytt520,
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error(" Btdytt520 GetMovieInfoURL Exception", ex);
                return null;
            }

        }


    }
}
