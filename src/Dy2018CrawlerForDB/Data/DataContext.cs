using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dy2018CrawlerWithDB.Models;
using Microsoft.EntityFrameworkCore;

namespace Dy2018CrawlerWithDB.Data
{
    public class DataContext : DbContext
    {
        public DbSet<MovieInfo> Movies { set; get; }

        public DbSet<CrawlerConfigconfiguration> CrawlerConfigurations { get; set; }


        /// <summary>
        /// Server =服务器IP，database = 数据库名称 uid=账号，pwd=密码
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         => optionsBuilder
         .UseMySql(@"Server=*;database=moviecrawler;uid=***;pwd=***;CharSet=utf8;");


    }
}
