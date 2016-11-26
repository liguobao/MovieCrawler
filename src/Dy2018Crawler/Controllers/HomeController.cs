using System;
using System.Collections.Generic;
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

        private HtmlParser htmlParser = new HtmlParser();

        public IActionResult Index()
        {
            List<MovieInfo> lstMovie = MovieInfoJsonHelper.GetListMoveInfo();
            AddToMovieList();
            return View(lstMovie);
        }


        public void AddToMovieList()
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
                                if (!MovieInfoJsonHelper.IsContainsMoive(onlineURL))
                                {
                                    var movieHTML = HTTPHelper.GetHTMLByURL(onlineURL);
                                    var movieDoc = htmlParser.Parse(movieHTML);
                                    var zoom = movieDoc.GetElementById("Zoom");
                                    var lstDownLoadURL = movieDoc.QuerySelectorAll("[bgcolor='#fdfddf']");
                                    var movieInfo = new MovieInfo()
                                    {
                                        MovieName = a.InnerHtml,
                                        Dy2018OnlineUrl = onlineURL,
                                        MovieIntro = zoom != null ? WebUtility.HtmlEncode(zoom.InnerHtml) : "暂无介绍...",
                                        XunLeiDownLoadURLList = lstDownLoadURL != null ?
                                        lstDownLoadURL.Select(d => d.FirstElementChild.InnerHtml).ToList() : null,
                                    };
                                    if (movieInfo.XunLeiDownLoadURLList != null && movieInfo.XunLeiDownLoadURLList.Count != 0)
                                        MovieInfoJsonHelper.AddToMovieDic(movieInfo);
                                }
                            });
                        }
                    }

                }
                catch
                {

                }
            });
          
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            MovieInfoJsonHelper.WriteToJsonFile(true);

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
