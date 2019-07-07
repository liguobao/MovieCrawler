using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using MovieCrawler.Crawlers;
using MovieCrawler.Dao;

namespace MovieCrawler
{
    class Program
    {
        public static IConfiguration Configuration;

        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var services = new ServiceCollection();
            InitConfiguration(environmentName, services);
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var crawlName = Environment.GetEnvironmentVariable("CRAWL_NAME");
            var crawler = serviceProvider.GetServices<BaseCrawler>()
            .FirstOrDefault(c => c.GetType().Name.ToLower() == crawlName?.ToUpper());
            if (crawler == null)
            {
                Console.WriteLine($"crawler:{crawlName} not found!");
                return;
            }
            crawler.Run();
        }

        private static void InitConfiguration(string environmentName, ServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                        .AddEnvironmentVariables().Build();
            services.AddOptions().Configure<AppSettings>(x => Configuration.Bind(x));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        private static void ConfigureServices(IServiceCollection services)
        {
            InitDI(services);

        }







        private static void InitDI(IServiceCollection services)
        {
            #region Mapper
            services.AddScoped<MovieDapper>();
            services.AddScoped<BaseDapper>();

            #endregion Service

            #region Crawler



            services.AddScoped<BaseCrawler, Dy2018>();
            services.AddScoped<BaseCrawler, Dy2018List>();
            services.AddScoped<BaseCrawler, Btbtdy>();
            services.AddScoped<BaseCrawler, Okzyco>();
            services.AddScoped<BaseCrawler, Btrenren>();//debug
            services.AddScoped<BaseCrawler, Zyvip>();//debug
            services.AddScoped<BaseCrawler, Btbttv>();//debug 
            services.AddScoped<BaseCrawler, Btkat>();//OK
            services.AddScoped<BaseCrawler, Yellow>();//OK
            #endregion

        }

    }
}
