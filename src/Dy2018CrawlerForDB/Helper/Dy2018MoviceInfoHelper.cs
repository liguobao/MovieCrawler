using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Models;

namespace Dy2018CrawlerWithDB
{
    public class Dy2018MoviceInfoHelper
    {
        private static HtmlParser htmlParser = new HtmlParser();

      
        /// <summary>
        /// 从在线网页提取电影数据
        /// </summary>
        /// <param name="onlineURL"></param>
        /// <returns></returns>
        public static MovieInfo GetMovieInfoFromURL(string onlineURL)
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
                    MovieName = movieName.TextContent ??  "找不到影片信息...",
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
                LogHelper.Error("GetMovieInfoFromURL Exception", ex, new { OnloneURL = onlineURL });
                return null;
            }

        }

       
        
    }
}
