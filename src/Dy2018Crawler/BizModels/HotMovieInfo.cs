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
    /// 热门电影数据
    /// </summary>
    public class HotMovieInfo
    {
        private static MovieInfoHelper hotMovieList = new MovieInfoHelper(Path.Combine(ConstsConf.WWWRootPath, "hotMovie.json"));

        private static HtmlParser htmlParser = new HtmlParser();
        
        /// <summary>
        /// 爬取数据
        /// </summary>
        public static void CrawlHotMovie()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    LogHelper.Info("CrawlHotMovie Start...");
                    var htmlDoc = HTTPHelper.GetHTMLByURL("http://www.dy2018.com/");
                    var dom = htmlParser.Parse(htmlDoc);
                    var lstDivInfo = dom.QuerySelectorAll("div.co_content222");
                    if (lstDivInfo != null)
                    {
                        //前三个DIV为新电影
                        foreach (var divInfo in lstDivInfo.Take(3))
                        {
                            divInfo.QuerySelectorAll("a").Where(a => a.GetAttribute("href").Contains("/i/")).ToList().ForEach(
                            a =>
                            {
                                var onlineURL = "http://www.dy2018.com" + a.GetAttribute("href");
                                if (!hotMovieList.IsContainsMoive(onlineURL))
                                {
                                    MovieInfo movieInfo = MovieInfoHelper.GetMovieInfoFromOnlineURL(onlineURL);
                                    if (movieInfo != null && movieInfo.XunLeiDownLoadURLList != null && movieInfo.XunLeiDownLoadURLList.Count != 0)
                                        hotMovieList.AddToMovieDic(movieInfo);
                                }
                            });
                        }
                    }

                    LogHelper.Info("CrawlHotMovie Finish...");

                }
                catch (Exception ex)
                {
                    LogHelper.Error("CrawlHotMovie Exception", ex);
                }
            });
        }

        /// <summary>
        /// 获取全部的电影数据
        /// </summary>
        /// <returns></returns>
        public static List<MovieInfo> GetAllMovieInfo()
        {
            return hotMovieList.GetListMoveInfo(); ;
        }



        public static MovieInfo GetMovieInfoByOnlineURL(string onlineURL)
        {
            return hotMovieList.GetMovieInfo(onlineURL);
        }
    }
}
