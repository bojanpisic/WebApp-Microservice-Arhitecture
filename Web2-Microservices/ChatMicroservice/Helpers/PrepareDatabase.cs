using ChatMicroservice.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.Helpers
{
    public class PrepareDatabase
    {
        public static void PreparePopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<ChatContext>());
            }
        }

        public static void SeedData(ChatContext context)
        {
            System.Console.WriteLine("Appling Migrations To Chat database...");

            context.Database.Migrate();

        }
    }
}
