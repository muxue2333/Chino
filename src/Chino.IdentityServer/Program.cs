using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino.IdentityServer.SeedData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Chino.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var seed = args.Contains("/seed");
                if(seed)
                    args = args.Except(new[] { "/seed" }).ToArray();

                var host = CreateHostBuilder(args).Build();

                if (seed)
                {
                    Console.WriteLine("Seeding database ...");
                    var config = host.Services.GetRequiredService<IConfiguration>();
                    SeedData.SeedData.EnsureSeedData(config);
                    return;
                }

                Console.WriteLine("Starting host...");
                host.Run();
                return;
            }
            catch(Exception e)
            {
                Console.WriteLine("Host terminated unexpectedly.");
                Console.WriteLine(e.Message);
            }
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
