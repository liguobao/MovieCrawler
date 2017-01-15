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
       
        [Invoke(Begin = "2017-01-15 00:30", Interval = 1000 * 3600*3, SkipWhileExecuting =true)]
        public void Run()
        {
            LogHelper.Info("Start crawling");
            LatestMovieInfo.CrawlLatestMovieInfo(100);
            HotMovieInfo.CrawlHotMovie();
            Btdytt520HotClickHelper.CrawlHotClickMovieInfo();
           // LogHelper.Info("Finish crawling");
        }

    
    }
}
