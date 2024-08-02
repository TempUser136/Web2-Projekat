using Common.DTO;
using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IStatelessInterface:IService
    {
        Task<String> GetService();
        Task<PriceWaitResponse> Calculate();
        Task<bool> CreateRide(Ride ride);
        Task<List<RideDto>> GetUserRides(String username);

        Task<List<RideDto>> GetAllRides();
    }
}
