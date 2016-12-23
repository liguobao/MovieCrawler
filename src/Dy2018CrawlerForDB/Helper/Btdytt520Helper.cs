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
       
        public static MovieInfo GetMovieInfoURL(string onlineURL)
        {
            var html = HTTPHelper.GetHTMLByURL(onlineURL,true);
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
            return new MovieInfo()
            {
                MovieName = nameDom != null ? nameDom.InnerHtml : "获取名称失败...",
                OnlineUrl = onlineURL,
                MovieIntro = introDom != null ? introDom.InnerHtml : "",
                PubDate = pubDate,
                DataCreateTime =DateTime.Now,
                MovieType = (int)MovieTypeEnum.Latest,
                SoureceDomain = SoureceDomainConsts.BTdytt520,
            };
        }

    }
}
