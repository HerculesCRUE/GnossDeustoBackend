using Microsoft.Extensions.Configuration;
using System.IO;

namespace ApiCargaWebInterface.Models.Services
{
    public class ConfigUrlCronService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Url { get; set; }
        public string GetUrl()
        {
            if (string.IsNullOrEmpty(Url))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                var connectionString = Configuration["ConfigUrlCron"];
                Url = connectionString;
            }
            return Url;
        }
    }
}
