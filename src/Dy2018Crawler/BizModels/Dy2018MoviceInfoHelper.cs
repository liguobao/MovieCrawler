using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018Crawler.Models;

namespace Dy2018Crawler
{
    public class Dy2018MoviceInfoHelper
    {
        private static HtmlParser htmlParser = new HtmlParser();

        /// <summary>
        /// 从在线网页提取电影数据
        /// </summary>
        /// <param name="onlineURL"></param>
        /// <returns></returns>
        public static MovieInfo GetMovieInfoFromOnlineURL(string onlineURL,bool isContainIntro=false)
        {
            try
            {
                var movieHTML = HTTPHelper.GetHTMLByURL(onlineURL);
                if (string.IsNullOrEmpty(movieHTML))
                    return null;
                var movieDoc = htmlParser.Parse(movieHTML);
                var zoom = movieDoc.GetElementById("Zoom");
                var lstDownLoadURL = movieDoc.QuerySelectorAll("[bgcolor='#fdfddf']");
                var updatetime = movieDoc.QuerySelector("span.updatetime"); var pubDate = DateTime.Now;
                if (updatetime != null && !string.IsNullOrEmpty(updatetime.InnerHtml))
                {
                    DateTime.TryParse(updatetime.InnerHtml.Replace("发布时间：", ""), out pubDate);
                }
                var lstOnlineURL = lstDownLoadURL.Select(a => a.QuerySelector("a")).Where(item => item != null).Select(item => item.InnerHtml).ToList();

                var movieName = movieDoc.QuerySelector("div.title_all");

                var movieInfo = new MovieInfo()
                {
                    MovieName = movieName != null && movieName.QuerySelector("h1") != null ?
                    movieName.QuerySelector("h1").InnerHtml : "找不到影片信息...",
                    Dy2018OnlineUrl = onlineURL,
                    MovieIntro = zoom != null && isContainIntro ? WebUtility.HtmlEncode(zoom.InnerHtml) : "暂无介绍...",
                    XunLeiDownLoadURLList = lstOnlineURL,
                    PubDate = pubDate,
                };
                return movieInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetMovieInfoFromOnlineURL Exception", ex, new { OnloneURL = onlineURL });
                return null;
            }

        }

    }
}
