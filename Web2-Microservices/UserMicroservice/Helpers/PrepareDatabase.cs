using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Data;

namespace UserMicroservice.Helpers
{
    public class PrepareDatabase
    {
        public static void PreparePopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<UserContext>());
            }
        }

        public static void SeedData(UserContext context)
        {
            System.Console.WriteLine("Appling Migrations To User database...");

            context.Database.Migrate();

        }
    }
}
