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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
            .UseMySql(@"Server=***;database=moviecrawler;uid=***;pwd=***;CharSet=utf8;");

    }
}
