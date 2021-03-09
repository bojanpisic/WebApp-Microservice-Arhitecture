using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserMicroservice.DTOs;
using UserMicroservice.Models;
using UserMicroservice.Repository;

namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public ProfileController(IUnitOfWork _unitOfWork, IConfiguration config)
        {
            unitOfWork = _unitOfWork;
            this.configuration = config;
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> s()
        {
            Console.WriteLine("DAAAAAAAAA");
            return Ok("sadasdasda");
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await unitOfWork.ProfileRepository.Logout();

            return Ok();
        }

        [HttpPut]
        [Route("change-city/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]


        public async Task<IActionResult> ChangeCity([FromBody] ChangeCityDto profile, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.City = profile.City;

                var result = await unitOfWork.ProfileRepository.ChangeProfile(user);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save changes");
            }
        }

        [HttpPut]
        [Route("change-email/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]


        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto profile, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var result = await unitOfWork.ProfileRepository.ChangeEmail(user, profile.Email);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save changes");
            }
        }

        [HttpPut]
        [Route("change-firstname/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeFirstName([FromBody] ChangeFirstNameDto profile, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.FirstName = profile.FirstName;

                var result = await unitOfWork.ProfileRepository.ChangeProfile(user);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save changes");
            }
        }

        [HttpPut]
        [Route("change-lastname/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeLastName([FromBody] ChangeLastNameDto profile, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.LastName = profile.LastName;

                var result = await unitOfWork.ProfileRepository.ChangeProfile(user);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Failed to save changes");
            }
        }

        [HttpPut]
        [Route("change-img/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeImgUrl(string id, IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                using (var stream = new MemoryStream())
                {
                    await img.CopyToAsync(stream);
                    user.ImageUrl = stream.ToArray();
                }

                var result = await unitOfWork.ProfileRepository.ChangeProfile(user);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save changes");
            }
        }

        [HttpPut]
        [Route("change-passw/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto profile, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var oldPasswMatch = await unitOfWork.UserManager.CheckPasswordAsync(user, profile.OldPassword);

                if (!oldPasswMatch)
                {
                    return BadRequest("Old Password dont match");

                }

                if (!profile.Password.Equals(profile.PasswordConfirm))
                {
                    return BadRequest("Passwords dont match");
                }

                bool firstLogin = false;
                if (!user.PasswordChanged && !userRole.Equals("ReqularUser"))
                {
                    firstLogin = true;
                }

                var result = await unitOfWork.ProfileRepository.ChangePassword(user, profile.Password);

                if (!userRole.Equals("ReqularUser") && firstLogin)
                {
                    user.PasswordChanged = true;
                    unitOfWork.UserRepository.Update(user);
                    await unitOfWork.Commit();

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                   {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("Roles", userRole),
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

                return Ok();

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save changes");
            }
        }

        [HttpPut]
        [Route("change-phone/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangePhone([FromBody] ChangePhoneDto profile, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var result = await unitOfWork.ProfileRepository.ChangePhone(user, profile.Phone);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save changes");
            }
        }

        [HttpPut]
        [Route("change-username/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUserNameDto profile, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var result = await unitOfWork.ProfileRepository.ChangeUserName(user, profile.UserName);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save changes");
            }
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("get-profile")]
        //GET : /api/UserProfile
        public async Task<object> GetUserProfile()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (Person)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (String.IsNullOrEmpty(userRole))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return new
                {
                    user.City,
                    user.Email,
                    user.UserName,
                    user.ImageUrl,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    bonus = userRole != "RegularUser" ? 0 : (user as User).BonusPoints
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Failed to return profile");
            }

        }
    }
}
