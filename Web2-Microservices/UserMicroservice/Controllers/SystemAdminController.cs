using EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UserMicroservice.DTOs;
using UserMicroservice.IntegrationEvents.Events;
using UserMicroservice.Models;
using UserMicroservice.Repository;

namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAdminController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IEventBus eventBus;

        public SystemAdminController(IUnitOfWork _unitOfWork, IEventBus eventBus)
        {
            unitOfWork = _unitOfWork;
            this.eventBus = eventBus;
        }

        [HttpGet]
        [Route("123")]
        public async Task<IActionResult> s() 
        {
            Console.WriteLine("DASSADSA");
            return Ok("SSSSSSSSSSSSSSSSS");
        }

        [HttpPost("register-systemadmin")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> RegisterSystemAdmin([FromBody] RegisterAdminDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Person createUser;

            try
            {
                //string userId = User.Claims.First(c => c.Type == "UserID").Value;
                //string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                //if (!userRole.Equals("Admin"))
                //{
                //    return Unauthorized();
                //}






                //if ((await unitOfWork.AuthenticationRepository.GetPersonByEmail(userDto.Email)) != null)
                //{
                //    return BadRequest("User with that email already exists!");
                //}
                if ((await unitOfWork.AuthenticationRepository.GetPersonByUserName(userDto.UserName)) != null)
                {
                    return BadRequest("User with that usermane already exists!");
                }

                if (userDto.Password.Length > 20 || userDto.Password.Length < 8)
                {
                    return BadRequest("Password length has to be between 8-20");
                }

                if (!unitOfWork.AuthenticationRepository.CheckPasswordMatch(userDto.Password, userDto.ConfirmPassword))
                {
                    return BadRequest("Passwords dont match");
                }
                
                createUser = new Person()
                {
                    Email = userDto.Email,
                    UserName = userDto.UserName,
                };
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to register system admin");
            }

            try
            {
                var result = await this.unitOfWork.AuthenticationRepository.RegisterSystemAdmin(createUser, userDto.Password);
                var addToRoleResult = await unitOfWork.AuthenticationRepository.AddToRole(createUser, "Admin");
                await unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal server error. Registration failed.");
            }

            try
            {
                var emailSent = await unitOfWork.AuthenticationRepository.SendConfirmationMail(createUser, "admin", userDto.Password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return StatusCode(500, "Internal server error. Sending email failed.");
            }

            return Ok();
        }
        [HttpPost]
        [Route("register-airline")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> RegisterAirlineAdmin([FromBody] RegisterAirlineAdminDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid.");
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);

                if (!userRole.Equals("Admin"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                //if ((await unitOfWork.AuthenticationRepository.GetPersonByEmail(registerDto.Email)) != null)
                //{
                //    return BadRequest("User with that email already exists!");
                //}
                if ((await unitOfWork.AuthenticationRepository.GetPersonByUserName(registerDto.UserName)) != null)
                {
                    return BadRequest("User with that usermane already exists!");
                }
                if (registerDto.Password.Length > 20 || registerDto.Password.Length < 8)
                {
                    return BadRequest("Password length has to be between 8-20");
                }
                if (!unitOfWork.AuthenticationRepository.CheckPasswordMatch(registerDto.Password, registerDto.ConfirmPassword))
                {
                    return BadRequest("Passwords dont match");
                }

                var admin = new AirlineAdmin()
                {
                    Email = registerDto.Email,
                    UserName = registerDto.UserName,
                };


                try
                {
                    await this.unitOfWork.AuthenticationRepository.RegisterAirlineAdmin(admin, registerDto.Password);
                    await unitOfWork.AuthenticationRepository.AddToRole(admin, "AirlineAdmin");
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to register airline admin. One of transactions failed");
                }

                Console.WriteLine("ADMIN ID  {0}", admin.Id);

                var @event = new CreateAirlineIntegrationEvent(registerDto.Name, registerDto.City, registerDto.State, 
                                                                    registerDto.Lat, registerDto.Lon, admin.Id);

                try
                {
                    eventBus.Publish(@event);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to publish createAirline event");
                    return StatusCode(500, "Failed to register airline admin.");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to register airline admin");
            }
        }

        [HttpPost]
        [Route("register-racservice")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> RegisterRACSAdmin([FromBody] RegisterRACSAdminDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid.");
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);

                if (!userRole.Equals("Admin"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                //if ((await unitOfWork.AuthenticationRepository.GetPersonByEmail(registerDto.Email)) != null)
                //{
                //    return BadRequest("User with that email already exists!");
                //}
                if ((await unitOfWork.AuthenticationRepository.GetPersonByUserName(registerDto.UserName)) != null)
                {
                    return BadRequest("User with that usermane already exists!");
                }
                if (registerDto.Password.Length > 20 || registerDto.Password.Length < 8)
                {
                    return BadRequest("Password length has to be between 8-20");
                }
                if (!unitOfWork.AuthenticationRepository.CheckPasswordMatch(registerDto.Password, registerDto.ConfirmPassword))
                {
                    return BadRequest("Passwords dont match");
                }
                var admin = new RACSAdmin()
                {
                    Email = registerDto.Email,
                    UserName = registerDto.UserName,
                };

                try
                {
                    await this.unitOfWork.AuthenticationRepository.RegisterRACSAdmin(admin, registerDto.Password);
                    await unitOfWork.AuthenticationRepository.AddToRole(admin, "RentACarServiceAdmin");
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to register racs admin. One of transactions failed");
                }

                var @event = new CreateRACSIntegrationEvent(registerDto.Name, registerDto.City, registerDto.State, registerDto.Lon, registerDto.Lat, admin.Id);

                try
                {
                    eventBus.Publish(@event);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to publish createRacs event");
                    return StatusCode(500, "Failed to register racs admin.");
                }


                try
                {
                    var emailSent = await unitOfWork.AuthenticationRepository.SendConfirmationMail(admin, "admin", registerDto.Password);
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to send registration email");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to register racs admin");
            }
        }

        [HttpPost]
        [Route("set-bonus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> SetBonus([FromBody] BonusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);

                if (!userRole.Equals("Admin"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (dto.Bonus < 0 || dto.Discount < 0 || dto.Discount > 100)
                {
                    return BadRequest("Inputs are not valid");
                }

                var bonus = await unitOfWork.BonusRepository.GetByID(1);

                if (bonus == null)
                {
                    bonus = new Bonus()
                    {
                        BonusPerKilometer = dto.Bonus,
                        DiscountPerReservation = dto.Discount
                    };

                    try
                    {
                        await unitOfWork.BonusRepository.Insert(bonus);
                        await unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Transaction failed");
                    }
                    return Ok();
                }

                bonus.BonusPerKilometer = dto.Bonus;
                bonus.DiscountPerReservation = dto.Discount;

                try
                {
                    unitOfWork.BonusRepository.Update(bonus);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Transaction failed");
                }

                return Ok();

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to set bonus");
            }
        }


        [HttpGet]
        [Route("get-bonus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetBonus()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);

                if (!userRole.Equals("Admin"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var bonus = await unitOfWork.BonusRepository.GetByID(1);

                if (bonus == null)
                {
                    return Ok(new { bonus = 0, discount = 0 });

                }

                var retVal = new
                {
                    bonus = bonus.BonusPerKilometer,
                    discount = bonus.DiscountPerReservation,
                };

                return Ok(retVal);

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to set bonus");
            }
        }
    }
}
