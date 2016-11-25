using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dy2018Crawler.Models
{
    public class MovieInfo
    {
        public string MovieName { get; set; }

        public string Dy2018OnlineUrl { get; set; }

        public string MovieIntro { get; set; }

        public List<string> XunLeiDownLoadURLList { get; set; }

    }
}
