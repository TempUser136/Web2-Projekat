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
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace UserService
{
    internal sealed class UserService : StatefulService, IStatefullInterface
    {
        private readonly IServiceProvider _serviceProvider;


        public UserService(StatefulServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task<string> GetService()
        {
            var serviceName = this.Context.ServiceName.ToString();
            return serviceName;
        }

        public async Task<UserDto> CreateUserAsync(UserForm formModel)
        {
            var imagePath = formModel.ImagePath;
            byte[] imageBytes;

            // Load the image as byte array
            try
            {
                imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Error reading image file: " + ex.Message);
                throw new Exception("Error reading image file.", ex);
            }

            var user = new UserDto
            {
                Username = formModel.Username,
                Address = formModel.Address,
                Birthday = formModel.Birthday,
                LastName = formModel.LastName,
                Type = formModel.Type,
                Password = formModel.Password,
                Email = formModel.Email,
                Name = formModel.Name,
                Image = imageBytes
            };
            if (user.Type == "Driver")
            {
                var status = new UserStatusDto
                {
                    Username = user.Username,
                    Status = "Waiting"
                };
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();
                        dbContext.UserStatus.Add(status);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.ServiceMessage(this.Context, "Error saving user to database: " + ex.Message);
                    throw;
                }
            }
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();

                        dbContext.Users.Add(user);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.ServiceMessage(this.Context, "Error saving user to database: " + ex.Message);
                    throw;
                }

            return user;
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();

                return await dbContext.Users.FindAsync(id);
            }
        }
        public async Task<List<UserDto>> GetUserStatusAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();

                List<UserStatusDto> users = new List<UserStatusDto>();
                users = await dbContext.UserStatus.ToListAsync();

                List<UserDto> AllUsers = new List<UserDto>();
                AllUsers = await dbContext.Users.ToListAsync();

                List<UserDto> usersToReturn = new List<UserDto>();
                foreach (var user in users) {
                    foreach(var u in AllUsers)
                    {
                        if(user.Username == u.Username)
                        {
                            usersToReturn.Add(u);
                        }
                    }
                }

                return usersToReturn;
            }
        }

        public async Task<UserDto> LogUserAsync(LoginModel Login)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();
                List<UserDto> users = new List<UserDto>();
                users = await dbContext.Users.ToListAsync();
                foreach (var item in users)
                {
                    if(item.Username == Login.Username && item.Password== Login.Password)
                    {
                        return item;
                    }
                }
                return null;
            }
        }
        public async Task<UserDto> Calculate(LoginModel Login)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();
                List<UserDto> users = new List<UserDto>();
                users = await dbContext.Users.ToListAsync();
                foreach (var item in users)
                {
                    if (item.Username == Login.Username && item.Password == Login.Password)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
