using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ReservationsMicroservice.Data;
using ReservationsMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ReservationsContext>(
               options =>
               {
                               //options.UseMySql(Configuration.GetConnectionString(Environment.GetEnvironmentVariable("CONTAINER") == "true" ? 
                               //    "ContainerConnection" :
                               //    "MySqlConnection"));

                               options.UseMySql(Configuration.GetConnectionString("ContainerConnection"));
               });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddControllers().AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      //builder.WithOrigins("http://localhost:4200");
                                      builder.AllowAnyMethod();
                                      builder.AllowAnyOrigin();
                                      builder.AllowAnyHeader();
                                  });
            });



            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration.GetSection("RabbitMQ:EventBusConnection").Value,
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(Configuration.GetSection("RabbitMQ:EventBusUserName").Value))
                {
                    factory.UserName = Configuration.GetSection("RabbitMQ:EventBusUserName").Value;
                }

                if (!string.IsNullOrEmpty(Configuration.GetSection("RabbitMQ:EventBusPassword").Value))
                {
                    factory.Password = Configuration.GetSection("RabbitMQ:EventBusPassword").Value;
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration.GetSection("RabbitMQ:EventBusRetryCount").Value))
                {
                    retryCount = int.Parse(Configuration.GetSection("RabbitMQ:EventBusRetryCount").Value);
                }


                return new DefaultRabbitMQPersistentConnection(factory, retryCount);
            });


            RegisterEventBus(services);

            services.AddOptions();

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            Helpers.PrepareDatabase.PreparePopulation(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureEventBus(app);

        }

        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration.GetSection("RabbitMQ:SubscriptionClientName").Value;

            services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration.GetSection("RabbitMQ:EventBusRetryCount").Value))
                {
                    retryCount = int.Parse(Configuration.GetSection("RabbitMQ:EventBusRetryCount").Value);
                }

                return new EventBusRabbitMq(rabbitMQPersistentConnection, eventBusSubcriptionsManager, iLifetimeScope, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();


        }
    }
}
