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
    public class Dy2018CrawlerHelper
    {
        private static  DataContext MovieDataContent { get; } = new DataContext();

        private static HtmlParser htmlParser = new HtmlParser();


        /// <summary>
        /// 爬取数据
        /// </summary>
        /// <param name="indexPageCount"></param>
        public static void CrawlLatestMovieInfo(int indexPageCount = 0)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    LogHelper.Info("CrawlLatestMovieInfo Start...");
                    indexPageCount = indexPageCount == 0 ? 3 : indexPageCount;
                    //取前五页
                    Enumerable.Range(1, 5).ForEach(i =>
                    {
                        try
                        {
                            var index = i == 1 ? "" : "_" + i;
                            var indexURL = $"http://www.dy2018.com/html/gndy/dyzz/index{index}.html";
                            CrawlerMovieInfoFromOnline(indexURL);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("CrawlLatestMovieInfo Exception", ex);
                        }
                    });
                    LogHelper.Info("CrawlLatestMovieInfo Finish!");
                }
                catch (Exception ex)
                {
                    LogHelper.Error("CrawlLatestMovieInfo Exception", ex);
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
                    LogHelper.Info("CrawlHotMovie Start...");
                    var htmlDoc = HTTPHelper.GetHTMLByURL("http://www.dy2018.com/");
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
                            var movieInfo = Dy2018MoviceInfoHelper.GetMovieInfoFromURL(onlineURL);
                            if (movieInfo != null)
                            {
                                movieInfo.MovieType = MovieType.Latest;
                                MovieDataContent.Movies.Add(movieInfo);
                                newMovieCount++;
                            }
                        }
                    });
                    MovieDataContent.SaveChanges();
                    LogHelper.Info($"Finish Dy2018 CrawlerMovieInfoFromOnline,New Data Count:{newMovieCount}");
                    LogHelper.Info("CrawlHotMovie Finish...");
                }
                catch (Exception ex)
                {
                    LogHelper.Error("CrawlHotMovie Exception", ex);
                }
            });
        }


        /// <summary>
        /// 从在线网页提取数据
        /// </summary>
        /// <param name="i"></param>
        private static void CrawlerMovieInfoFromOnline(string indexURL)
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
                    if(!MovieDataContent.Movies.Any(mo=>mo.OnlineUrl == onlineURL))
                    {
                        var movieInfo = Dy2018MoviceInfoHelper.GetMovieInfoFromURL(onlineURL);
                        if(movieInfo!=null)
                        {
                            movieInfo.MovieType = MovieType.Latest;
                            MovieDataContent.Movies.Add(movieInfo);
                            newMovieCount++;
                        }
                        
                    }
                });
                MovieDataContent.SaveChanges();
            LogHelper.Info($"Finish Dy2018 CrawlerMovieInfoFromOnline,New Data Count:{newMovieCount}");
        }
    }
}
 