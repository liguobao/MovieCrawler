using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Models;

namespace Dy2018CrawlerWithDB
{
    public class Btdytt520Helper
    {
        private static HtmlParser htmlParser = new HtmlParser();


        public static BizMovieInfo GetMovieInfoByOnlineURL(string onlineURL, bool isContainIntro = false)
        {
            var html = HTTPHelper.GetHTMLByURL(onlineURL);
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
            return new BizMovieInfo()
            {
                MovieName = nameDom != null ? nameDom.InnerHtml : "获取名称失败...",
                Dy2018OnlineUrl = onlineURL,
                MovieIntro = introDom != null && isContainIntro ? introDom.InnerHtml : "",
                PubDate = pubDate,
            };
        }

    }
}
