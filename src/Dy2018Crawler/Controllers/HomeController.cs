using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018Crawler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dy2018Crawler.Controllers
{
    public class HomeController : Controller
    {

        private static MovieInfoHelper latestMovieList = new MovieInfoHelper(Path.Combine(ConstsConf.WWWRootPath, "latestMovie.json"));

        private static MovieInfoHelper hotMovieList = new MovieInfoHelper(Path.Combine(ConstsConf.WWWRootPath, "hotMovie.json"));

        private HtmlParser htmlParser = new HtmlParser();

        public IActionResult Index(int isRefresh = 0)
        {
            List<MovieInfo> lstMovie = hotMovieList.GetListMoveInfo();
            if(isRefresh!=0)
            {
                AddToHotMovieList();
            }
            return View(lstMovie);
        }

        private void AddToHotMovieList()
        {
            Task.Factory.StartNew(()=> 
            {
                try
                {
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
                                    MovieInfo movieInfo = FillMovieInfoFormWeb(a, onlineURL);
                                    if (movieInfo.XunLeiDownLoadURLList != null && movieInfo.XunLeiDownLoadURLList.Count != 0)
                                        hotMovieList.AddToMovieDic(movieInfo);
                                }
                            });
                        }
                    }

                }
                catch(Exception ex)
                {

                }
            });
        }

        private MovieInfo FillMovieInfoFormWeb(AngleSharp.Dom.IElement a, string onlineURL)
        {
            var movieHTML = HTTPHelper.GetHTMLByURL(onlineURL);
            var movieDoc = htmlParser.Parse(movieHTML);
            var zoom = movieDoc.GetElementById("Zoom");
            var lstDownLoadURL = movieDoc.QuerySelectorAll("[bgcolor='#fdfddf']");
            var updatetime = movieDoc.QuerySelector("span.updatetime"); var pubDate = DateTime.Now;
            if(updatetime!=null && !string.IsNullOrEmpty(updatetime.InnerHtml))
            {
                DateTime.TryParse(updatetime.InnerHtml.Replace("发布时间：", ""), out pubDate);
            }
            

            var movieInfo = new MovieInfo()
            {
                MovieName = a.InnerHtml.Replace("<font color=\"#0c9000\">","").Replace("<font color=\"	#0c9000\">","").Replace("</font>", ""),
                Dy2018OnlineUrl = onlineURL,
                MovieIntro = zoom != null ? WebUtility.HtmlEncode(zoom.InnerHtml) : "暂无介绍...",
                XunLeiDownLoadURLList = lstDownLoadURL != null ?
                lstDownLoadURL.Select(d => d.FirstElementChild.InnerHtml).ToList() : null,
                PubDate = pubDate,
            };
            return movieInfo;
        }

        public IActionResult LatestMovieList(int isRefresh=0,int indexPageCount=0)
        {
            List<MovieInfo> lstMovie = latestMovieList.GetListMoveInfo();
            if(isRefresh!=0)
            {
                AddToLatestMovieList(isRefresh);
            }
            return View(lstMovie);
        }

        private void AddToLatestMovieList(int indexPageCount=0)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    indexPageCount = indexPageCount == 0 ? 3 : indexPageCount;
                    //取前五页
                    for (var i=1;i< indexPageCount; i++)
                    {
                        var index = i == 1 ? "" : "_" + i;
                        var indexURL = $"http://www.dy2018.com/html/gndy/dyzz/index{index}.html";
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
                                    MovieInfo movieInfo = FillMovieInfoFormWeb(a, onlineURL);
                                    if (movieInfo.XunLeiDownLoadURLList != null && movieInfo.XunLeiDownLoadURLList.Count != 0)
                                        latestMovieList.AddToMovieDic(movieInfo);
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }

        public IActionResult Receiver()
        {
            return View();
        }

        public IActionResult RefreshMovie()
        {
            AddToHotMovieList();
            AddToLatestMovieList();
            return View();
        }

        public IActionResult ShowLatestMoiveInfo(string onlineURL)
        {
            return View(latestMovieList.GetMovieInfo(onlineURL));
        }

        public IActionResult ShowHotMoiveInfo(string onlineURL)
        {
            return View(hotMovieList.GetMovieInfo(onlineURL));
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
