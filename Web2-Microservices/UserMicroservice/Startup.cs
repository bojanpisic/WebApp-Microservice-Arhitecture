using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserMicroservice.Data;
using UserMicroservice.Models;
using UserMicroservice.Repository;

namespace UserMicroservice
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
            //var server = Configuration["DBServer"] ?? "mysql-server";
            //var port = Configuration["DBPort"] ?? "3306";
            //var user = Configuration["DBUser"] ?? "user";
            //var password = Configuration["DBPassword"] ?? "pass";
            //var database = Configuration["Database"] ?? "usermsDb";

            //string connectionString;
            //if (Environment.GetEnvironmentVariable("CONTAINER") == "true")
            //    connectionString = "server=mysql;port=3306;database=CoreAppDB;user=root;password=fooCoreApp";
            //else
            //    connectionString = "server=localhost;port=3306;database=CoreAppDB;user=root;password=fooCoreApp";

            services.AddDbContext<UserContext>(
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

            services.AddDefaultIdentity<Person>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<UserContext>();


            services.AddControllers().AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
            }
);

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
                    Title = "User service",
                    Version = "v1"
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
            //prepare database -> migrate db shema to docker container db
            Helpers.PrepareDatabase.PreparePopulation(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(conf =>
            {
                conf.SwaggerEndpoint("/swagger/v1/swagger.json", "Airline API");
            });


            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

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
            // services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>();
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //
        }
    }
}
