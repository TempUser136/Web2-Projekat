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
    public interface IStatefullInterface: IService
    {
        Task<String> GetService();

        Task<UserDto> CreateUserAsync(UserForm uf);

        Task<UserDto> GetUserAsync(int id);

        Task<UserWithToken> LogUserAsync(LoginModel Login);

        Task<List<UserDto>> GetUserStatusAsync();

    }
}
