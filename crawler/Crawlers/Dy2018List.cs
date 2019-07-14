using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using MovieCrawler.Dao;


namespace MovieCrawler.Crawlers
{
    public class Dy2018List : Dy2018
    {

        public Dy2018List(IOptions<AppSettings> options, MovieDapper movieDapper)
        : base(options, movieDapper)
        {

        }

        public override List<DBMovie> ParseMovies(string html)
        {
            var dom = htmlParser.Parse(html);
            var aList = dom.QuerySelectorAll("div.co_content8")?.SelectMany(div => div.QuerySelectorAll("a"))
                .Where(a => a.GetAttribute("href").StartsWith("/i/"));

            var movies = aList?.Select(a =>
           {
               var onlineURL = "https://www.dy2018.com" + a.GetAttribute("href");
               var movie = new DBMovie()
               {
                   Name = a.TextContent,
                   Link = onlineURL,
                   UpdateTime = DateTime.Now,
                   PublishTime =DateTime.Now,
               };
               FillMovieDetail(onlineURL, movie);
               return movie;
           }).ToList();
            return movies;
        }
    }
}