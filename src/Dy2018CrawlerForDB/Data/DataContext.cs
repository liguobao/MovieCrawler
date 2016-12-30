using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dy2018CrawlerWithDB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dy2018CrawlerWithDB.Data
{
    public class DataContext : DbContext
    {
        public DbSet<MovieInfo> Movies { set; get; }

        public DbSet<CrawlerConfigconfiguration> CrawlerConfigurations { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          => optionsBuilder
          .UseMySql(ConstsConf.MySQLConnectionString);


    }
}
