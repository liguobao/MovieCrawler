using System;
using System.Collections.Generic;
using System.Linq;
using Dy2018CrawlerWithDB.Data;
using Dy2018CrawlerWithDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dy2018CrawlerWithDB.Controllers
{
    public class HomeController : Controller
    {

        private DataContext movieDataContent { get; } = new DataContext();

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="isRefresh"></param>
        /// <returns></returns>
        public IActionResult Index()
        {
            var lstDy2018HotMovie = new List<MovieInfo>();
            lstDy2018HotMovie = movieDataContent.Movies.Where(
                   mo => mo.MovieType == (int)MovieTypeEnum.Hot
                   && mo.SoureceDomain == SoureceDomainConsts.Dy2018Domain && mo.PubDate > DateTime.Now.Date.AddDays(-30)
                   ).ToList();
            return View(lstDy2018HotMovie);
        }

        /// <summary>
        /// 最新电影
        /// </summary>
        /// <returns></returns>
        public IActionResult LatestMovieList(int count=100)
        {
            var lstDy2018HotMovie = new List<MovieInfo>();
            lstDy2018HotMovie = movieDataContent.Movies.Where(
                   mo => mo.MovieType == (int)MovieTypeEnum.Latest
                   && mo.SoureceDomain == SoureceDomainConsts.Dy2018Domain
                   ).OrderByDescending(mo=>mo.PubDate).Take(count).ToList();
            return View(lstDy2018HotMovie);
        }

        public IActionResult Btdytt520HotClick(int count = 100)
        {
            var lstDy2018HotMovie = new List<MovieInfo>();
            lstDy2018HotMovie = movieDataContent.Movies.Where(
                   mo => mo.MovieType == (int)MovieTypeEnum.Latest
                   && mo.SoureceDomain == SoureceDomainConsts.BTdytt520
                   ).OrderByDescending(mo => mo.PubDate).Take(count).ToList();
            return View(lstDy2018HotMovie);
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
          
            return View();
        }

        public IActionResult ShowMoiveDetail(string onlineURL)
        {
            var movieInfo = movieDataContent.Movies.FirstOrDefault(mo => mo.OnlineUrl == onlineURL);
            return View(movieInfo);
        }

        public IActionResult Error()
        {
            return View();
        }

       }
}
