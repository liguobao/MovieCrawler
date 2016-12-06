using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018Crawler.Models;
using Pomelo.AspNetCore.TimedJob;

namespace Dy2018Crawler.Jobs
{
    public class AutoGetMovieListJob:Job
    {
       
        [Invoke(Begin = "2016-11-29 22:10", Interval = 1000 * 3600*3, SkipWhileExecuting =true)]
        public void Run()
        {
            LogHelper.Info("Start crawling");
            LatestMovieInfo.CrawlLatestMovieInfo(100);
            HotMovieInfo.CrawlHotMovie();
           // LogHelper.Info("Finish crawling");
        }

    
    }
}
