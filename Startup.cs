using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akeem.Web.Tools.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Akeem.Web.Tools.Services;

namespace Akeem.Web.Tools
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
            services.AddControllersWithViews();

            services.Configure<Models.ShortUrlSetting>(Configuration.GetSection("ShortUrlSetting"));
            ShortUrlSetting shortUrlSetting = new ShortUrlSetting();
            Configuration.Bind("ShortUrlSetting", shortUrlSetting);

            services.AddDbContext<Tools.Models.ToolsContext>(options => options.UseMySql(shortUrlSetting.Connection));
            services.AddScoped<UrlServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
