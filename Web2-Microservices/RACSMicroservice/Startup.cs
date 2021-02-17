using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using RACSMicroservice.Data;
using RACSMicroservice.IntegrationEvents.EventHandling;
using RACSMicroservice.IntegrationEvents.Events;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RACSMicroservice
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
            //services.AddControllers();

            services.AddDbContext<RACSContext>(
                         options =>
                         {
                             //options.UseMySql(Configuration.GetConnectionString(Environment.GetEnvironmentVariable("CONTAINER") == "true" ? 
                             //    "ContainerConnection" :
                             //    "MySqlConnection"));

                             options.UseMySql(Configuration.GetConnectionString("ContainerConnection"));
                         });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<HttpClient>(new HttpClient());

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

            services.AddSingleton<HttpClient>(new HttpClient());


            //Jwt Authentication

            var key = Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value);

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            //swagger
            services.AddSwaggerGen(conf =>
            {
                conf.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Airline service",
                    Version = "v1"
                });
            });
            //plain.rabbitmq
            //services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
            //services.AddScoped<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
            //    "report_exchange",
            //    ExchangeType.Topic));



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
            Helpers.PopulateDatabase.PreparePopulation(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);


            app.UseSwagger();

            app.UseSwaggerUI(conf =>
            {
                conf.SwaggerEndpoint("/swagger/v1/swagger.json", "RACS API");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureEventBus(app); //subscribe microservice 
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<CreateRACSIntegrationEvent, CreateRACSIntegrationEventHandler>();
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
            services.AddTransient<CreateRACSIntegrationEventHandler>();
        }
    }
}
