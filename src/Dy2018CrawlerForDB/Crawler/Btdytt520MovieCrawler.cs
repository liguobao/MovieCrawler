using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Data;
using Dy2018CrawlerWithDB.Models;

namespace Dy2018CrawlerWithDB
{
    public class Btdytt520MovieCrawler
    {
       
        private static HtmlParser htmlParser = new HtmlParser();

        private static DataContext movieDataContent { get; } = new DataContext();

        public static void CrawlHostMovieInfo()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    #region

                    int newMovieCount = 0;

                    var indexURL = "http://www.btdytt520.com/movie/";

                    string html = GetHTMLByHTTPWebRequest(indexURL);

                    if (string.IsNullOrEmpty(html))
                        return;

                    LogHelper.Info("Btdytt520 GetHTMLByHTTPWebRequest Success!");

                    var htmlDom = htmlParser.Parse(html);
                    var divMovie = htmlDom.QuerySelector("div.index_Sidebar_cc");
                    divMovie.QuerySelectorAll("a").Select(a => a).ToList().ForEach(
                        a =>
                        {
                            var onlineURL = "http://www.btdytt520.com" + a.GetAttribute("href");
                            if (movieDataContent.Movies.FirstOrDefault(mo => mo.OnlineUrl == onlineURL) == null)
                            {
                                var movieInfo = GetMovieInfoURL(onlineURL);
                                if (movieInfo != null)
                                {
                                    movieInfo.MovieType = (int)MovieTypeEnum.Latest;
                                    movieDataContent.Add(movieInfo);
                                    newMovieCount++;
                                }
                            }

                        });
                    movieDataContent.SaveChanges();

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
                if (infoTable != null && infoTable.Length > 2)
                {
                    if (infoTable[1] != null && !string.IsNullOrEmpty(infoTable[1].TextContent))
                    {
                        DateTime.TryParse(infoTable[1].TextContent.Replace("发布时间", "").Replace("\n", ""), out pubDate);
                    }
                }
                return new MovieInfo()
                {
                    MovieName = nameDom != null ? nameDom.InnerHtml : "获取名称失败...",
                    OnlineUrl = onlineURL,
                    MovieIntro = introDom != null ? introDom.InnerHtml : "",
                    PubDate = pubDate,
                    DataCreateTime = DateTime.Now,
                    MovieType = (int)MovieTypeEnum.Latest,
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
