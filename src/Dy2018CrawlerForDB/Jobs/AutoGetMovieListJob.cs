using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Dy2018CrawlerWithDB.Models;
using Pomelo.AspNetCore.TimedJob;

namespace Dy2018CrawlerWithDB.Jobs
{
    public class AutoGetMovieListJob:Job
    {
       
        [Invoke(Begin = "2016-01-15 9:30", Interval = 1000 * 3600*6, SkipWhileExecuting =true)]
        public void Run()
        {
            LogHelper.Info("Start crawling");
            Btdytt520MovieCrawler.CrawlHostMovieInfo();
            Dy2018MovieCrawler.CrawlHotMovie();
            Dy2018MovieCrawler.CrawlMovieInfo();
            LogHelper.Info("Finish crawling");
        }

    
    }
}
