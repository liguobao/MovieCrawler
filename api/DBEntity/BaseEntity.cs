using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;

namespace MovieCrawler.API.Dao
{

    [Serializable]

    public class BaseEntity
    {
        [Column("id")]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; } = DateTime.Now;

        public T ToModel<T>()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
        }
    }
}