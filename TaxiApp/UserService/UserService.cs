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
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using XSystem.Security.Cryptography;

namespace UserService
{
    internal sealed class UserService : StatefulService, IStatefullInterface
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _secretKey = "ovde_ide_neka_tajna_nasa_koja_mora_biti_kompleksna";

        public UserService(StatefulServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _serviceProvider = serviceProvider;
            
        }
        public async Task<string> BlockDriver(string username)
            {
            ApproveDto banned = new ApproveDto()
            {
                Username = username
            };
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();
                dbContext.Banned.Add(banned);
                await dbContext.SaveChangesAsync();
                return "Korisnik uspesno banovan";
            }
        }
        public async Task<string> UnblockDriver(string username)
        {
            ApproveDto unbanned = new ApproveDto()
            {
                Username = username
            };
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();
                dbContext.Banned.Remove(unbanned);
                await dbContext.SaveChangesAsync();
                return "Korisnik uspesno un-banovan";
            }
        }
        public async Task<UserDto> UpdateUserAsync(UserForm formModel)
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
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(formModel.Password);

            var tmpSource = ASCIIEncoding.ASCII.GetBytes(formModel.Password);
            var tempHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            var user = new UserDto
            {
                Username = formModel.Username,
                Address = formModel.Address,
                Birthday = formModel.Birthday,
                LastName = formModel.LastName,
                Type = formModel.Type,
                Password = passwordHash,
                Email = formModel.Email,
                Name = formModel.Name,
                Image = imageBytes
            };
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();
                var user2 = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                user2.Address = user.Address;
                user2.Username = user.Username;
                user2.LastName = user.LastName;
                if(user.Birthday != null)
                user2.Birthday = user.Birthday;
                user2.Name = user.Name;
                if(user.Image != null)
                user2.Image = user.Image;
                user2.Email = user.Email;
                user2.Type = user.Type;
                user2.Password = passwordHash;
                await dbContext.SaveChangesAsync();
                return user2;
            }

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
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(formModel.Password);

            var tmpSource = ASCIIEncoding.ASCII.GetBytes(formModel.Password);
            var tempHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            var user = new UserDto
            {
                Username = formModel.Username,
                Address = formModel.Address,
                Birthday = formModel.Birthday,
                LastName = formModel.LastName,
                Type = formModel.Type,
                Password = passwordHash,
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

        public async Task<String> ApproveDriver(string username)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();

                UserStatusDto user = await dbContext.UserStatus.FirstOrDefaultAsync(u => u.Username == username);
                dbContext.UserStatus.Remove(user);
                await dbContext.SaveChangesAsync();

                UserDto userToMail = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                String mail = userToMail.Email;
                var emailService = new EmailService("smtp.gmail.com", 587, "mickomitr01@gmail.com", "ccio lmxp kyov mxzp");
                await emailService.SendEmailAsync("mickomitr01@gmail.com", "Account status", "Your account is approved");
                return "mail approved";

            }
        }
        public async Task<String> DeclineDriver(string username)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();

                UserStatusDto user = await dbContext.UserStatus.FirstOrDefaultAsync(u => u.Username == username);
                dbContext.UserStatus.Remove(user);
                await dbContext.SaveChangesAsync();

                UserDto userToDelete = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                dbContext.Users.Remove(userToDelete);
                await dbContext.SaveChangesAsync();
                return "Driver declined";
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

        public async Task<UserWithToken> LogUserAsync(LoginModel login)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
                try
                {


                    if (BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                    {

                        if (user == null)
                        {
                            return null; // Invalid username or password
                        }

                        // Generate JWT token if user is found and credentials match
                        List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                        // Add specific claims for roles if needed
                        if (user.Type == "Administrator")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "admin"));
                        }
                        else if (user.Type == "User")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "user"));
                        }
                        else if (user.Type == "Driver")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "driver"));
                        }

                        // Custom claims can be added here
                        // claims.Add(new Claim("CustomClaim", "CustomValue"));

                        // Signing credentials (secret key)
                        SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                        // Token options
                        var tokenOptions = new JwtSecurityToken(
                            issuer: "http://localhost:8613",
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(20), // Token expiration time
                            signingCredentials: signingCredentials
                        );
                        try
                        {
                            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                            UserWithToken uwt = new UserWithToken()
                            {
                                Username = user.Username,
                                Password = user.Password,
                                Type = user.Type,
                                Address = user.Address,
                                Birthday = user.Birthday,
                                Email = user.Email,
                                Image = user.Image,
                                LastName = user.LastName,
                                Name = user.Name,
                                Token = tokenString
                            };
                            return uwt; // Return the token as the login response
                        }
                        catch (Exception ex)
                        {
                            ServiceEventSource.Current.ServiceMessage(this.Context, "Error saving user to database: " + ex.Message);
                            throw;
                        }

                    }
                }
                catch(Exception ex)
                {
                    ServiceEventSource.Current.ServiceMessage(this.Context, "Error saving user to database: " + ex.Message);
                    throw;
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
        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}
