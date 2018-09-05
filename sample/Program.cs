using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NestWrapper.Builder;
using NestWrapper.Mock;

namespace sample
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddTransient<IBuilderElasticClient, MockBuilderElasticClient>();
                    services.AddSingleton<IHostedService, ElasticService>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConsole();
                });
            builder.RunConsoleAsync().GetAwaiter();
        }
    }
}
