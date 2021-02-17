using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OcelotApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                //.ConfigureAppConfiguration(cb =>
                //{
                    
                    //var sources = cb.Sources;
                    //sources.Insert(3, new Microsoft.Extensions.Configuration.Json.JsonConfigurationSource()
                    //{
                    //    Optional = true,
                    //    Path = "configuration.json",
                    //    ReloadOnChange = false
                    //});
                //})
                .ConfigureAppConfiguration((host, config) => 
                {
                    config.AddJsonFile("ocelotRoutes.json",optional: false, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .Build();

    }
}
