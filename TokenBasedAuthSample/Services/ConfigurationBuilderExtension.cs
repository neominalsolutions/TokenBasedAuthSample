
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TokenBasedAuthSample
{
    // tipe yeni özellik kazandıracağız. Prototype ile bir dizinin farklı özellikler ile örnek yapmıştık. OrderBy extension yapmıştık.
    // this string keyword ile neyi extend edeceğimiz hangi tipi extend edeceğimizi söyleriz.
    // extension yazmak için static bir sınıfa ihtiyaç var.
    public static class StringExtension
    {
        public static string Upper(this string text)
        {
            return text.ToUpper();
        }
    }

    public static class ConfigurationBuilderExtension 
    {

        // IConfigurationRoot üzerinden konfigürasyon dosyasına erişiriz.
        // IConfigurationRoot

        public static void Decrypt(this IConfigurationRoot root, string secretKey, string vectorKey, string cipherPrefix)
        {

            // json dosyasını dizinden okur.
            var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
            var json = File.ReadAllText(appSettingsPath); // stringJson olarak okuduk

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ExpandoObjectConverter()); // object tanımlaması
            /*
             * 
             * "JWT": {
    "audience": "+rw1c6ZyhYNwtfrn7xHGlVW6wNdRGJBzqdEYi5anTXo=",
    "issuer": "+rw1c6ZyhYNwtfrn7xHGlSlyU7K4fL7JTKPpRvBwonw=",
    "signingKey": "9y2Wl7XRM4qIACTrJ356jTeew7Y3Nuht1UtWPEC3TLo="
  },
             */
            jsonSettings.Converters.Add(new StringEnumConverter()); // "AllowedHosts": "*"

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings); // config dosyasını JSON Object çevirir.

            // run-time da bir değişken tipi ile çalışma imakanı sunuyor. değişken tipi build-time da değil runtime da karar veriliyor. Ne gelirse tip ona göre işlem yapmamızı sağlıyor. 

            //var keyValuePairs =  config.ToList();

            //foreach (var item in keyValuePairs)
            //{
            //    object value =  item.Value;
            //}




            foreach (var child in root.GetChildren())
            {
                //Şifrelenmiş mi şifrelenmemiş mi bilemiyoruz bu sebeple şifrelenmiş halinin önüne bir ön takı koyalım.

                if (child.Key.Contains("JWT")) // keylerden JWT içeren ir şey varsa
                {
                    var audienceText = child.GetSection("audience").Value; // JWT içerisindeki audience section'a in onun value oku.

                    if (!audienceText.StartsWith(cipherPrefix))
                    {
                        
                        var encryptedAudience = EncryptProvider.AESEncrypt(audienceText, secretKey, vectorKey); // value şifrele
                        config.JWT.audience = $"{cipherPrefix}:{encryptedAudience}"; // json dosya yolundaki dizine şifrelenmiş olan değeri set et
                    }

                    var issuerText = child.GetSection("issuer").Value;

                    if(!issuerText.StartsWith(cipherPrefix))
                    {
                        var encryptedIssuer = EncryptProvider.AESEncrypt(issuerText, secretKey, vectorKey);

                        config.JWT.issuer = $"{cipherPrefix}:{encryptedIssuer}";
                    }

                    var signingKeyText = child.GetSection("signingKey").Value;

                    if (!signingKeyText.StartsWith(cipherPrefix))
                    {
                        var encryptedSigningKey = EncryptProvider.AESEncrypt(signingKeyText, secretKey, vectorKey);

                        config.JWT.signingKey = $"{cipherPrefix}:{encryptedSigningKey}";
                    }
                }

            }

            var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings); // dosyanın güncel halini ise json string yap.

            // appSettingsPath uzantılı dosyanın bulunduğu yere yeni json dosyasını yaz.
            File.WriteAllText(appSettingsPath, newJson);


        }

    }

}
