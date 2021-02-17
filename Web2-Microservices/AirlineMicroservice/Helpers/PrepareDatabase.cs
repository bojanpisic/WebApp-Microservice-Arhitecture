using AirlineMicroservice.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Helpers
{
    public class PrepareDatabase
    {
        public static void PreparePopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AirlineContext>());
            }
        }

        public static void SeedData(AirlineContext context)
        {
            System.Console.WriteLine("Appling Migrations To Airline database...");

            context.Database.Migrate();

        }
    }
}
