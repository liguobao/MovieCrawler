using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerForJson.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dy2018CrawlerForJson.Controllers
{
    public class HomeController : Controller
    {

       
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="isRefresh"></param>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<MovieInfo> lstMovie = HotMovieInfo.GetAllMovieInfo();
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
            List<MovieInfo> lstMovie = LatestMovieInfo.GetAllMovieInfo();
            if (isRefresh != 0)
            {
                LatestMovieInfo.CrawlLatestMovieInfo(indexPageCount);
            }
            return View(lstMovie);
        }

        public IActionResult Btdytt520HotClick()
        {
            // Btdytt520MoviceInfo.CrawlHostMovieInfo();
            // Btdytt520HotClickHelper.CrawlHotClickMovieInfo();
            

            return View(Btdytt520HotClickHelper.GetAllMovieInfo());
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
            LatestMovieInfo.CrawlLatestMovieInfo();
            HotMovieInfo.CrawlHotMovie();
            return View();
        }

        public IActionResult ShowMoiveDetail(string onlineURL)
        {
            var movieInfo = Dy2018MoviceInfoHelper.GetMovieInfoFromOnlineURL(onlineURL,true);
            if(movieInfo==null)
            {
               var  lasestMovieInfo = LatestMovieInfo.GetMovieInfoByOnlineURL(onlineURL);
               var hotMovieInfo =HotMovieInfo.GetMovieInfoByOnlineURL (onlineURL);
                if (lasestMovieInfo != null)
                    movieInfo = lasestMovieInfo;
                else if(hotMovieInfo!=null)
                    movieInfo = hotMovieInfo;
            }
            return View(movieInfo);
        }



        public IActionResult ShowBtdytt520MovieInfo(string onlineURL)
        {
            ViewBag.OnlineURL = onlineURL;
            return View(Btdytt520Helper.GetMovieInfoByOnlineURL(onlineURL,true));
        }

        public IActionResult Error()
        {
            return View();
        }


        
       }
}
