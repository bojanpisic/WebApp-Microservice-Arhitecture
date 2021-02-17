using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public interface IProfileRepository
    {
        Task<IdentityResult> Logout();
        Task<IdentityResult> ChangeEmail(Person user, string emai);
        Task<IdentityResult> ChangePassword(Person user, string newPasword);
        Task<IdentityResult> ChangeUserName(Person user, string username);
        Task<IdentityResult> ChangeProfile(Person user);
        Task<IdentityResult> ChangePhone(Person user, string phone);
    }
}
