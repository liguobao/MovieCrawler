using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Data;
using Dy2018CrawlerWithDB.Models;
using Dy2018CrawlerForDB.Helper;
using AngleSharp.Dom;

namespace Dy2018CrawlerWithDB
{
    public class Dy2018MovieCrawler
    {
        private static  DataContext MovieDataContent { get; } = new DataContext();

        private static HtmlParser htmlParser = new HtmlParser();


        /// <summary>
        /// 爬取数据
        /// </summary>
        /// <param name="indexPageCount"></param>
        public static void CrawlMovieInfo()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var  indexPageCount = MovieDataContent.CrawlerConfigurations
                    .FirstOrDefault(c=>c.ConfigconfigurationName== ConstsConf.Dy2018CrawlerPageCount).ConfigconfigurationKey;
                    LogHelper.Info($"Dy2018 Crawl MovieInfo Start,PageCount:{indexPageCount}...");
                    MovieDataContent.CrawlerConfigurations.Where(c => c.IsEnabled && c.ConfigconfigurationName == ConstsConf.Dy2018CrawlerList)
                    .ForEach(config =>
                    {
                        //取前五页
                        Enumerable.Range(1, 5).ForEach(i =>
                        {
                            try
                            {
                                var index = i == 1 ? "" : "_" + i;
                                var indexURL = $"{config.ConfigconfigurationValue}index{index}.html";
                                CrawlerMovieInfoFromOnline(indexURL, config.ConfigconfigurationKey);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("Crawl MovieInfo Exception", ex);
                            }

                        });
                    });       
                    LogHelper.Info("Dy2018 Crawl MovieInfo Finish!");
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Dy2018 Crawl MovieInfo Exception", ex);
                }
            });
        }


        /// <summary>
        /// 爬取数据
        /// </summary>
        public static void CrawlHotMovie()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var newMovieCount = 0;
                    LogHelper.Info("Dy2018 CrawlHotMovie Start...");
                    var htmlDoc = HTTPHelper.GetHTMLByURL("http://www.dy2018.com/");
                    htmlDoc = GetHTMLOnJumpWebPage(htmlDoc);
                    var dom = htmlParser.Parse(htmlDoc);
                    dom.QuerySelectorAll("div.co_content222")
                    ?.Take(3)
                    .Select(divInfo => divInfo.QuerySelectorAll("a").Where(a => a.GetAttribute("href").StartsWith("/i/")))
                    .Aggregate((IEnumerable<IElement> a, IEnumerable<IElement> b) => a.Concat(b))
                    .ForEach(a =>
                    {
                        var onlineURL = "http://www.dy2018.com" + a.GetAttribute("href");
                        if (!MovieDataContent.Movies.Any(mo => mo.OnlineUrl == onlineURL))
                        {
                            var movieInfo = GetMovieInfoFromURL(onlineURL);
                            if (movieInfo != null)
                            {
                                movieInfo.MovieType = MovieType.Latest;
                                MovieDataContent.Movies.Add(movieInfo);
                                newMovieCount++;
                            }
                        }
                    });
                    MovieDataContent.SaveChanges();
                    LogHelper.Info($"Finish Dy2018 CrawlHotMovie,New Data Count:{newMovieCount}");

                }
                catch (Exception ex)
                {
                    LogHelper.Error("Dy2018 CrawlHotMovie Exception", ex);
                }
            });
        }


        /// <summary>
        /// 从在线网页提取数据
        /// </summary>
        /// <param name="i"></param>
        private static void CrawlerMovieInfoFromOnline(string indexURL, int movieType)
        {
            var newMovieCount = 0;
            var htmlDoc = HTTPHelper.GetHTMLByURL(indexURL);
            var dom = htmlParser.Parse(htmlDoc);
            dom.QuerySelector("div.co_content8")
                ?.QuerySelectorAll("a")
                .Where(a => a.GetAttribute("href").StartsWith("/i/"))
                .ForEach(a =>
                {
                    var onlineURL = "http://www.dy2018.com" + a.GetAttribute("href");
                    if (!MovieDataContent.Movies.Any(mo => mo.OnlineUrl == onlineURL))
                    {
                        var movieInfo = GetMovieInfoFromURL(onlineURL);
                        if (movieInfo != null)
                        {
                            movieInfo.MovieType = MovieType.Latest;
                            MovieDataContent.Movies.Add(movieInfo);
                            newMovieCount++;
                        }

                    }
                });
            MovieDataContent.SaveChanges();
            LogHelper.Info($"Finish Dy2018 Crawl {movieType.ToString()}MovieInfo,New Data Count:{newMovieCount},IndexURL:{indexURL}");
        }

        private static string GetHTMLOnJumpWebPage(string htmlDoc)
        {
            if (htmlDoc.Contains("window.location"))
            {
                var tempDom = htmlParser.Parse(htmlDoc);
                var scriptDom = tempDom.QuerySelector("script");
                var tempURL = "http://www.dy2018.com" + scriptDom.InnerHtml.Replace("window.location=", "")
                    .Replace("+", "").Replace("\"", "").Replace(" ", "").Replace(";", "");
                htmlDoc = HTTPHelper.GetHTMLByURL(tempURL);
                LogHelper.Info($"GetHTML From JumpURL {(string.IsNullOrEmpty(htmlDoc) ? "Success" : "Fail")}!,the URL:{tempURL}");
            }
            //LogHelper.Info(htmlDoc);
            return htmlDoc;
        }


        /// <summary>
        /// 从在线网页提取电影数据
        /// </summary>
        /// <param name="onlineURL"></param>
        /// <returns></returns>
        private static MovieInfo GetMovieInfoFromURL(string onlineURL)
        {
            try
            {
                var movieHTML = HTTPHelper.GetHTMLByURL(onlineURL);
                if (string.IsNullOrEmpty(movieHTML))
                    return null;
                var movieDoc = htmlParser.Parse(movieHTML);
                var zoom = movieDoc.GetElementById("Zoom");
                var lstDownLoadURL = movieDoc.QuerySelectorAll("[bgcolor='#fdfddf']");
                var updatetime = movieDoc.QuerySelector("span.updatetime");
                var pubDate = DateTime.Now;
                if (!string.IsNullOrEmpty(updatetime?.TextContent))
                {
                    DateTime.TryParse(updatetime.TextContent.Replace("发布时间：", ""), out pubDate);
                }
                var lstURL = lstDownLoadURL.Select(a => a.QuerySelector("a")?.TextContent ?? "");
                var movieName = movieDoc.QuerySelector("div.title_all")?.QuerySelector("h1");
                var movieInfo = new MovieInfo()
                {
                    MovieName = movieName.TextContent ?? "找不到影片信息...",
                    OnlineUrl = onlineURL,
                    MovieIntro = zoom?.TextContent ?? "暂无介绍...",
                    DownLoadURLList = string.Join(";", lstURL),
                    PubDate = pubDate.Date,
                    DataCreateTime = DateTime.Now,
                    SoureceDomain = SoureceDomainConsts.Dy2018Domain,
                    //MovieType=(int)MovieTypeEnum.Latest
                };
                return movieInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error("Dy2018 GetMovieInfoFromURL Exception", ex, new { OnloneURL = onlineURL });
                return null;
            }

        }

    }
}
 