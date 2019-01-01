using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Options;
using MovieCrawler.API.Model;
using MovieCrawler.API.Service;
using RestSharp;

namespace MovieCrawler.API.Crawler
{
    public class Dy2018List : Dy2018
    {

        public Dy2018List(IOptions<AppSettings> options, ElasticService elasticService)
        : base(options, elasticService)
        {

        }

        public override List<MovieDetail> ParseMovies(string html)
        {
            var dom = htmlParser.Parse(html);
            var aList = dom.QuerySelectorAll("div.co_content8")?.SelectMany(div => div.QuerySelectorAll("a"))
                .Where(a => a.GetAttribute("href").StartsWith("/i/"));

            var movies = aList?.Select(a =>
           {
               var onlineURL = "https://www.dy2018.com" + a.GetAttribute("href");
               var movie = new MovieDetail()
               {
                   Name = a.TextContent,
                   Link = onlineURL,
               };
               FillMovieDetail(onlineURL, movie);
               return movie;
           }).ToList();
            return movies;
        }
    }
}