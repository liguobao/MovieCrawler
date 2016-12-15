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
        
        public long Id { get; set; }

        [MaxLength(200)]
        public string MovieName { get; set; }

        [MaxLength(200)]
        public string OnlineUrl { get; set; }

        [MaxLength(65400)]
        public string MovieIntro { get; set; }

        public DateTime PubDate { get; set; }

        public int MovieType { get; set; }

        [MaxLength(200)]
        public string SoureceDomain { get; set; }

        [MaxLength(1000)]
        public string XunLeiDownLoadURLList { get; set; }
    }


    public enum MovieTypeEnum
    {
        /// <summary>
        /// 热门
        /// </summary>
        Hot = 1,
        /// <summary>
        /// 最新
        /// </summary>
        Latest = 2,
    }

    public class SoureceDomainConsts
    {
        public const string Dy2018Domain = "www.dy2018.com";

        public const string BTdytt520 = "www.btdytt520.com";

    }
}
