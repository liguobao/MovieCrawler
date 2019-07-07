using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieCrawler.API.Dao
{

    [Table("movie")]
    public class DBMovie : BaseEntity
    {

        public string Name { get; set; }

        public string Intro { get; set; }

        public string Cover { get; set; }

        public string Link { get; set; }

        public string Type { get; set; }


        public DateTime PublishTime { get; set; }

        [NotMapped]
        public List<Resource> DownResources { get
        {
            return JsonConvert.DeserializeObject<List<Resource>>(this.Resources);
        }}

        public string Resources{ get; set; }

    }

    public class Resource
    {
        public string Description { get; set; }

        public string Link { get; set; }
    }
}