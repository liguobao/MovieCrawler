using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieCrawler.API.Dao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json.Linq;
using RestSharp;
using MovieCrawler.API.Common;

namespace MovieCrawler.API.Controllers
{
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieCrawlerDbContext _dbContext;

        public MovieController(MovieCrawlerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("api/v1/movies")]
        [SwaggerResponse(200, "", typeof(MovieResponse))]
        public ActionResult GetMovies([FromQuery, SwaggerParameter("关键字")]string keyword = "", 
        [FromQuery, SwaggerParameter("类型")]string type = "", [FromQuery, SwaggerParameter("页码")]int page = 0,
        [FromQuery, SwaggerParameter("分页数量")]int size = 10)
        {
            var query = _dbContext.Movies.AsQueryable();
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(q => q.Type == type);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.Name.Contains(keyword));
            }
            var result = query.OrderByDescending(m => m.CreateTime).Skip(page * size).Take(size).ToList();
            return Ok(new
            {
                code = 0,
                data = new MovieResponse()
                {
                    count = query.Count(),
                    result = result
                }
            });
        }

        [HttpGet("api/v1/movies/{id}")]
        [SwaggerResponse(200, "", typeof(DBMovie))]
        public ActionResult GetOneMovie([FromRoute, SwaggerParameter("电影Id")]long id)
        {
            return Ok(new { code = 0, data =  _dbContext.Movies.FirstOrDefault(m =>m.Id == id)});
        }


    }

    class MovieResponse
    {
        public int count;
        public List<DBMovie> result;
    }
}
