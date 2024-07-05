using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using API.Infrastructure;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Services.Runtime;

namespace UserService
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
                ServiceRuntime.RegisterServiceAsync("UserServiceType",
                    context => new UserService(context, serviceProvider)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(UserService).Name);

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
                  services.AddDbContext<FacultyDbContext>(options =>
                      options.UseOracle("User Id=sys;Password=123;Data Source=localhost:1521/xepdb1;DBA Privilege=SYSDBA;"));

                  // Register other services here
              });

    }
}
