using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace MovieCrawler.API.Dao
{
    public class MovieCrawlerDbContext : DbContext
    {
        public MovieCrawlerDbContext(DbContextOptions<MovieCrawlerDbContext> options)
            : base(options)
        {
        }
        public DbSet<DBMovie> Movies { get; set; }

         public DbSet<DBMovieType> MovieTypes { get; set; }

    }
}
