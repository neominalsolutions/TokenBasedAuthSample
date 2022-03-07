using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenBasedAuthSample.Services;

namespace TokenBasedAuthSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

          //  var config = new ConfigurationBuilder()
          //.AddJsonFile("appsettings.json")
          //.AddEnvironmentVariables()
          //.Build()
          //.Decrypt(cipherSecretKey: "CipherKey", cipherPrefix: "CipherText:");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

        Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(opt =>
                {
                    // AppSettingsdosyasýnda extra araya girip bir iþlem yap.

                    opt.AddJsonFile("appsettings.json").Build().Decrypt(secretKey: ConfigurationEncryptionTypes.SecretKey, vectorKey: ConfigurationEncryptionTypes.VectorKey, cipherPrefix: ConfigurationEncryptionTypes.ChiperPrefix);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
