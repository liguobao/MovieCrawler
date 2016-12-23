using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Data;
using Dy2018CrawlerWithDB.Models;

namespace Dy2018CrawlerWithDB
{
    public class Btdytt520CrawlerHelper
    {
       
        private static HtmlParser htmlParser = new HtmlParser();

        private static DataContext movieDataContent { get; } = new DataContext();

        public static void CrawlHostMovieInfo()
        {
            int newMovieCount = 0;

            var indexURL = "http://www.btdytt520.com/movie/";
            var html = HTTPHelper.GetHTMLByURL(indexURL,true);
            if (string.IsNullOrEmpty(html))
                return;
            var htmlDom = htmlParser.Parse(html);
            var divMovie = htmlDom.QuerySelector("div.index_Sidebar_cc");
            divMovie.QuerySelectorAll("a").Select(a => a).ToList().ForEach(
                a =>
                {
                    var onlineURL = "http://www.btdytt520.com" + a.GetAttribute("href");
                    if (movieDataContent.Movies.FirstOrDefault(mo => mo.OnlineUrl == onlineURL) == null)
                    {
                        var movieInfo = Btdytt520Helper.GetMovieInfoURL(onlineURL);
                        if (movieInfo != null)
                        {
                            movieInfo.MovieType = (int)MovieTypeEnum.Latest;
                            movieDataContent.Movies.Add(movieInfo);
                            newMovieCount++;
                        }
                    }

                });
            movieDataContent.SaveChanges();

            LogHelper.Info($"Finish Btdytt520 CrawlHostMovieInfo,New Data Count:{newMovieCount}");

        }

    }
}
