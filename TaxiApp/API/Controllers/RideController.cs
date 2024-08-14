using Common;
using Common.DTO;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RideController : ControllerBase
    {
        [HttpGet]
        [Route("statefull")]
        public async Task<string> GetService()
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/TaxiApp/RideService")
                );
            var serviceName = await statelessProxy.GetService();
            return serviceName;
        }

        [HttpGet]
        [Route("Calculate")]
        public async Task<PriceWaitResponse> Calculate()
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/TaxiApp/RideService")
                );
            var TimeAndPrice = await statelessProxy.Calculate();
            return TimeAndPrice;
        }
        [Authorize]
        [HttpGet]
        [Route("GetUserRides")]
        public async Task<List<RideDto>> GetUserRides([FromQuery] String username)
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/TaxiApp/RideService")
                );
            List<RideDto> rides = await statelessProxy.GetUserRides(username);
            return rides;
        }

        [HttpGet]
        [Route("GetAllRides")]
        public async Task<List<RideDto>> GetAllRides()
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/TaxiApp/RideService")
                );
            List<RideDto> rides = await statelessProxy.GetAllRides();
            return rides;
        }

        [HttpPost]
        [Route("AddRide")]
        public async Task<String> AddRide(Ride ride)
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/TaxiApp/RideService")
                );
            var TimeAndPrice = await statelessProxy.CreateRide(ride);
            return "TimeAndPrice";
        }

        [HttpPost]
        [Route("UpdateRideStatus")]
        public async Task<String> UpdateRideStatus([FromBody] RideUpdateDto update)
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/TaxiApp/RideService")
                );
            var TimeAndPrice = await statelessProxy.UpdateRideStatus(update);
            return "Updated!";
        }

    }
}
