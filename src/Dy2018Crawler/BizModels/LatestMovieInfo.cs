using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018Crawler.Models;

namespace Dy2018Crawler
{
    /// <summary>
    /// 最新电影数据
    /// </summary>
    public class LatestMovieInfo
    {
        private static MovieInfoHelper latestMovieList = new MovieInfoHelper(Path.Combine(ConstsConf.WWWRootPath, "latestMovie.json"));

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
                    for (var i = 1; i < indexPageCount; i++)
                    {
                        try
                        {
                            var index = i == 1 ? "" : "_" + i;
                            var indexURL = $"http://www.dy2018.com/html/gndy/dyzz/index{index}.html";
                            FillMovieFromOnline(indexURL);
                        }
                        catch(Exception ex)
                        {
                            LogHelper.Error("CrawlLatestMovieInfo Exception", ex);
                        }
                       
                    }

                    LogHelper.Info("CrawlLatestMovieInfo Finish!");
                }
                catch (Exception ex)
                {
                    LogHelper.Error("CrawlLatestMovieInfo Exception", ex);
                }
            });
        }

        /// <summary>
        /// 从在线网页提取数据
        /// </summary>
        /// <param name="i"></param>
        private static void FillMovieFromOnline(string indexURL)
        {
           
            var htmlDoc = HTTPHelper.GetHTMLByURL(indexURL);
            var dom = htmlParser.Parse(htmlDoc);
            var lstDivInfo = dom.QuerySelectorAll("div.co_content8");
            if (lstDivInfo != null)
            {
                lstDivInfo.FirstOrDefault().QuerySelectorAll("a").Where(a => a.GetAttribute("href").Contains("/i/")).ToList()
                .ForEach(a =>
                {
                    var onlineURL = "http://www.dy2018.com" + a.GetAttribute("href");
                    if (!latestMovieList.IsContainsMoive(onlineURL))
                    {
                        MovieInfo movieInfo = MovieInfoHelper.GetMovieInfoFromOnlineURL(onlineURL);
                        if (movieInfo != null && movieInfo.XunLeiDownLoadURLList != null && movieInfo.XunLeiDownLoadURLList.Count != 0)
                            latestMovieList.AddToMovieDic(movieInfo);
                    }
                });
            }
        }


        /// <summary>
        /// 获取全部的电影数据
        /// </summary>
        /// <returns></returns>
        public static List<MovieInfo> GetAllMovieInfo()
        {
            return latestMovieList.GetListMoveInfo(); ;
        }

        public static MovieInfo GetMovieInfoByOnlineURL(string onlineURL)
        {
            return latestMovieList.GetMovieInfo(onlineURL);
        }
    }
}
