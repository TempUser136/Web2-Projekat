using Common;
using Common.Models;
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

    }
}
