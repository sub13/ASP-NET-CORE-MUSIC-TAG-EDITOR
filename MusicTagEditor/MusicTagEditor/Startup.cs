using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicTagEditor.Businees.Servicess;
using MusicTagEditor.Data.Models;
using MusicTagEditor.Mappings;

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

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<SongsDbContext>();

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options => //CookieAuthenticationOptions
            //    {
            //        options.LoginPath = new PathString("/Auth/Login");
            //    });

            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                opt =>
                {
                    opt.LoginPath = new PathString("/Auth/Login");
                });

            services.AddControllersWithViews();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new SongDataProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMusicFileService, MusicFileService>();

        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
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
