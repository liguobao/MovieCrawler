using System;
using System.Collections.Generic;
using System.Linq;
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

            List<MovieInfo> lstMovie = new List<MovieInfo>();

            var htmlDoc = HTTPHelper.GetHTMLByURL("http://www.dy2018.com/");
            var dom = htmlParser.Parse(htmlDoc);
            var lstDivInfo = dom.QuerySelectorAll("div.co_content222");
            if(lstDivInfo!=null)
            {
                lstMovie = lstDivInfo.FirstOrDefault().QuerySelectorAll("a").Where(a => a.GetAttribute("href").Contains("/i/")).Select(
                    a=> 
                    {
                        var onlineURL = "http://www.dy2018.com" + a.GetAttribute("href");
                        var movieHTML = HTTPHelper.GetHTMLByURL(onlineURL);
                        var movieDoc = htmlParser.Parse(movieHTML);
                        var zoom = movieDoc.GetElementById("Zoom");
                        var lstDownLoadURL = movieDoc.QuerySelectorAll("[bgcolor='#fdfddf']");
                        return new MovieInfo()
                        {
                            MovieName = a.InnerHtml,
                            Dy2018OnlineUrl = onlineURL,
                            MovieIntro = zoom!=null ?zoom.InnerHtml:"暂无介绍...",
                            XunLeiDownLoadURLList = lstDownLoadURL!=null ? 
                            lstDownLoadURL.Select(d=>d.FirstElementChild.InnerHtml).ToList():new List<string>(),
                        };
                    }).ToList();
            }



            return View(lstMovie);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
