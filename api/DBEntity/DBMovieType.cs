using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieCrawler.API.Dao
{

    [Table("movie_type")]
    public class DBMovieType : BaseEntity
    {
        public string Name { get; set; }
    }

}