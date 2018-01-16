using Pomelo.AspNetCore.TimedJob;

namespace Dy2018CrawlerForJson.Jobs
{
    public class AutoGetMovieListJob:Job
    {
       
        [Invoke(Begin = "2018-01-16 00:30", Interval = 1000 * 3600*3, SkipWhileExecuting =true)]
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
