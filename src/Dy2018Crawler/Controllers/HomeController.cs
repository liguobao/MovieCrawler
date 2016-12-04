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

        private static HtmlParser htmlParser = new HtmlParser();

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="isRefresh"></param>
        /// <returns></returns>
        public IActionResult Index(int isRefresh = 0)
        {
            List<MovieInfo> lstMovie = hotMovieList.GetListMoveInfo();
            return View(lstMovie);
        }

        /// <summary>
        /// 最新电影
        /// </summary>
        /// <param name="isRefresh"></param>
        /// <param name="indexPageCount"></param>
        /// <returns></returns>
        public IActionResult LatestMovieList(int isRefresh = 0, int indexPageCount = 0)
        {
            List<MovieInfo> lstMovie = latestMovieList.GetListMoveInfo();
            if (isRefresh != 0)
            {
                AddToLatestMovieList(isRefresh);
            }
            return View(lstMovie);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <returns></returns>
        public IActionResult Receiver()
        {
            return View();
        }

        /// <summary>
        /// 刷新当前数据
        /// </summary>
        /// <returns></returns>
        public IActionResult RefreshMovie()
        {
            AddToHotMovieList();
            AddToLatestMovieList();
            return View();
        }

        /// <summary>
        /// 显示电影详情
        /// </summary>
        /// <param name="onlineURL"></param>
        /// <returns></returns>
        public IActionResult ShowLatestMoiveInfo(string onlineURL)
        {
            return View(MovieInfoHelper.GetMovieInfoFromOnlineURL(onlineURL));
        }

        public IActionResult ShowHotMoiveInfo(string onlineURL)
        {
            return View(MovieInfoHelper.GetMovieInfoFromOnlineURL(onlineURL));
        }


        public IActionResult ShowMoiveDetail(string onlineURL)
        {
            var movieInfo = MovieInfoHelper.GetMovieInfoFromOnlineURL(onlineURL);
            if(movieInfo==null)
            {
               var  lasestMovieInfo = latestMovieList.GetMovieInfo(onlineURL);
               var hotMovieInfo = hotMovieList.GetMovieInfo(onlineURL);
                if (lasestMovieInfo != null)
                    movieInfo = lasestMovieInfo;
                else if(hotMovieInfo!=null)
                    movieInfo = hotMovieInfo;
            }
            return View(movieInfo);
        }


        public IActionResult Error()
        {
            return View();
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
                                    MovieInfo movieInfo = MovieInfoHelper.GetMovieInfoFromOnlineURL(onlineURL);
                                    if (movieInfo!=null&&movieInfo.XunLeiDownLoadURLList != null && movieInfo.XunLeiDownLoadURLList.Count != 0)
                                        hotMovieList.AddToMovieDic(movieInfo);
                                }
                            });
                        }
                    }

                }
                catch(Exception ex)
                {
                    LogHelper.Error("AddToHotMovieList Exception", ex);
                }
            });
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
                                    MovieInfo movieInfo = MovieInfoHelper.GetMovieInfoFromOnlineURL(onlineURL);
                                    if (movieInfo!=null&&movieInfo.XunLeiDownLoadURLList != null && movieInfo.XunLeiDownLoadURLList.Count != 0)
                                        latestMovieList.AddToMovieDic(movieInfo);
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("AddToLatestMovieList Exception", ex);
                }
            });
        }

         }
}
