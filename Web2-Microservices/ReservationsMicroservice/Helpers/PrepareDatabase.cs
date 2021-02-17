using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationsMicroservice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Helpers
{
    public class PrepareDatabase
    {
        public static void PreparePopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<ReservationsContext>());
            }
        }

        public static void SeedData(ReservationsContext context)
        {
            System.Console.WriteLine("Appling Migrations To Reservations database...");

            context.Database.Migrate();

        }
    }
}
