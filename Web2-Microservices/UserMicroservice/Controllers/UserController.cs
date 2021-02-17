using EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UserMicroservice.DTOs;
using UserMicroservice.IntegrationEvents.Events;
using UserMicroservice.Models;
using UserMicroservice.Repository;

namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IEventBus eventBus;

        public UserController(IUnitOfWork _unitOfWork, IEventBus eventBus)
        {
            unitOfWork = _unitOfWork;
            this.eventBus = eventBus;
        }


        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> s()
        {
            Console.WriteLine("DAAAAAAAAA");
            return Ok("USAO");
        }

        [HttpGet]
        [Route("test2")]
        public async Task<IActionResult> s(string id)
        {
            Console.WriteLine("DAAAAAAAAA");
            return Ok("ID JE:" + id);
        }


        [HttpGet]
        [Route("user-json")]
        public async Task<string> ReturnUserJSON(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return null;
            }

            try
            {
                var user = await unitOfWork.UserRepository.GetByID(id);

                if (user == null)
                {
                    return null;
                }

                return JsonConvert.SerializeObject(user);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("find-user")]
        public async Task<IActionResult> FindUser(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var user = await unitOfWork.UserRepository.GetByID(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return user");
            }
        }

        [HttpGet]
        [Route("get-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to get user");
            }
        }


        #region Friendship methods
        [HttpPost]
        [Route("send-friendship-invitation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> SendFriendshipInvitation([FromBody] AddFriendDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var friend = await unitOfWork.UserManager.FindByIdAsync(dto.UserId);

                if (friend == null)
                {
                    return NotFound("Searched user not found");
                }

                var friendships = await unitOfWork.UserRepository.GetFriends(user as User);
                if (friendships.FirstOrDefault(f => f.User1 == user as User && f.User2 == friend || f.User2 == user && f.User1 == friend) != null)
                {
                    return BadRequest("Already friend");
                }

                User sender = (User)user;
                User receiver = (User)friend;

                var f = new Friendship()
                {
                    Rejacted = false,
                    Accepted = false,
                    User1 = sender,
                    User2 = receiver
                };

                sender.FriendshipInvitations.Add(f);
                receiver.FriendshipRequests.Add(f);

                try
                {
                    unitOfWork.UserRepository.Update(sender);
                    unitOfWork.UserRepository.Update(receiver);

                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to send invitation");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to send invitation");
            }

        }

        [HttpGet]
        [Route("get-user-requests")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserRequests()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var requests = await unitOfWork.UserRepository.GetRequests(user);

                List<object> allrequ = new List<object>();

                foreach (var item in requests)
                {
                    allrequ.Add(new
                    {
                        senderUserName = item.User1.UserName,
                        senderEmail = item.User1.Email,
                        accepted = item.Accepted,
                        senderFirstName = item.User1.FirstName,
                        senderLastName = item.User1.LastName,
                        senderId = item.User1.Id
                    });
                }

                return Ok(allrequ);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return requests");
            }

        }

        [HttpGet]
        [Route("get-all-users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var allUsers = await unitOfWork.UserRepository.GetAllUsers();
                var usersToReturn = new List<object>();

                foreach (var item in allUsers)
                {
                    var role = await unitOfWork.AuthenticationRepository.GetRoles(item);
                    if (role.Count == 0)
                    {
                        continue;
                    }
                    if (!role.FirstOrDefault().Equals("RegularUser") || item.UserName.Equals(user.UserName))
                    {
                        continue;
                    }
                    if (user.Friends.Contains((User)item))
                    {
                        continue;
                    }
                    var invites = await unitOfWork.UserRepository.GetInvitations(user);
                    if (invites.FirstOrDefault(i => i.User2.UserName == item.UserName || i.User1.UserName == item.UserName) != null)
                    {
                        continue;
                    }


                    usersToReturn.Add(new
                    {
                        username = item.UserName,
                        email = item.Email,
                        firstname = item.FirstName,
                        lastname = item.LastName,
                        id = item.Id
                    });
                }
                return Ok(usersToReturn);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return all users");
            }
        }

        [HttpGet]
        [Route("get-friends")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetFriends()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var friends = await unitOfWork.UserRepository.GetFriends(user);

                var usersToReturn = new List<object>();
                foreach (var item in friends)
                {

                    if (!item.Accepted)
                    {
                        continue;
                    }
                    if (item.User2 == user)
                    {
                        usersToReturn.Add(new
                        {
                            username = item.User1.UserName,
                            email = item.User1.Email,
                            firstname = item.User1.FirstName,
                            lastname = item.User1.LastName,
                            id = item.User1.Id,
                        });
                    }
                    else
                    {
                        usersToReturn.Add(new
                        {
                            username = item.User2.UserName,
                            email = item.User2.Email,
                            firstname = item.User2.FirstName,
                            lastname = item.User2.LastName,
                            id = item.User2.Id,
                        });
                    }
                }
                return Ok(usersToReturn);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return user friends");
            }
        }

        [HttpPost]
        [Route("accept-friendship")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AcceptFriendship([FromBody] AddFriendDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var friend = await unitOfWork.UserManager.FindByIdAsync(dto.UserId);

                if (friend == null)
                {
                    return NotFound("Searched user not found");
                }

                var friendships = await unitOfWork.UserRepository.GetFriends(user as User);
                if (friendships.FirstOrDefault(f => f.User1 == user as User && f.User2 == friend
                                                || f.User2 == user && f.User1 == friend) != null)
                {
                    return BadRequest("Already friend");
                }

                var friendship = await unitOfWork.UserRepository.GetSpecificRequest(userId, dto.UserId);

                if (friendship == null)
                {
                    return BadRequest();
                }

                user.Friends.Add(friendship.User1);
                user.FriendshipRequests.FirstOrDefault(f => f.User1Id == friendship.User1.Id && f.User2Id == friendship.User2.Id).Accepted = true;
                friendship.User1.Friends.Add(user);

                try
                {
                    unitOfWork.UserRepository.Update(user);
                    unitOfWork.UserRepository.Update(friendship.User1);

                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to accept friendship. One of transactions failed");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to accept friendship");
            }
        }

        [HttpPost]
        [Route("reject-request")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RejectRequest([FromBody] AddFriendDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var friendship = await unitOfWork.UserRepository.GetSpecificRequest(userId, dto.UserId);

                if (friendship == null)
                {
                    return BadRequest();
                }

                user.FriendshipRequests.Remove(friendship);
                var friend = friendship.User1 == user ? friendship.User2 : friendship.User1;
                friend.FriendshipInvitations.Remove(friendship);

                try
                {
                    unitOfWork.UserRepository.DeleteFriendship(friendship);
                    unitOfWork.UserRepository.Update(user);
                    unitOfWork.UserRepository.Update(friend);

                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to reject request. One of transactions failed");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to reject request");
            }
        }

        [HttpPost]
        [Route("delete-friend")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteFriend([FromBody] AddFriendDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var friendships = await unitOfWork.UserRepository.GetFriends(user);

                if (friendships == null)
                {
                    return BadRequest("You dont have any friendships");
                }

                var friendship = friendships.FirstOrDefault(f => f.User1 == user && f.User2Id == dto.UserId
                                                            || f.User2 == user && f.User1Id == dto.UserId);

                if (friendship == null)
                {
                    return BadRequest("Friendship not found");
                }
                var friend = friendship.User1 == user ? friendship.User2 : friendship.User1;
                user.Friends.Remove(friend);
                friend.Friends.Remove(user);

                try
                {
                    unitOfWork.UserRepository.DeleteFriendship(friendship);
                    unitOfWork.UserRepository.Update(user);
                    unitOfWork.UserRepository.Update(friend);

                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to reject request. One of transactions failed");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to reject request");
            }
        }
        #endregion

        //#region Rent methods
        //[HttpPost]
        //[Route("rent-car")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> RentCar(CarRentDto dto)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        if (dto.TakeOverDate < DateTime.Now.Date)
        //        {
        //            return BadRequest("Date is in past");
        //        }

        //        if (dto.TakeOverDate > dto.ReturnDate)
        //        {
        //            return BadRequest("Takeover date shoud be lower then return date.");
        //        }

        //        var car = (await unitOfWork.CarRepository.AllCars(c => c.CarId == dto.CarRentId)).FirstOrDefault();

        //        if (car == null)
        //        {
        //            return NotFound("Car not found");
        //        }

        //        //provera da li ima special offer za taj period
        //        var specialOffer = car.SpecialOffers.FirstOrDefault(so =>
        //                            dto.ReturnDate >= so.FromDate && so.FromDate >= dto.TakeOverDate ||
        //                            dto.ReturnDate >= so.ToDate && so.FromDate >= dto.TakeOverDate ||
        //                            so.FromDate < dto.TakeOverDate && so.ToDate > dto.ReturnDate);

        //        if (specialOffer != null)
        //        {
        //            return BadRequest("This car has special offer for selected period. Cant rent this car");
        //        }

        //        //provera da li je vec rezervisan u datom periodu
        //        foreach (var rent in car.Rents)
        //        {
        //            if (!(rent.TakeOverDate < dto.TakeOverDate && rent.ReturnDate < dto.TakeOverDate ||
        //                rent.TakeOverDate > dto.ReturnDate && rent.ReturnDate > dto.ReturnDate))
        //            {
        //                return BadRequest("The selected car is reserved for selected period");
        //            }
        //        }

        //        //var racsId = car.BranchId == null ? car.RentACarServiceId : car.Branch.RentACarServiceId;

        //        //var result = await unitOfWork.RentACarRepository.Get(r => r.RentACarServiceId ==racsId, null, "Address,Branches");
        //        //var racs = result.FirstOrDefault();

        //        //if (racs == null)
        //        //{
        //        //    return NotFound("RACS not found");
        //        //}
        //        if (car.Branch != null)
        //        {
        //            if (!car.Branch.City.Equals(dto.TakeOverCity))
        //            {
        //                return BadRequest("Takeover city and rent service/branch city dont match");

        //            }
        //        }
        //        else
        //        {
        //            if (!car.RentACarService.Address.City.Equals(dto.TakeOverCity))
        //            {
        //                return BadRequest("Takeover city and rent service/branch city dont match");
        //            }
        //        }

        //        //provera da li postoje branch gde moze da se vrati auto

        //        var citiesToReturn = new List<string>();

        //        foreach (var item in car.RentACarService == null ? car.Branch.RentACarService.Branches : car.RentACarService.Branches)
        //        {
        //            citiesToReturn.Add(item.City);
        //        }

        //        citiesToReturn.Add(car.RentACarService == null ? car.Branch.RentACarService.Address.City : car.RentACarService.Address.City);

        //        if (!citiesToReturn.Contains(dto.ReturnCity))
        //        {
        //            return BadRequest("Cant return to selected city");
        //        }

        //        //using (var transaction = _context.Database.BeginTransactionAsync())
        //        //{
        //        var carRent = new CarRent()
        //        {
        //            TakeOverCity = dto.TakeOverCity,
        //            ReturnCity = dto.ReturnCity,
        //            TakeOverDate = dto.TakeOverDate,
        //            ReturnDate = dto.ReturnDate,
        //            RentedCar = car,
        //            User = user,
        //            TotalPrice = await CalculateTotalPrice(dto.TakeOverDate, dto.ReturnDate, car.PricePerDay),
        //            RentDate = DateTime.Now
        //        };

        //        user.CarRents.Add(carRent);
        //        car.Rents.Add(carRent);

        //        try
        //        {
        //            unitOfWork.UserRepository.Update(user);
        //            unitOfWork.CarRepository.Update(car);

        //            await unitOfWork.Commit();
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            return BadRequest("Car is modified in the meantime, or reserved by another user");
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Failed to rent car. One of transactions failed");
        //        }

        //        //slanje email-a
        //        try
        //        {
        //            await unitOfWork.AuthenticationRepository.SendRentConfirmationMail(user, carRent);
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Car is rented, but failed to send confirmation email.");
        //        }

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to rent car");
        //    }
        //}

        //[HttpGet]
        //[Route("rent-total-price")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        //public async Task<IActionResult> GetTotalPrice()
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var queryString = Request.Query;
        //        var carId = 0;
        //        DateTime retDate;
        //        DateTime takeDate;

        //        if (!Int32.TryParse(queryString["carId"].ToString(), out carId))
        //        {
        //            return BadRequest();
        //        }
        //        if (!DateTime.TryParse(queryString["ret"].ToString(), out retDate))
        //        {
        //            return BadRequest();
        //        }
        //        if (!DateTime.TryParse(queryString["dep"].ToString(), out takeDate))
        //        {
        //            return BadRequest();
        //        }
        //        if (retDate < takeDate)
        //        {
        //            return BadRequest();
        //        }

        //        var car = await unitOfWork.CarRepository.GetByID(carId);

        //        if (car == null)
        //        {
        //            return NotFound("Car not found");
        //        }
        //        //ovde bi se trebali uracunati i bodovi korisnika, kako bi se uracunala snizenja
        //        var totalPrice = await CalculateTotalPrice(takeDate, retDate, car.PricePerDay);

        //        //var returnData = new {
        //        //    from = queryString["from"].ToString(),
        //        //    to = queryString["to"].ToString(),
        //        //    dep = takeDate,
        //        //    ret = retDate,
        //        //    brand = car.Brand,
        //        //    carId = car.CarId,
        //        //    model = car.Model, =
        //        //    name = car.
        //        //};

        //        return Ok(totalPrice);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to return total price");
        //    }
        //}

        //[HttpDelete]
        //[Route("cancel-rent/{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> CancelRent(int id)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var ress = await unitOfWork.CarRentRepository.Get(crr => crr.CarRentId == id, null, "RentedCar,User");
        //        var rent = ress.FirstOrDefault();
        //        var car = rent.RentedCar;

        //        if (car == null)
        //        {
        //            return NotFound("Car not found");
        //        }

        //        if (rent.User != user)
        //        {
        //            return BadRequest();
        //        }

        //        if (rent.TakeOverDate.AddDays(-2) < DateTime.Now.Date)
        //        {
        //            return BadRequest("Cant cancel reservation");
        //        }

        //        var specialOffers = await unitOfWork.RACSSpecialOfferRepository.Get(s => s.Car == car && s.FromDate == rent.TakeOverDate && s.ToDate == rent.ReturnDate);
        //        var specOffer = specialOffers.FirstOrDefault();

        //        if (specOffer != null)
        //        {
        //            specOffer.IsReserved = false;
        //        }
        //        user.CarRents.Remove(rent);
        //        car.Rents.Remove(rent);

        //        try
        //        {
        //            if (specOffer != null)
        //            {
        //                unitOfWork.RACSSpecialOfferRepository.Update(specOffer);
        //            }
        //            unitOfWork.UserRepository.Update(user);
        //            unitOfWork.CarRepository.Update(car);

        //            await unitOfWork.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Failed to cancel rent car. One of transactions failed");
        //        }

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to cancel rent car");
        //    }
        //}

        //[HttpGet]
        //[Route("get-car-reservations")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> GetRents()
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var rents = await unitOfWork.CarRentRepository.GetRents(user);
        //        var retVal = new List<object>();

        //        foreach (var rent in rents)
        //        {
        //            var sum = 0.0;
        //            foreach (var item in rent.RentedCar.Rates)
        //            {
        //                sum += item.Rate;
        //            }

        //            var rate = sum == 0.0 ? 0.0 : sum / rent.RentedCar.Rates.Count;

        //            retVal.Add(new
        //            {
        //                brand = rent.RentedCar.Brand,
        //                carId = rent.RentedCar.CarId,
        //                carServiceId = rent.RentedCar.RentACarService == null ?
        //                               rent.RentedCar.Branch.RentACarService.RentACarServiceId : rent.RentedCar.RentACarService.RentACarServiceId,
        //                model = rent.RentedCar.Model,
        //                name = rent.RentedCar.RentACarService == null ?
        //                               rent.RentedCar.Branch.RentACarService.Name : rent.RentedCar.RentACarService.Name,
        //                seatsNumber = rent.RentedCar.SeatsNumber,
        //                pricePerDay = rent.RentedCar.PricePerDay,
        //                type = rent.RentedCar.Type,
        //                year = rent.RentedCar.Year,
        //                totalPrice = rent.TotalPrice,
        //                from = rent.TakeOverCity,
        //                to = rent.ReturnCity,
        //                dep = rent.TakeOverDate,
        //                ret = rent.ReturnDate,
        //                city = rent.RentedCar.RentACarService == null ?
        //                               rent.RentedCar.Branch.RentACarService.Address.City : rent.RentedCar.RentACarService.Address.City,
        //                state = rent.RentedCar.RentACarService == null ?
        //                               rent.RentedCar.Branch.RentACarService.Address.State : rent.RentedCar.RentACarService.Address.State,
        //                isCarRated = rent.RentedCar.Rates.FirstOrDefault(r => r.UserId == userId) != null ? true : false,
        //                isRACSRated = (await unitOfWork.RentACarRepository.Get(r => r.RentACarServiceId == (rent.RentedCar.RentACarService == null ?
        //                               rent.RentedCar.Branch.RentACarService.RentACarServiceId : rent.RentedCar.RentACarService.RentACarServiceId), null, "Rates"))
        //                               .FirstOrDefault().Rates.FirstOrDefault(r => r.UserId == userId) == null ? false : true,
        //                canCancel = rent.TakeOverDate.AddDays(-2) >= DateTime.Now.Date,
        //                canRate = rent.ReturnDate < DateTime.Now.Date,
        //                isUpcoming = rent.TakeOverDate >= DateTime.Now.Date,
        //                reservationId = rent.CarRentId,
        //                rate = rate
        //            });
        //        }

        //        return Ok(retVal);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to cancel rent car");
        //    }
        //}

        //private async Task<float> CalculateTotalPrice(DateTime startDate, DateTime endDate, float pricePerDay)
        //{
        //    await Task.Yield();

        //    return (float)((endDate - startDate).TotalDays == 0 ? pricePerDay : pricePerDay * (endDate - startDate).TotalDays);
        //}
        //#endregion

        //#region Rate car/racs methods
        //[HttpPost]
        //[Route("rate-car")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> RateCar(RateDto dto)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        if (dto.Rate > 5 || dto.Rate < 1)
        //        {
        //            return BadRequest("Invalid rate. Rate from 1 to 5");
        //        }

        //        var rent = await unitOfWork.CarRentRepository.GetRentByFilter(r => r.User == user && r.RentedCar.CarId == dto.Id);

        //        //var rents = await unitOfWork.CarRentRepository.Get(crr => crr.User == user, null, "RentedCar");

        //        //var rent = rents.FirstOrDefault(r => r.RentedCar.CarId == dto.Id);

        //        if (rent == null)
        //        {
        //            return BadRequest("This car is not on your rent list");
        //        }
        //        if (rent.RentedCar.Rates.FirstOrDefault(r => r.UserId == userId) != null)
        //        {
        //            return BadRequest("You already rated this car");
        //        }

        //        if (rent.ReturnDate > DateTime.Now)
        //        {
        //            return BadRequest("You can rate this car only when rate period expires");
        //        }

        //        var rentedCar = rent.RentedCar;

        //        rentedCar.Rates.Add(new CarRate()
        //        {
        //            Rate = dto.Rate,
        //            User = user,
        //            UserId = user.Id,
        //            Car = rentedCar
        //        });

        //        try
        //        {
        //            unitOfWork.CarRepository.Update(rentedCar);

        //            await unitOfWork.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Failed to rate car. One of transactions failed");
        //        }

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to rate car");
        //    }
        //}
        //[HttpPost]
        //[Route("rate-car-service")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> RateRACS(RateDto dto)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        if (dto.Rate > 5 || dto.Rate < 1)
        //        {
        //            return BadRequest("Invalid rate. Rate from 1 to 5");
        //        }

        //        var racs = await unitOfWork.RentACarRepository.GetRacsWithRates(dto.Id);

        //        //if (rent.IsRACSRated)
        //        //{
        //        //    return BadRequest("you already rate this racs");
        //        //}

        //        if (racs.Rates.FirstOrDefault(r => r.UserId == userId) != null)
        //        {
        //            return BadRequest("You already rate this racs");
        //        }

        //        var rents = await unitOfWork.CarRentRepository
        //            .Get(crr => crr.User == user &&
        //            crr.RentedCar.RentACarService == null ?
        //            crr.RentedCar.Branch.RentACarService.RentACarServiceId == dto.Id : crr.RentedCar.RentACarService.RentACarServiceId == dto.Id
        //            , null, "RentedCar");

        //        //var rent = rents.FirstOrDefault(r => r.RentedCar.CarId == dto.Id);
        //        var rent = rents.FirstOrDefault();

        //        if (rent == null)
        //        {
        //            return BadRequest("Cant rate this service.");
        //        }

        //        if (rent.ReturnDate > DateTime.Now)
        //        {
        //            return BadRequest("You can rate this rent service only when rate period expires");
        //        }

        //        racs.Rates.Add(new RentCarServiceRates()
        //        {
        //            Rate = dto.Rate,
        //            User = user,
        //            UserId = user.Id,
        //            RentACarService = racs
        //        });

        //        try
        //        {
        //            unitOfWork.RentACarRepository.Update(racs);
        //            await unitOfWork.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Failed to rate car. One of transactions failed");
        //        }

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to rate rent service");
        //    }
        //}
        //#endregion



        //[HttpPost]
        //[Route("get-trip-info")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> GetTripInfo([FromBody] InfoDto dto)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var mySeats = await unitOfWork.SeatRepository.Get(s => dto.MySeatsIds.Contains(s.SeatId));
        //        var seats = new List<object>();

        //        foreach (var item in mySeats)
        //        {
        //            seats.Add(new
        //            {
        //                row = item.Row,
        //                clas = item.Class,
        //                col = item.Column,
        //            });
        //        }

        //        var friendList = new List<object>();

        //        foreach (var item in dto.Friends)
        //        {
        //            var friend = await unitOfWork.UserRepository.GetByID(item.Id);
        //            var seat = await unitOfWork.SeatRepository.GetByID(item.SeatId);
        //            friendList.Add(new
        //            {
        //                friendEmail = friend.Email,
        //                friendFirstName = friend.FirstName,
        //                friendLastName = friend.LastName,
        //                column = seat.Column,
        //                row = seat.Row,
        //                clas = seat.Class,
        //            });
        //        }
        //        var unregisteredFriendList = new List<object>();

        //        foreach (var item in dto.UnregisteredFriends)
        //        {
        //            var seat = await unitOfWork.SeatRepository.GetByID(item.SeatId);
        //            unregisteredFriendList.Add(new
        //            {
        //                lastName = item.LastName,
        //                firstName = item.FirstName,
        //                passport = item.Passport,
        //                column = seat.Column,
        //                row = seat.Row,
        //                clas = seat.Class,
        //            });
        //        }

        //        float totalPrice = 0;

        //        foreach (var item in mySeats)
        //        {
        //            totalPrice += item.Price;
        //        }

        //        float priceWithBonus = 0;

        //        if (totalPrice < user.BonusPoints * 0.01)
        //        {
        //            priceWithBonus = 0;
        //        }
        //        else
        //        {
        //            priceWithBonus = (float)(totalPrice - user.BonusPoints * 0.01);
        //        }

        //        return Ok(new
        //        {
        //            priceWithBonus = priceWithBonus,
        //            totalPrice = totalPrice,
        //            friends = friendList,
        //            unregisteredFriends = unregisteredFriendList,
        //            mySeats = seats,
        //            myBonus = user.BonusPoints
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to return trip info");
        //    }
        //}

        //#endregion

        //#region Trip methods
        //[HttpGet]
        //[Route("get-previous-flights")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> GetPreviousFlights()
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var trips = await unitOfWork.FlightReservationRepository.GetTrips(user);
        //        var retVal = new List<object>();

        //        foreach (var trip in trips)
        //        {
        //            foreach (var ticket in trip.Tickets)
        //            {
        //                if (ticket.Seat.Flight.LandingDateTime >= DateTime.Now)
        //                {
        //                    continue;
        //                }
        //                List<object> stops = new List<object>();

        //                foreach (var stop in ticket.Seat.Flight.Stops)
        //                {
        //                    stops.Add(new
        //                    {
        //                        stop.Destination.City,
        //                        stop.Destination.State
        //                    });
        //                }

        //                retVal.Add(new
        //                {
        //                    column = ticket.Seat.Column,
        //                    row = ticket.Seat.Row,
        //                    clas = ticket.Seat.Class,
        //                    seatId = ticket.Seat.SeatId,
        //                    seatPrice = ticket.Seat.Price,
        //                    takeOffDate = ticket.Seat.Flight.TakeOffDateTime.Date,
        //                    landingDate = ticket.Seat.Flight.LandingDateTime.Date,
        //                    airlineLogo = ticket.Seat.Flight.Airline.LogoUrl,
        //                    airlineName = ticket.Seat.Flight.Airline.Name,
        //                    airlineId = ticket.Seat.Flight.Airline.AirlineId,
        //                    from = ticket.Seat.Flight.From.City,
        //                    to = ticket.Seat.Flight.To.City,
        //                    takeOffTime = ticket.Seat.Flight.TakeOffDateTime.TimeOfDay,
        //                    landingTime = ticket.Seat.Flight.LandingDateTime.TimeOfDay,
        //                    flightTime = ticket.Seat.Flight.TripTime,
        //                    flightLength = ticket.Seat.Flight.tripLength,
        //                    flightNumber = ticket.Seat.Flight.FlightNumber,
        //                    flightId = ticket.Seat.Flight.FlightId,
        //                    stops = stops,
        //                    isAirlineRated = ticket.Seat.Flight.Airline.Rates.FirstOrDefault(r => r.User == user) == null ?
        //                                     false : true,
        //                    isFlightRated = ticket.Seat.Flight.Rates.FirstOrDefault(r => r.User == user) == null ?
        //                                     false : true,
        //                });
        //            }
        //        }

        //        return Ok(retVal);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to return trips");
        //    }
        //}

        //[HttpGet]
        //[Route("get-upcoming-trips")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> GetUpcomingTrips()
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var trips = await unitOfWork.FlightReservationRepository.GetTrips(user);
        //        var flights = new List<object>();
        //        var retVal = new List<object>();

        //        foreach (var trip in trips)
        //        {
        //            var previous = trip.Tickets.Where(t => t.Seat.Flight.LandingDateTime < DateTime.Now);
        //            if (previous.ToList().Count == trip.Tickets.ToList().Count)
        //            {
        //                continue;
        //            }

        //            flights = new List<object>();

        //            foreach (var ticket in trip.Tickets)
        //            {

        //                List<object> stops = new List<object>();

        //                foreach (var stop in ticket.Seat.Flight.Stops)
        //                {
        //                    stops.Add(new
        //                    {
        //                        stop.Destination.City,
        //                        stop.Destination.State
        //                    });
        //                }

        //                flights.Add(new
        //                {
        //                    column = ticket.Seat.Column,
        //                    row = ticket.Seat.Row,
        //                    clas = ticket.Seat.Class,
        //                    seatId = ticket.Seat.SeatId,
        //                    seatPrice = ticket.Seat.Price,
        //                    takeOffDate = ticket.Seat.Flight.TakeOffDateTime.Date,
        //                    landingDate = ticket.Seat.Flight.LandingDateTime.Date,
        //                    airlineLogo = ticket.Seat.Flight.Airline.LogoUrl,
        //                    airlineName = ticket.Seat.Flight.Airline.Name,
        //                    airlineId = ticket.Seat.Flight.Airline.AirlineId,
        //                    from = ticket.Seat.Flight.From.City,
        //                    to = ticket.Seat.Flight.To.City,
        //                    takeOffTime = ticket.Seat.Flight.TakeOffDateTime.TimeOfDay,
        //                    landingTime = ticket.Seat.Flight.LandingDateTime.TimeOfDay,
        //                    flightTime = ticket.Seat.Flight.TripTime,
        //                    flightLength = ticket.Seat.Flight.tripLength,
        //                    flightNumber = ticket.Seat.Flight.FlightNumber,
        //                    flightId = ticket.Seat.Flight.FlightId,
        //                    stops = stops,
        //                    canCancel = Math.Abs((ticket.Seat.Flight.TakeOffDateTime - DateTime.Now).TotalHours) >= 3,
        //                });
        //            }
        //            retVal.Add(new
        //            {
        //                reservationId = trip.FlightReservationId,
        //                flights = flights,
        //                totalPrice = trip.Price
        //            });
        //        }

        //        return Ok(retVal);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to return trips");
        //    }
        //}
        //[HttpPost]
        //[Route("accept-trip-invitation")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> AcceptTripInvitation(AcceptTripDto dto)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var invitation = await unitOfWork.TripInvitationRepository.GetTripInvitationById(dto.Id);

        //        if (invitation == null)
        //        {
        //            return BadRequest("Cant find your invitation");
        //        }


        //        var flightReservation = new FlightReservation()
        //        {
        //            User = user,
        //        };

        //        var tickets = new List<Ticket>();

        //        tickets.Add(new Ticket()
        //        {
        //            Passport = dto.Passport,
        //            Price = invitation.Price,
        //            Seat = invitation.Seat,
        //            SeatId = invitation.SeatId,
        //            Reservation = flightReservation
        //        });

        //        var systemB = await unitOfWork.BonusRepository.GetByID(1);

        //        int systemBonus = 0;

        //        if (systemB == null)
        //        {
        //            systemBonus = 0;
        //        }
        //        else
        //        {
        //            systemBonus = systemB.BonusPerKilometer;
        //        }

        //        flightReservation.Tickets = tickets;

        //        user.FlightReservations.Add(flightReservation);
        //        user.TripRequests.Remove(invitation);
        //        user.BonusPoints += (int)invitation.Seat.Flight.tripLength * systemBonus;
        //        invitation.Sender.TripInvitations.Remove(invitation);

        //        try
        //        {
        //            unitOfWork.UserRepository.Update(user);
        //            unitOfWork.UserRepository.Update(invitation.Sender);

        //            unitOfWork.TripInvitationRepository.Delete(invitation);

        //            await unitOfWork.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            StatusCode(500, "Failed to accept invitation");
        //        }
        //        try
        //        {

        //        }
        //        catch (Exception)
        //        {
        //            StatusCode(500, "Successfully accepted invitation, but unable to send email.");
        //        }

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to accept trip");
        //    }
        //}

        //[HttpDelete]
        //[Route("reject-trip-invitation/{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> RejectTripInvitation(int id)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var invitation = await unitOfWork.TripInvitationRepository.GetTripInvitationById(id);

        //        if (invitation == null)
        //        {
        //            return BadRequest("Cant find your invitation");
        //        }


        //        user.TripRequests.Remove(invitation);
        //        invitation.Sender.TripInvitations.Remove(invitation);

        //        invitation.Seat.Available = true;
        //        invitation.Seat.Reserved = false;

        //        try
        //        {
        //            unitOfWork.UserRepository.Update(user);
        //            unitOfWork.UserRepository.Update(invitation.Sender);
        //            unitOfWork.SeatRepository.Update(invitation.Seat);

        //            unitOfWork.TripInvitationRepository.Delete(invitation);

        //            await unitOfWork.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            StatusCode(500, "Failed to reject invitation");
        //        }
        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to reject invitation");
        //    }
        //}

        //[HttpGet]
        //[Route("get-trip-invitations")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> GetTripInvitations()
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var invitations = await unitOfWork.TripInvitationRepository.GetTripInvitations(user);
        //        var retVal = new List<object>();

        //        foreach (var invite in invitations)
        //        {
        //            if (invite.Expires <= DateTime.Now)
        //            {
        //                user.TripRequests.Remove(invite);
        //                invite.Sender.TripInvitations.Remove(invite);

        //                invite.Seat.Available = true;
        //                invite.Seat.Reserved = false;

        //                try
        //                {
        //                    unitOfWork.UserRepository.Update(user);
        //                    unitOfWork.UserRepository.Update(invite.Sender);
        //                    unitOfWork.SeatRepository.Update(invite.Seat);

        //                    unitOfWork.TripInvitationRepository.Delete(invite);

        //                    await unitOfWork.Commit();
        //                }
        //                catch (Exception)
        //                {

        //                }
        //                continue;
        //            }
        //            retVal.Add(new
        //            {
        //                column = invite.Seat.Column,
        //                row = invite.Seat.Row,
        //                clas = invite.Seat.Class,
        //                seatId = invite.Seat.SeatId,
        //                seatPrice = invite.Seat.Price,
        //                takeOffDate = invite.Seat.Flight.TakeOffDateTime.Date,
        //                landingDate = invite.Seat.Flight.LandingDateTime.Date,
        //                airlineLogo = invite.Seat.Flight.Airline.LogoUrl,
        //                airlineName = invite.Seat.Flight.Airline.Name,
        //                airlineId = invite.Seat.Flight.Airline.AirlineId,
        //                from = invite.Seat.Flight.From.City,
        //                to = invite.Seat.Flight.To.City,
        //                takeOffTime = invite.Seat.Flight.TakeOffDateTime.TimeOfDay,
        //                landingTime = invite.Seat.Flight.TakeOffDateTime.TimeOfDay,
        //                flightTime = invite.Seat.Flight.TripTime,
        //                flightLength = invite.Seat.Flight.tripLength,
        //                flightNumber = invite.Seat.Flight.FlightNumber,
        //                flightId = invite.Seat.Flight.FlightId,
        //                senderId = invite.Sender.Id,
        //                senderUserName = invite.Sender.UserName,
        //                senderFirstName = invite.Sender.FirstName,
        //                senderLastName = invite.Sender.LastName,
        //                senderEmail = invite.Sender.Email,
        //                invitationId = invite.InvitationId
        //            });
        //        }

        //        return Ok(retVal);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to return trips");
        //    }
        //}
        //#endregion

        #region Rate flight/air methods
        [HttpPost]
        [Route("rate-flight")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RateFlight(RateDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (dto.Rate > 5 || dto.Rate < 1)
                {
                    return BadRequest("Invalid rate. Rate from 1 to 5");
                }


                var @event = new RateFlightIntegrationEvent(userId, dto.Id, dto.Rate);

                try
                {
                    eventBus.Publish(@event);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to publish rateFlight event");
                    return StatusCode(500, "Failed to rate flight.");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to rate flight");
            }
        }
        [HttpPost]
        [Route("rate-airline")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RateAirline(RateDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (dto.Rate > 5 || dto.Rate < 1)
                {
                    return BadRequest("Invalid rate. Rate from 1 to 5");
                }

                var @event = new RateAirlineIntegrationEvent(userId, dto.Id, dto.Rate);

                try
                {
                    eventBus.Publish(@event);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to publish rateAirline event");
                    return StatusCode(500, "Failed to rate airline.");
                }

                return Ok();

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to rate airline");
            }
        }
        #endregion

        //#region Reserve special offers of flight/car
        //[HttpPost]
        //[Route("reserve-special-offer-car")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ReserveSpecialOfferCar([FromBody] ReserveDto dto)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var specialOffer = await unitOfWork.RACSSpecialOfferRepository.GetSpecialOfferById(dto.Id);

        //        if (specialOffer == null)
        //        {
        //            return NotFound("Selected special offer is not found");
        //        }

        //        if (specialOffer.IsReserved)
        //        {
        //            return BadRequest("Already reserved");
        //        }

        //        foreach (var rent in specialOffer.Car.Rents)
        //        {
        //            if (!(rent.TakeOverDate < specialOffer.FromDate && rent.ReturnDate < specialOffer.FromDate ||
        //                rent.TakeOverDate > specialOffer.ToDate && rent.ReturnDate > specialOffer.ToDate))
        //            {
        //                return BadRequest("The selected car is reserved for selected period");
        //            }
        //        }

        //        var carRent = new CarRent()
        //        {
        //            TakeOverDate = specialOffer.FromDate,
        //            ReturnDate = specialOffer.ToDate,
        //            TakeOverCity = specialOffer.Car.RentACarService == null ?
        //                            specialOffer.Car.Branch.City : specialOffer.Car.RentACarService.Address.City,
        //            ReturnCity = specialOffer.Car.RentACarService == null ?
        //                            specialOffer.Car.Branch.City : specialOffer.Car.RentACarService.Address.City,
        //            RentedCar = specialOffer.Car,
        //            TotalPrice = specialOffer.NewPrice,
        //            RentDate = DateTime.Now
        //        };

        //        user.CarRents.Add(carRent);
        //        specialOffer.Car.Rents.Add(carRent);
        //        specialOffer.IsReserved = true;

        //        try
        //        {
        //            unitOfWork.UserRepository.Update(user);
        //            unitOfWork.CarRepository.Update(specialOffer.Car);
        //            unitOfWork.RACSSpecialOfferRepository.Update(specialOffer);

        //            await unitOfWork.Commit();
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            return BadRequest("Car is modified in the meantime, or reserved by another user");
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Failed to rent car. One of transactions failed");
        //        }

        //        //slanje email-a
        //        try
        //        {
        //            await unitOfWork.AuthenticationRepository.SendRentConfirmationMail(user, carRent);
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Car is rented, but failed to send confirmation email.");
        //        }

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to reserve special offer");
        //    }
        //}


        //#endregion

    }
}
