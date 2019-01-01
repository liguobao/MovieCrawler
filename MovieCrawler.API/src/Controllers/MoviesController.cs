using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MovieCrawler.API.Crawler;
using MovieCrawler.API.Service;

namespace MovieCrawler.API.Controllers
{

    [ApiController]
    [Route("v1/[controller]/")]
    public class MoviesController : ControllerBase
    {
        private readonly ElasticService _elasticSearch;

        public MoviesController(ElasticService elasticSearch)
        {
            _elasticSearch = elasticSearch;
        }

        [HttpGet("{name}")]
        public ActionResult Get(string name)
        {
            return new JsonResult(new { code = 0, data = _elasticSearch.Query(name) });
        }


        [HttpGet("search")]
        public ActionResult Search([FromQuery]string keyword)
        {
            return new JsonResult(new { code = 0, data = _elasticSearch.Query(keyword) });
        }
    }
}
