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
    public class Dy2018CrawlerHelper
    {
        private static  DataContext movieDataContent { get; } = new DataContext();

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
                    LogHelper.Info("Dy2018 CrawlLatestMovieInfo Start...");
                    indexPageCount = indexPageCount == 0 ? 3 : indexPageCount;
                    //取前五页
                    for (var i = 1; i < indexPageCount; i++)
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

                    }

                    LogHelper.Info("Dy2018 CrawlLatestMovieInfo Finish!");
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Dy2018 CrawlLatestMovieInfo Exception", ex);
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
                    LogHelper.Info("Dy2018 CrawlHotMovie Start...");
                    var htmlDoc = HTTPHelper.GetHTMLByURL("http://www.dy2018.com/");
                    htmlDoc = GetHTMLOnJumpWebPage(htmlDoc);
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
                                if (movieDataContent.Movies.FirstOrDefault(mo => mo.OnlineUrl == onlineURL) == null)
                                {
                                    var movieInfo = GetMovieInfoFromURL(onlineURL);
                                    if (movieInfo != null)
                                    {
                                        movieInfo.MovieType = (int)MovieTypeEnum.Latest;
                                        movieDataContent.Add(movieInfo);
                                        newMovieCount++;
                                    }

                                }
                            });
                        }
                        movieDataContent.SaveChanges();
                    }
                    LogHelper.Info($"Finish Dy2018 CrawlHotMovie,New Data Count:{newMovieCount}");

                }
                catch (Exception ex)
                {
                    LogHelper.Error("Dy2018 CrawlHotMovie Exception", ex);
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
            htmlDoc = GetHTMLOnJumpWebPage(htmlDoc);

            var dom = htmlParser.Parse(htmlDoc);
            var lstDivInfo = dom.QuerySelectorAll("div.co_content8");
            if (lstDivInfo != null && lstDivInfo.Count() > 0)
            {
                lstDivInfo.FirstOrDefault().QuerySelectorAll("a").Where(a => a.GetAttribute("href").Contains("/i/")).ToList()
                .ForEach(a =>
                {
                    var onlineURL = "http://www.dy2018.com" + a.GetAttribute("href");
                    if (movieDataContent.Movies.FirstOrDefault(mo => mo.OnlineUrl == onlineURL) == null)
                    {
                        var movieInfo = GetMovieInfoFromURL(onlineURL);
                        if (movieInfo != null)
                        {
                            movieInfo.MovieType = (int)MovieTypeEnum.Latest;
                            movieDataContent.Add(movieInfo);
                            newMovieCount++;
                        }

                    }
                });
                movieDataContent.SaveChanges();

            }
            LogHelper.Info($"Finish Dy2018 CrawlLatestMovieInfo,New Data Count:{newMovieCount},IndexURL:{indexURL}");
        }

        private static string GetHTMLOnJumpWebPage(string htmlDoc)
        {
            if (htmlDoc.Contains("window.location"))
            {
                var tempDom = htmlParser.Parse(htmlDoc);
                var scriptDom = tempDom.QuerySelector("script");
                var tempURL = "http://www.dy2018.com" + scriptDom.InnerHtml.Replace("window.location=", "")
                    .Replace("+", "").Replace("\"", "").Replace(" ", "").Replace(";", "");
                htmlDoc = HTTPHelper.GetHTMLByURL(tempURL);
                LogHelper.Info($"GetHTML From JumpURL {(string.IsNullOrEmpty(htmlDoc) ? "Success" : "Fail")}!,the URL:{tempURL}");
            }
            //LogHelper.Info(htmlDoc);
            return htmlDoc;
        }


        /// <summary>
        /// 从在线网页提取电影数据
        /// </summary>
        /// <param name="onlineURL"></param>
        /// <returns></returns>
        private static MovieInfo GetMovieInfoFromURL(string onlineURL)
        {
            try
            {
                var movieHTML = HTTPHelper.GetHTMLByURL(onlineURL);
                if (string.IsNullOrEmpty(movieHTML))
                    return null;
                movieHTML = GetHTMLOnJumpWebPage(movieHTML);

                var movieDoc = htmlParser.Parse(movieHTML);
                var zoom = movieDoc.GetElementById("Zoom");
                var lstDownLoadURL = movieDoc.QuerySelectorAll("[bgcolor='#fdfddf']");
                var updatetime = movieDoc.QuerySelector("span.updatetime"); var pubDate = DateTime.Now;
                if (updatetime != null && !string.IsNullOrEmpty(updatetime.InnerHtml))
                {
                    DateTime.TryParse(updatetime.InnerHtml.Replace("发布时间：", ""), out pubDate);
                }
                var lstURL = lstDownLoadURL.Select(a => a.QuerySelector("a")).Where(item => item != null).Select(item => item.InnerHtml).ToList();

                var movieName = movieDoc.QuerySelector("div.title_all");

                var movieInfo = new MovieInfo()
                {
                    MovieName = movieName != null && movieName.QuerySelector("h1") != null ?
                    movieName.QuerySelector("h1").InnerHtml : "找不到影片信息...",
                    OnlineUrl = onlineURL,
                    MovieIntro = zoom != null ? zoom.InnerHtml : "暂无介绍...",
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
                LogHelper.Error("Dy2018 GetMovieInfoFromURL Exception", ex, new { OnloneURL = onlineURL });
                return null;
            }

        }

    }
}
 