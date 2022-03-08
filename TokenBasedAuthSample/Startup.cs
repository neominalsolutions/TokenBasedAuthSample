using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TokenBasedAuthSample.identity;
using TokenBasedAuthSample.Services;

namespace TokenBasedAuthSample
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

            services.AddControllers();
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TokenBasedAuthSample", Version = "v1" });
            });

            //services.AddIdentity<MyUser, MyRole>();

            // login olurken kullanýlan þema 
            // Bir uygulamada farklý þekillerde birden fazla login yapýlabilmesi için farklý isimlerde þemaya ihtiyaç var.
            services.AddAuthentication().AddJwtBearer("myScheme",opt =>
            {

                opt.SaveToken = true;
               
                

               string issuer =  EncryptProvider.AESDecrypt(EncrptedHelper.Replace(Configuration["JWT:issuer"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);

                string audience = EncryptProvider.AESDecrypt(EncrptedHelper.Replace(Configuration["JWT:audience"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);


                string signingKey = EncryptProvider.AESDecrypt(EncrptedHelper.Replace(Configuration["JWT:signingKey"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);

                // access token control mekanizmasý

                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true, // issuer bilgisi yanlýþ gönderilierse accesstoken onaylanamaz
                    ValidateAudience = true, // audience yanlýþ gönderilirse accesstoken onaylanmaz
                    ValidateLifetime = true, // exipire olursa onaylanmaz
                    ValidateIssuerSigningKey = true, // singkey yanlýþ ise onaylanmaz
                    ValidIssuer = issuer, // issuer deðeri
                    ValidAudience = audience, // audince deðeri
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)) // private key
                };

            });


            services.AddAuthentication(o => {

                o.DefaultScheme = "google";
                o.DefaultSignInScheme = "External";
               
                // External dýþ bir kaynaða yönledirmek için kullanýlan scheme ismi External bu isim default bir isim bu sayede uygulama harici bir kaynaktan login olacaðýmýzý anlýyor.
                // Schema ismini deðiþtireebiliriz login olurkende bu ismi kullanmaya dikkat etmemiz lazým

            })
            .AddCookie("google")
            .AddCookie("External")
            .AddGoogle(googleOptions =>
            {
                googleOptions.SaveTokens = true;
                googleOptions.ClientId = "239647126082-jlaa7t5r9l5d0ba9ejr0inofv4bjq23q.apps.googleusercontent.com";
                googleOptions.ClientSecret = "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";
            });

            // farklý bir jwt þemasý da tanýmladýk.
            //services.AddAuthentication().AddJwtBearer("google",opt => { });

            services.AddSingleton<ITokenService, JwtTokenService>();


            // bu kontrol için kullanýcýnýn type email olan bir claiþm olmasý lazým
            // role dýþýndaki tüm claimler için policy tanýmlamasý yaparýz.
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("emailRequired", policy =>
                {
                    policy.RequireAuthenticatedUser().RequireClaim("emailaddress");
                });
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("AgeRequired", policy =>
                {
                    policy.RequireClaim("age");
                });
            });

            // value'su üzerinden claim kontolü
            // sadece bu email hesabýna sahip olanlar girebilir.
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("SpesificEmailAddress", policy =>
                {
                    policy.RequireClaim("emailaddress","mert@test.com","test@test.com","mert.alptekin@neominal.com");
                });
            });

            //services.AddTransient<SignInManager<MyUser>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TokenBasedAuthSample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
       name: "default",
       pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });
        }
    }
}
