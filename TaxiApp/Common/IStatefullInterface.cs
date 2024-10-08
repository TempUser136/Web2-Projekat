﻿using Common.DTO;
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

        Task<List<UserDto>> GetUnapprovedDrivers();

        Task<Status> GetDriverStatus(string username);


        Task<String> ApproveDriver(string username);
        Task<String> DeclineDriver(string username);

        Task<UserDto> UpdateUserAsync(UserForm uf);

        Task<String> BlockDriver(string username);
        Task<String> UnblockDriver(string username);

    }
}
