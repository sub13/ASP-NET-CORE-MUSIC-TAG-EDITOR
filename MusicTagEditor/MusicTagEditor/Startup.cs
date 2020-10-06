using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MusicTagEditor.DataApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;

namespace MusicTagEditor
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllersWithViews(mvcOtions =>
            //{
            //    mvcOtions.EnableEndpointRouting = false;
            //});
            //services.AddMvc();
            services.AddSignalR();

            services.Configure<FormOptions>(
                options =>
                {
                    options.MultipartBodyLengthLimit = 80000000;
                    options.ValueLengthLimit = int.MaxValue;
                    options.MultipartHeadersLengthLimit = int.MaxValue;
                });

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SongsDbContext>(options => options.UseSqlServer(connection));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new PathString("/Auth/Login");
                });
            services.AddControllersWithViews();
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Menu}/{action=General}/{id?}");

            //    routes.MapRoute(
            //        name: "others",
            //        template: "{controller}/{action}/{id?}");
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Menu}/{action=General}/{id?}");
                endpoints.MapControllerRoute(
                    name: "others",
                    pattern: "{controller}/{action}/{id?}");
                endpoints.MapHub<SendTagHub>("/GetTag");
            });
        }
    }
}
