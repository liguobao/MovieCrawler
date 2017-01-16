using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Data;
using Dy2018CrawlerWithDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dy2018CrawlerWithDB.Controllers
{
    public class HomeController : Controller
    {

        private DataContext MovieDataContent { get; } = new DataContext();

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="isRefresh"></param>
        /// <returns></returns>
        public IActionResult Index(int count = 100)
        {
            var lstDy2018HotMovie = new List<MovieInfo>();
            lstDy2018HotMovie = MovieDataContent.Movies.Where(
                   mo => mo.MovieType == MovieType.Hot
                   && mo.SoureceDomain == SoureceDomainConsts.Dy2018Domain && mo.PubDate > DateTime.Now.Date.AddYears(-1)
                   ).Take(count).ToList();
            return View(lstDy2018HotMovie);
        }


        public IActionResult Dy2018MovieList(int count = 100, int movieType = 2)
        {
            var lstDy2018HotMovie = new List<MovieInfo>();
            lstDy2018HotMovie = MovieDataContent.Movies.Where(
                   mo => mo.MovieType == movieType
                   && mo.SoureceDomain == SoureceDomainConsts.Dy2018Domain
                   ).OrderByDescending(mo => mo.PubDate).Take(count).ToList();
            return View(lstDy2018HotMovie);
        }



        public IActionResult Btdytt520HotClick(int count = 100)
        {
            var lstDy2018HotMovie = new List<MovieInfo>();
            lstDy2018HotMovie = MovieDataContent.Movies.Where(
                   mo => mo.MovieType == MovieType.Latest
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
            LogHelper.Info("Start crawling");
            Btdytt520MovieCrawler.CrawlHostMovieInfo();
            Dy2018MovieCrawler.CrawlHotMovie();
            Dy2018MovieCrawler.CrawlMovieInfo();
            LogHelper.Info("Finish crawling");
            return View();
        }

        public IActionResult ShowMoiveDetail(string onlineURL)
        {
            var movieInfo = MovieDataContent.Movies.FirstOrDefault(mo => mo.OnlineUrl == onlineURL);
            return View(movieInfo);
        }

        public IActionResult Error()
        {
            return View();
        }

       }
}
