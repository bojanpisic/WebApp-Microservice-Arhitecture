using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.DTOs;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public interface IAuthenticationRepository
    {
        Task<IdentityResult> RegisterUser(User user, string password);
        Task<IdentityResult> RegisterAirlineAdmin(AirlineAdmin admin, string password);
        Task<IdentityResult> RegisterSystemAdmin(Person admin, string password);
        Task<IdentityResult> RegisterRACSAdmin(Person admin, string password);
        Task<Person> GetPerson(string email, string password);
        //Task<Person> GetPersonById(int id, UserManager<Person> userManager);
        Task<bool> CheckPassword(Person user, string password);
        Task<bool> IsEmailConfirmed(Person user);
        Task<IList<string>> GetRoles(Person user);
        Task<IdentityResult> AddToRole(Person user, string roleName);
        bool VerifyToken(string providerToken);
        bool CheckPasswordMatch(string password, string confirmPassword);
        Task<IdentityResult> SendConfirmationMail(Person user, string usertype, string password = "");
        Task<IdentityResult> SendRentConfirmationMail(string receiverEmail, RentInfo info);
        Task<IdentityResult> SendMailToFriend(Person inviter, string receiverEmail, FlightInfo info);
        Task<IdentityResult> SendTicketConfirmationMail(string receiverEmail, TripInfo info);
        Task<IdentityResult> ConfirmEmail(Person user, string token);
        Task<Person> GetUserById(string id);
        Task<Person> GetPersonByUserName(string username);
        Task<Person> GetPersonByEmail(string email);
    }
}
