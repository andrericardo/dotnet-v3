using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotnet_v3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envs = System.Environment.GetEnvironmentVariables();

            var webhostBuilder = CreateHostBuilder(args);

            SetupAppRunnerEnvironment(webhostBuilder, envs);

            webhostBuilder.Build().Run();
        }

        public static void SetupAppRunnerEnvironment(IHostBuilder hostBuilder, IDictionary envs)
        {
            if (envs.Contains("APP_PORT"))
            {
                var port = int.Parse(envs["APP_PORT"].ToString());

                hostBuilder.ConfigureWebHost(webhostBuilder =>
                    webhostBuilder.UseKestrel(options =>
                    {
                        options.ListenAnyIP(port);
                    })
                );
            }

            if (envs.Contains("APP_NAME"))
            {
                //app runner expect the service to be host at http(s)://host:port/APP_NAME
                hostBuilder.ConfigureHostConfiguration(config =>
                    {
                        config.AddInMemoryCollection(new List<KeyValuePair<string, string>>() {
                        new KeyValuePair<string, string>("PathBase", envs["APP_NAME"].ToString())
                        });
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                       webBuilder.UseStartup<Startup>();
                    });
//                var configration = builder.Build();
                //webhostBuilder.UseConfiguration(configration);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
