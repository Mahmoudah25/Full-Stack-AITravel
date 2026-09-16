
using AITravelB.API.Hubs;
using AITravelB.API.Services;
using AITravelB.Application.Common.Interfaces;
using AITravelB.Application.Common.setting;
using AITravelB.Application.Trips.Commands;
using AITravelB.Infrastructure.ExteranlService.Groq;
using AITravelB.Infrastructure.ExteranlService.Paymob;
using AITravelB.Infrastructure.ExteranlService.Weather;
using AITravelB.Infrastructure.Service;
using AITravelB.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace AITravelB.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

           
            builder.Services.AddControllers();

            ///////////////////////
            // DB Context
            ///////////////////////
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            //////////
            // Service
            /////////
            builder.Services.AddHttpClient<IPlacesService, OsmPlacesService>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "AITravelB/1.0 (contact: your-email@example.com)");
                client.Timeout = TimeSpan.FromSeconds(5);
            });
            builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            //builder.Services.AddScoped<IItineraryAiService, GeminiItineraryService>();
            builder.Services.AddHttpClient<IItineraryAiService, GroqItineraryService>();
            builder.Services.AddHttpClient<IWeatherService, OpenWeatherService>();
            builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
            builder.Services.AddHttpClient<IPaymentGateway, PayMobService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ExternalServices:Paymob:BaseUrl"]
                    ?? throw new InvalidOperationException("PayMob BaseUrl is not configured."));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            builder.Services.Configure<PayMobSetting>(builder.Configuration.GetSection("ExternalServices:Paymob"));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTripCommand).Assembly));
            builder.Services.AddSignalR();
            //builder.Services.AddOpenApi();

            // =======================
            // Swagger
            // =======================
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AITripPlanner",
                    Description = "ASP.NET Core Web API"
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebClient", policy =>
                {
                    policy.WithOrigins(
                            "http://127.0.0.1:5500", "http://localhost:5500",
                            "http://127.0.0.1:5501", "http://localhost:5501",
                            "https://ai-travelf.vercel.app",
                             "https://ai-travel-front.vercel.app")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();
            // Configure the HTTP request pipeline.


            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseCors("AllowWebClient");  



            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.Run();
        }
    }
}
