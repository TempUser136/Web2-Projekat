﻿using Common;
using Common.DTO;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Net.Http;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("steteless")]
        public async Task<string> GetService()
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/TaxiApp/UserService")
                );
            var serviceName = await statelessProxy.GetService();
            return serviceName;
        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserFormModel formModel, [FromQuery] int id)
        {
            try
            {
                // Define the path where the image will be saved
                var imagePath = Path.Combine("C:\\Projekat\\Projekat\\TaxiApp\\Common\\uploads\\", formModel.Image.FileName);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                // Save the image to the specified path
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await formModel.Image.CopyToAsync(stream);
                }

                // Create a UserForm object with the path to the saved image
                UserForm uf = new UserForm(formModel)
                {
                    ImagePath = imagePath
                };

                // Create a Service Fabric proxy to call the stateful service
                var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                    new Uri("fabric:/TaxiApp/UserService"),
                    new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
                );

                // Call the CreateUserAsync method on the stateful service
                var newUser = await statefullProxy.UpdateUserAsync(uf);

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                // Log the error (consider using a logging framework like Serilog or NLog)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost]
        [Route("creteUser")]
        public async Task<IActionResult> CreateUser(UserFormModel formModel, [FromQuery] int id)
        {
            try
            {
                // Define the path where the image will be saved
                var imagePath = Path.Combine("C:\\Projekat\\Projekat\\TaxiApp\\Common\\uploads\\", formModel.Image.FileName);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                // Save the image to the specified path
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await formModel.Image.CopyToAsync(stream);
                }

                // Create a UserForm object with the path to the saved image
                UserForm uf = new UserForm(formModel)
                {
                    ImagePath = imagePath
                };

                // Create a Service Fabric proxy to call the stateful service
                var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                    new Uri("fabric:/TaxiApp/UserService"),
                    new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
                );

                // Call the CreateUserAsync method on the stateful service
                var newUser = await statefullProxy.CreateUserAsync(uf);

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                // Log the error (consider using a logging framework like Serilog or NLog)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        [HttpPost]
        [Route("Login/{id}")]
        public async Task<ActionResult<User>> Login(int id, [FromBody] LoginModel Login)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
            );
            var user = await statefullProxy.LogUserAsync(Login);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }


        [HttpPost]
        [Route("ApproveDriver")]
        public async Task<String> ApproveDriver([FromQuery] int id,[FromBody] ApproveDto dto)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
            );
            string nesto = await statefullProxy.ApproveDriver(dto.Username);

            return "Nije uspesno menjanje";
        }

        [HttpPost]
        [Route("BlockDriver")]
        public async Task<String> BlockDriver([FromQuery] int id, [FromBody] ApproveDto dto)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
            );
            string nesto = await statefullProxy.BlockDriver(dto.Username);

            return "Nije uspesno blokiranje";
        }
        [HttpPost]
        [Route("UnblockDriver")]
        public async Task<String> UnblockDriver([FromQuery] int id, [FromBody] ApproveDto dto)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
            );
            string nesto = await statefullProxy.UnblockDriver(dto.Username);

            return "Nije uspesno blokiranje";
        }

        [HttpPost]
        [Route("DeclineDriver")]
        public async Task<String> DeclineDriver([FromQuery] int id, [FromBody] ApproveDto dto)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
            );
            string nesto = await statefullProxy.DeclineDriver(dto.Username);

            return "Nije uspesno menjanje";
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
                );
            var user = await statefullProxy.GetUserAsync(id);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpGet("GetDrivers")]
        public async Task<ActionResult<User>> GetDrivers([FromQuery] int id)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
                );
            List<UserDto> user = new List<UserDto>();
            try
            {
                user = await statefullProxy.GetUserStatusAsync();
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error calling GetUserStatusAsync: {ex.Message}");
                // Additional logging if needed
                Console.WriteLine(ex.StackTrace);
            }
            if (user.Count >0)
            {
                return Ok(user);
            }
            return Ok(user);
        }

        [HttpGet("GetDriverStatus")]
        public async Task<ActionResult<Status>> GetDriverStatus([FromQuery] int id, [FromQuery] string username)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
                );
            Status status = new Status();
            status = await statefullProxy.GetDriverStatus(username);

            return Ok(status);
        }
        [HttpGet("GetUnapproved")]
        public async Task<ActionResult<User>> GetUnapproved([FromQuery] int id)
        {
            var statefullProxy = ServiceProxy.Create<IStatefullInterface>(
                new Uri("fabric:/TaxiApp/UserService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(id)
                );
            List<UserDto> user = new List<UserDto>();
            try
            {
                user = await statefullProxy.GetUnapprovedDrivers();
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error calling GetUserStatusAsync: {ex.Message}");
                // Additional logging if needed
                Console.WriteLine(ex.StackTrace);
            }
            if (user.Count > 0)
            {
                return Ok(user);
            }
            return NotFound();
        }

    }
}
