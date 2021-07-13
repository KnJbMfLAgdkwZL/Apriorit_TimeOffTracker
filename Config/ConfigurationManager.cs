using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace TimeOffTracker.Config
{
    static class ConfigManager
    {
        public static IConfiguration AppSetting { get; set; }

        static ConfigManager()
        {
            AppSetting = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static IServiceCollection ConfigureOptions<TOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string key = null)
            where TOptions : class
        {
            var configurationSection = configuration.GetSection(key ?? typeof(TOptions).Name);

            return services.Configure<TOptions>(configurationSection);
        }
    }
}