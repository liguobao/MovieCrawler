using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MovieCrawler.API.Common;
using MovieCrawler.API.Crawler;

namespace MovieCrawler.API.Controllers
{

    [ApiController]
    [Route("v1/[controller]/")]
    public class CrawlerController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public CrawlerController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("{name}")]
        public ActionResult Get(string name)
        {
            var crawler = _serviceProvider.GetServices<BaseCrawler>().FirstOrDefault(c => c.GetType().Name.ToLower() == name);
            if (crawler != null)
            {
                LogHelper.RunActionTaskNotThrowEx(() =>
                {
                    crawler.Run();
                }, name);
                return new JsonResult(new { msg = $"{name} running." });
            }
            return new JsonResult(new { msg = $"{name} not found." });

        }
    }
}
