using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieCrawler.API.Crawler;
using MovieCrawler.API.Model;
using MovieCrawler.API.Service;
using NLog.Web;

namespace MovieCrawler.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOptions().Configure<AppSettings>(Configuration);
            services.AddSingleton<ElasticService, ElasticService>();
            services.AddScoped<BaseCrawler, Dy2018>();
            services.AddScoped<BaseCrawler, Dy2018List>();
            services.AddScoped<BaseCrawler, Btbtdy>();




            services.AddScoped<ElasticService, ElasticService>();

            //添加cors 服务
            services.AddCors(o => o.AddPolicy("APICors", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            env.ConfigureNLog("nlog.config");
            app.UseMvc();
        }
    }
}
