using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;

namespace Yahvol.Web
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var silo = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Build();

            await silo.StartAsync();

            var client = silo.Services.GetRequiredService<IClusterClient>();

            var webHostBuilder = CreateWebHostBuilder(args, client);
            var biltWebHost = webHostBuilder.Build();
            await biltWebHost.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, IClusterClient client) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                    services
                        .AddSingleton<IGrainFactory>(client)
                        .AddSingleton<IClusterClient>(client))
                .UseStartup<Startup>();


        //public static async Task Main(string[] args)
        //{
        //    var silo = new SiloHostBuilder()
        //        .UseLocalhostClustering()
        //        .Build();

        //    await silo.StartAsync();

        //    var client = silo.Services.GetRequiredService<IClusterClient>();

        //    var webHost = new WebHostBuilder()
        //        .ConfigureServices(services =>
        //            services
        //                .AddSingleton<IGrainFactory>(client)
        //                .AddSingleton<IClusterClient>(client))
        //        .UseStartup<Startup>()
        //        // Other ASP.NET configuration...
        //        .Build();
        //    webHost.Run();
        //}
    }
}
