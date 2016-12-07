using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018Crawler.Models;

namespace Dy2018Crawler
{
    public class Btdytt520MoviceInfo
    {
        private static MovieInfoHelper btdytt520MoviceHelper = new MovieInfoHelper(Path.Combine(ConstsConf.WWWRootPath, "btdytt520Movice.json"));

        private static HtmlParser htmlParser = new HtmlParser();

        public static void CrawlLatestMovieInfo()
        {
            var indexURL = "http://www.btdytt520.com/movie/";
            var html = HTTPHelper.GetHTMLByURL(indexURL);
            if (string.IsNullOrEmpty(html))
                return;
            var htmlDom = htmlParser.Parse(html);
            var divMovie = htmlDom.QuerySelector("div.index_Sidebar_cc");
           var lstMovie = divMovie.QuerySelectorAll("a").Select(a => new MovieInfo()
           {
               Dy2018OnlineUrl = "http://www.btdytt520.com/" + a.GetAttribute("href"),
               MovieName = a.InnerHtml
           }).ToList();
        }


        /// <summary>
        /// 获取全部的电影数据
        /// </summary>
        /// <returns></returns>
        public static List<MovieInfo> GetAllMovieInfo()
        {
            return btdytt520MoviceHelper.GetListMoveInfo(); ;
        }



        public static MovieInfo GetMovieInfoByOnlineURL(string onlineURL)
        {
            return btdytt520MoviceHelper.GetMovieInfo(onlineURL);
        }

    }
}
