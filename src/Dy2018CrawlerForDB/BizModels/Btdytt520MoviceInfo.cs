using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Models;

namespace Dy2018CrawlerWithDB
{
    public class Btdytt520MoviceInfo
    {
        private static MovieInfoHelper hotMoviceHelper = new MovieInfoHelper(Path.Combine(ConstsConf.WWWRootPath, "btdytt520HotMovice.json"));

        private static HtmlParser htmlParser = new HtmlParser();

        public static void CrawlHostMovieInfo()
        {
            var indexURL = "http://www.btdytt520.com/movie/";
            var html = HTTPHelper.GetHTMLByURL(indexURL);
            if (string.IsNullOrEmpty(html))
                return;
            var htmlDom = htmlParser.Parse(html);
            var divMovie = htmlDom.QuerySelector("div.index_Sidebar_cc");
            divMovie.QuerySelectorAll("a").Select(a => a).ToList().ForEach(
                a =>
                {
                    var aURL = "http://www.btdytt520.com" + a.GetAttribute("href");
                    if(!hotMoviceHelper.IsContainsMoive(aURL))
                    {
                        hotMoviceHelper.AddToMovieDic(Btdytt520Helper.GetMovieInfoByOnlineURL(aURL));
                    }
                    
                });
        }

        /// <summary>
        /// 获取全部的电影数据
        /// </summary>
        /// <returns></returns>
        public static List<BizMovieInfo> GetAllMovieInfo()
        {
            return hotMoviceHelper.GetListMoveInfo();
        }


    }
}
