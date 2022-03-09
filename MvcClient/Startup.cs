using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcClient.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient
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
            services.AddControllersWithViews().AddJsonOptions(opt => {
                opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            services.AddSingleton<RequestInterceptor>();
            services.AddHttpContextAccessor(); // IHttpContextAccessor kardeþe eriþmek için

            // requestInterceptor request atarken araya giren bir kardeþ
            // accesstoken gönderebiliriz. bu sayede requeste her serferinde access token göndermeyi unutmayýz.

            services.AddHttpClient("api1", opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:5001");
                opt.DefaultRequestHeaders.Add("ContentType", "application/json");
                opt.DefaultRequestHeaders.Add("User-Agent", "MVCClient");

            }).AddHttpMessageHandler<RequestInterceptor>();


            services.AddAuthentication(opt  => {

                opt.DefaultAuthenticateScheme = "ExternalAuth";
                opt.DefaultSignInScheme = "ExternalAuth";


            }).AddCookie("ExternalAuth", opt =>
            {
                opt.Cookie.Name = "ExternalCookie";
                opt.LoginPath = "/Account/Login";
                opt.LogoutPath = "/Account/Logout";
                opt.AccessDeniedPath = "/Account/AccessDenied";

            });

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
