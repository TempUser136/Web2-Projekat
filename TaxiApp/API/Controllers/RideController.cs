﻿using Common;
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

    }
}
