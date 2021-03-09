using EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly HttpClient httpClient;

        public UserController(IUnitOfWork _unitOfWork, IEventBus eventBus, HttpClient httpClient)
        {
            unitOfWork = _unitOfWork;
            this.eventBus = eventBus;
            this.httpClient = httpClient;
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
                    Console.WriteLine("notfound");
                    return NotFound();
                }

                Console.WriteLine("user found");
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return user");
            }
        }

        [HttpGet]
        [Route("find-userbyid")]
        public async Task<IActionResult> FindUserById(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var user = await unitOfWork.UserRepository.GetByID(id) as User;
                return Ok(user);
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


                unitOfWork.UserRepository.Update(sender);
                unitOfWork.UserRepository.Update(receiver);

                await unitOfWork.Commit();

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

                unitOfWork.UserRepository.Update(user);
                unitOfWork.UserRepository.Update(friendship.User1);

                await unitOfWork.Commit();

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

                unitOfWork.UserRepository.DeleteFriendship(friendship);
                unitOfWork.UserRepository.Update(user);
                unitOfWork.UserRepository.Update(friend);

                await unitOfWork.Commit();

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

                unitOfWork.UserRepository.DeleteFriendship(friendship);
                unitOfWork.UserRepository.Update(user);
                unitOfWork.UserRepository.Update(friend);

                await unitOfWork.Commit();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to reject request");
            }
        }
        #endregion

        [HttpPost]
        [Route("accept-trip-invitation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AcceptTripInvitation(AcceptTripDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var invitation = await unitOfWork.TripInvitationRepository.GetTripInvitationById(dto.Id);

                if (invitation == null)
                {
                    return BadRequest("Cant find your invitation");
                }

                var @event = new CreateFlightReservationEvent(invitation.InvitationId, user.Id, invitation.SeatId,dto.Passport, invitation.Price);

                eventBus.Publish(@event);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to accept trip");
            }
        }

        [HttpDelete]
        [Route("reject-trip-invitation/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RejectTripInvitation(int id)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }
                var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var invitation = await unitOfWork.TripInvitationRepository.GetTripInvitationById(id);

                if (invitation == null)
                {
                    return BadRequest("Cant find your invitation");
                }

                var @event1 = new RejectTripInvitationEvent(invitation.SeatId, invitation.InvitationId);
                eventBus.Publish(@event1);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to reject invitation");
            }
        }

        [HttpGet]
        [Route("get-trip-invitations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTripInvitations()
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

                var invitations = await unitOfWork.TripInvitationRepository.GetTripInvitations(user);
                var retVal = new List<object>();

                foreach (var invite in invitations)
                {
                    if (invite.Expires <= DateTime.Now)
                    {
                        var @event1 = new RejectTripInvitationEvent(invite.SeatId, invite.InvitationId);
                        eventBus.Publish(@event1);

                        continue;
                    }

                    //get seat info 

                    var result = await httpClient.GetStringAsync(String.Format("http://airlinemicroservice:80/api/seat/get-seat?id={0}",invite.SeatId));
                    dynamic data = JObject.Parse(result);

                    retVal.Add(new
                    {
                        column = data.column,
                        row = data.row,
                        clas = data.class1,
                        seatId = data.seatId,
                        seatPrice = data.price,
                        takeOffDate = data.takeOffDate,
                        landingDate = data.landingDate,
                        airlineLogo = data.airlineLogo,
                        airlineName = data.airlineName,
                        airlineId = data.airlineId,
                        from = data.from,
                        to = data.to,
                        takeOffTime = data.takeOffTime,
                        landingTime = data.landingTime,
                        flightTime = data.flightTime,
                        flightLength = data.flightLength,
                        flightNumber = data.flightNumber,
                        flightId = data.flightId,
                        senderId = invite.Sender.Id,
                        senderUserName = invite.Sender.UserName,
                        senderFirstName = invite.Sender.FirstName,
                        senderLastName = invite.Sender.LastName,
                        senderEmail = invite.Sender.Email,
                        invitationId = invite.InvitationId
                    });
                }

                return Ok(retVal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Failed to return trips");
            }
        }
    }
}
