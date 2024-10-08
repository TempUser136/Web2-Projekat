using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AutoMapper;
using API.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using API.Infrastructure;
using Common.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    internal sealed class API : StatelessService
    {
        public API(StatelessServiceContext context)
            : base(context)
        { }
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
        new ServiceInstanceListener(serviceContext =>
            new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
            {
                ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                var builder = WebApplication.CreateBuilder();
                var configuration = builder.Configuration;

                // Add services to the container
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAllOrigin",
                        builder => builder
                            .AllowAnyOrigin() // Allow any origin for testing, replace with specific origin in production
                            .AllowAnyHeader()
                            .AllowAnyMethod());
                });

                // Database Contexts
                builder.Services.AddDbContext<FacultyDbContext>(options =>
                    options.UseOracle(configuration.GetConnectionString("DefaultConnection")));
                builder.Services.AddDbContext<RideDbContext>(options =>
                    options.UseOracle(configuration.GetConnectionString("RideConnection")));

                // AutoMapper
                var mapperConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                });

                IMapper mapper = mapperConfig.CreateMapper();
                builder.Services.AddSingleton(mapper);
                builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);

                // JWT Authentication
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "http://localhost:8613", // Your issuer
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])) // Your secret key
                    };
                });

                builder.WebHost
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                    .UseUrls(url);

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Use the CORS policy
                app.UseCors("AllowAllOrigin");

                app.UseAuthentication(); 
                app.UseAuthorization();

                app.MapControllers();

                return app;
            }))
            };
        }

    }
}
