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

                        //Baza
                        builder.Services.AddDbContext<FacultyDbContext>(options =>
                            options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

                        //Mapping
                        var mapperConfig = new MapperConfiguration(mc =>
                        {
                            mc.AddProfile(new MappingProfile());
                        });

                        IMapper mapper = mapperConfig.CreateMapper();
                        builder.Services.AddSingleton(mapper);
                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);

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

                        app.UseAuthorization();
                        app.MapControllers();

                        return app;
                    }))
            };
        }
    }
}
