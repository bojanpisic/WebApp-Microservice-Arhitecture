using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserMicroservice.DTOs;
using UserMicroservice.Models;
using UserMicroservice.Repository;

namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly SignInManager<Person> _signInManager;
        private IUnitOfWork unitOfWork;

        public AuthenticationController(SignInManager<Person> signInManager, IConfiguration config, IUnitOfWork _unitOfWork)
        {
            this._signInManager = signInManager;
            this.configuration = config;
            this.unitOfWork = _unitOfWork;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User newUser;

            try
            {
                //if ((await unitOfWork.AuthenticationRepository.GetPersonByEmail(userDto.Email)) != null)
                //{
                //    return BadRequest("User with that email already exists!");
                //}

                if ((await unitOfWork.AuthenticationRepository.GetPersonByUserName(userDto.UserName)) != null)
                {
                    return BadRequest("User with that username already exists!");
                }
                if (!unitOfWork.AuthenticationRepository.CheckPasswordMatch(userDto.Password, userDto.ConfirmPassword))
                {
                    return BadRequest("Passwords dont match");
                }

                newUser = new User()
                {
                    Email = userDto.Email,
                    UserName = userDto.UserName,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    ImageUrl = userDto.ImageUrl,
                    PhoneNumber = userDto.Phone,
                    City = userDto.City,
                    EmailConfirmed = false,
                };

                await unitOfWork.AuthenticationRepository.RegisterUser(newUser, userDto.Password);
                await unitOfWork.AuthenticationRepository.AddToRole(newUser, "RegularUser");

                await unitOfWork.Commit();

            }
            catch (DbUpdateException) 
            {
                Console.WriteLine("Failed to register. transaction failed");
                return StatusCode(500, "Registration failed.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Registration failed.");
            }

            try
            {
                var emailSent = await unitOfWork.AuthenticationRepository.SendConfirmationMail(newUser, "user");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return StatusCode(500, "Sending email failed.");
            }
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await this.unitOfWork.AuthenticationRepository.GetPerson(loginUser.UserNameOrEmail
                    /*String.IsNullOrEmpty(loginUser.Email) ? loginUser.Username : loginUser.Email*/,
                    loginUser.Password);
                if (user == null)
                {
                    return NotFound("Username or email doesnt exist");
                }

                if (loginUser.Token != null && loginUser.UserId != null)
                {
                    await unitOfWork.AuthenticationRepository.ConfirmEmail(user, loginUser.Token);
                }

                var isPasswordCorrect = await this.unitOfWork.AuthenticationRepository.CheckPassword(user, loginUser.Password);

                if (!isPasswordCorrect)
                {
                    return BadRequest("Username or email or password is incorrect");
                }

                var role = (await unitOfWork.AuthenticationRepository.GetRoles(user)).FirstOrDefault();

                if (role == "RegularUser")
                {
                    var isEmailConfirmed = await this.unitOfWork.AuthenticationRepository.IsEmailConfirmed(user);

                    if (!isEmailConfirmed)
                    {
                        return BadRequest("Email not confirmed");
                    }
                    //var systemB = await unitOfWork.BonusRepository.GetByID(1);

                    //int systemBonus = 0;

                    //if (systemB == null)
                    //{
                    //    systemBonus = 0;
                    //}
                    //else
                    //{
                    //    systemBonus = systemB.BonusPerKilometer;
                    //}
                    //var trips = await unitOfWork.FlightReservationRepository.GetTrips(user as User);

                    //foreach (var trip in trips)
                    //{
                    //    foreach (var ticket in trip.Tickets)
                    //    {
                    //        if (ticket.Seat.Flight.LandingDateTime >= DateTime.Now)
                    //        {
                    //            continue;
                    //        }

                    //        int bonus = 0;
                    //        Int32.TryParse(ticket.Seat.Flight.tripLength.ToString(), out bonus);
                    //        (user as User).BonusPoints += bonus * systemBonus;

                    //        try
                    //        {
                    //            unitOfWork.UserRepository.Update(user);
                    //            await unitOfWork.Commit();
                    //        }
                    //        catch (Exception)
                    //        {
                    //        }

                    //    }
                    //}
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("Roles", role),
                        new Claim("PasswordChanged", user.PasswordChanged.ToString())
                        //new Claim("EmailConfirmed", user.EmailConfirmed.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value)),
                            SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return Ok(new { token });
            }
            catch (Exception)
            {
                return StatusCode(500, "Login failed.");
            }
        }

        [HttpPost]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                var user = await unitOfWork.AuthenticationRepository.GetUserById(userId);
                if (user == null)
                {
                    return BadRequest(new IdentityError() { Code = "User dont exist" });
                }

                var result = await unitOfWork.AuthenticationRepository.ConfirmEmail(user, token);

                if (result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Email confirmation failed.");
            }
        }

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("social-login")]
        //public async Task<IActionResult> SocialLogin([FromBody] SocialNetworkDto dto)
        //{
        //    if (unitOfWork.AuthenticationRepository.VerifyToken(dto.IdToken))
        //    {

        //        try
        //        {
        //            var user = await this.unitOfWork.AuthenticationRepository.GetPersonByEmail(dto.Email);
        //            string role = "RegularUser";

        //            if (user == null)
        //            {
        //                //registracija
        //                user = new User()
        //                {
        //                    Email = dto.Email,
        //                    UserName = dto.Email,
        //                    FirstName = dto.FirstName,
        //                    LastName = dto.LastName,
        //                    EmailConfirmed = true,
        //                };
        //                try
        //                {
        //                    await unitOfWork.AuthenticationRepository.RegisterUser(user as User, "");
        //                    await unitOfWork.AuthenticationRepository.AddToRole(user, "RegularUser");
        //                    await unitOfWork.Commit();
        //                    //transaction.Complete();
        //                }
        //                catch (Exception)
        //                {
        //                    //unitOfWork.Rollback();
        //                    return StatusCode(500, "Registration failed.");
        //                }
        //                //}
        //                try
        //                {
        //                    var emailSent = await unitOfWork.AuthenticationRepository.SendConfirmationMail(user, "user");
        //                }
        //                catch (Exception)
        //                {
        //                    return StatusCode(500, "Sending email failed.");
        //                }
        //            }
        //            else
        //            {
        //                role = (await unitOfWork.AuthenticationRepository.GetRoles(user)).FirstOrDefault();
        //            }


        //            var tokenDescriptor = new SecurityTokenDescriptor
        //            {
        //                Subject = new ClaimsIdentity(new Claim[]
        //           {
        //                new Claim("UserID",user.Id.ToString()),
        //                new Claim("Roles", role),
        //           }),
        //                Expires = DateTime.UtcNow.AddMinutes(5),
        //                //Key min: 16 characters
        //                SigningCredentials = new SigningCredentials(
        //               new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value)),
        //               SecurityAlgorithms.HmacSha256Signature)
        //            };
        //            var tokenHandler = new JwtSecurityTokenHandler();
        //            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        //            var token = tokenHandler.WriteToken(securityToken);
        //            return Ok(new { token });
        //        }
        //        catch (Exception)
        //        {
        //        }

        //    }

        //    return BadRequest();
        //}

        [AllowAnonymous]
        [HttpPost]
        [Route("socials-login")]
        public async Task<IActionResult> SignInWithGoogle()
        {
            var authenticationProperties = _signInManager
                    .ConfigureExternalAuthenticationProperties("Google", Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, "Google");
        }

        public async Task<IActionResult> HandleExternalLogin()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (!result.Succeeded) //user does not exist yet
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.Name);

                var newUser = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                };
                var createResult = await unitOfWork.UserManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                    throw new Exception(createResult.Errors.Select(e => e.Description).Aggregate((errors, error) => $"{errors}, {error}"));

                await unitOfWork.UserManager.AddLoginAsync(newUser, info);
                var newUserClaims = info.Principal.Claims.Append(new Claim("userId", newUser.Id));
                await unitOfWork.UserManager.AddClaimsAsync(newUser, newUserClaims);
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            return Redirect("http://localhost:4200");
        }
    }
}
