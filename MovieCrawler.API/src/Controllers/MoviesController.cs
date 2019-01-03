using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
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


        [HttpGet("")]
        [EnableCors("APICors")]
        public ActionResult GetAll()
        {
            var page = !string.IsNullOrEmpty(HttpContext.Request.Query["page"]) ? int.Parse(HttpContext.Request.Query["page"]) : 0;
            var size = !string.IsNullOrEmpty(HttpContext.Request.Query["size"]) ? int.Parse(HttpContext.Request.Query["size"]) : 10;
            return new JsonResult(new { code = 0, data = _elasticSearch.Query("", page, size) });
        }

        [HttpGet("{name}")]
        [EnableCors("APICors")]
        public ActionResult Get(string name) => new JsonResult(new { code = 0, data = _elasticSearch.Query(name) });


        [HttpGet("search")]
        [EnableCors("APICors")]
        public ActionResult Search([FromQuery]string keyword) => new JsonResult(new { code = 0, data = _elasticSearch.Query(keyword) });
    }
}
