using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RACSMicroservice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Helpers
{
    public class PopulateDatabase
    {
        public static void PreparePopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<RACSContext>());
            }
        }

        public static void SeedData(RACSContext context)
        {
            System.Console.WriteLine("Appling Migrations To RACS database...");

            context.Database.Migrate();
        }
    }
}
