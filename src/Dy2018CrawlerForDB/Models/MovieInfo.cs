using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dy2018CrawlerWithDB.Data;

namespace Dy2018CrawlerWithDB.Models
{
    public class MovieInfo
    {

        public Guid Id { get; set; }

        [MaxLength(512)]
        public string MovieName { get; set; }

        [MaxLength(512)]
        public string OnlineUrl { get; set; }


        public string MovieIntro { get; set; }


        public DateTime PubDate { get; set; }

        public int MovieType { get; set; }

        [MaxLength(512)]
        public string SoureceDomain { get; set; }

        [MaxLength(1024)]
        public string DownLoadURLList { get; set; }

        public DateTime DataCreateTime { get; set; }
    }


    public class MovieType
    {
        /// <summary>
        /// 热门
        /// </summary>
        public static int Hot { get; } = 1;
        /// <summary>
        /// 最新
        /// </summary>
        public static int Latest { get; } = 2;
    }

  
}
