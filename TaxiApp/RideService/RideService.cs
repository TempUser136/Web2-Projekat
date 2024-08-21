using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Infrastructure;
using Common;
using Common.DTO;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace RideService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class RideService : StatelessService, IStatelessInterface
    {
        private readonly IServiceProvider _serviceProvider;
        static int identificator = 1;
        public RideService(StatelessServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetService()
        {
            return this.Context.ServiceName.ToString();
        }

        public async Task<PriceWaitResponse> Calculate()
        {
            Random rnd = new Random();
            int time= rnd.Next(1, 45);
            int price = rnd.Next(1, 1300);
            PriceWaitResponse response = new PriceWaitResponse();
            response.Price = price; 
            response.WaitTime= time;
            return response;
        }
        public async Task<List<RideDto>> GetUserRides(string username)
        {
            List<RideDto> userRides = new List<RideDto>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RideDbContext>();
                List<RideDto> rides = new List<RideDto>();
                
                rides = await dbContext.Rides.ToListAsync();
                foreach (var ride in rides)
                {
                    if (ride.Username == username)
                    {
                        userRides.Add(ride);
                    }
                }

            }
                return userRides;
        }
        public async Task<List<RideDto>> GetAllRides()
        {
            List<RideDto> rides = new List<RideDto>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RideDbContext>();
                

                rides = await dbContext.Rides.ToListAsync();

            }
            return rides;
        }
        public async Task<String> UpdateRideStatus(RideUpdateDto update)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RideDbContext>();

                // Find the ride by ID directly
                var rideToUpdate = await dbContext.Rides.FindAsync(update.Id);
                if (rideToUpdate == null)
                {
                    return "Ride not found";
                }

                // Update the status
                rideToUpdate.Status = update.Status;

                dbContext.Rides.Update(rideToUpdate);
                await dbContext.SaveChangesAsync();

                // Start the background task to update status to 'done' after wait time
                if(update.Status!= "Done")
                    _ = Task.Run(() => AcceptRide(rideToUpdate.Id, rideToUpdate.WaitTime));
            }

            return "Updated";
        }

        public async Task AcceptRide(int id, int waitTime)
        {
            await Task.Delay(TimeSpan.FromMinutes(waitTime));

            RideUpdateDto rideUpdate = new RideUpdateDto()
            {
                Id = id,
                Status = "Done"
            };

            // Update ride status to 'done'
            await UpdateRideStatus(rideUpdate);
        }
        public async Task<bool> CreateRide(Ride ride)
        {
            var rideSave = new RideDto
            {
                Id = identificator,
                StartAddress = ride.StartAddress,
                Destination = ride.Destination,
                Price = ride.Price,
                WaitTime = ride.WaitTime,
                Status = ride.Status,
                Username = ride.Username
            };
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<RideDbContext>();
                    List<RideDto> rides = new List<RideDto>();
                    rides= await dbContext.Rides.ToListAsync();
                    int tempId = 0;
                    

                    if (rides.Count > 0)
                    {
                        foreach (RideDto r in rides)
                        {
                            if (r.Id > tempId)
                            {
                                tempId = r.Id;
                            }
                        }
                        identificator = tempId+1;
                    }
                    rideSave.Id = identificator;
                    dbContext.Rides.Add(rideSave);
                    await dbContext.SaveChangesAsync();
                    identificator++;
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Error saving user to database: " + ex.Message);
                throw;
            }
            return true;
        }
        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        
    }
}
