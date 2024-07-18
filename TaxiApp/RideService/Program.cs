using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Runtime;

namespace RideService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var host = CreateHostBuilder().Build();
                var serviceProvider = host.Services;
                // Register the service with the Service Fabric runtime, passing the IServiceProvider
                ServiceRuntime.RegisterServiceAsync("RideServiceType",
                    context => new RideService(context, serviceProvider)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(RideService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
        private static IHostBuilder CreateHostBuilder() =>
          Host.CreateDefaultBuilder()
              .ConfigureServices((context, services) =>
              {
                  // Register DbContext and other services
                  services.AddDbContext<RideDbContext>(options =>
                      options.UseOracle("User Id=sys;Password=123;Data Source=localhost:1521/xe;DBA Privilege=SYSDBA;"));

                  // Register other services here
              });

    }
}
