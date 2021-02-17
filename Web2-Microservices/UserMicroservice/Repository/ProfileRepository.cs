using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Data;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly UserContext context;
        private readonly UserManager<Person> userManager;
        private readonly SignInManager<Person> _signInManager;

        public ProfileRepository(UserContext _context, UserManager<Person> _userManager, SignInManager<Person> signInManager)
        {
            _signInManager = signInManager;
            context = _context;
            userManager = _userManager;
        }
        public async Task<IdentityResult> ChangeProfile(Person user)
        {
            return await userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangeEmail(Person user, string email)
        {
            var emailChangeToken = await userManager.GenerateChangeEmailTokenAsync(user, email);
            return await userManager.ChangeEmailAsync(user, email, emailChangeToken);
        }
        public async Task<IdentityResult> ChangePassword(Person user, string newPassword)
        {
            using (var transaction = context.Database.BeginTransactionAsync())
            {
                try
                {
                    user.PasswordChanged = true;
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, newPassword);
                    var result = await userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        return IdentityResult.Failed(result.Errors.First());
                    }

                    //var res = await userManager.ChangePasswordAsync(user, user.PasswordHash, newPassword);
                    await transaction.Result.CommitAsync();

                    return IdentityResult.Success;
                }
                catch (Exception)
                {
                    await transaction.Result.RollbackAsync();
                    return IdentityResult.Failed();
                }

            }
        }

        public async Task<IdentityResult> ChangePhone(Person user, string phone)
        {
            var phoneToken = await userManager.GenerateChangePhoneNumberTokenAsync(user, phone);
            return await userManager.ChangePhoneNumberAsync(user, phone, phoneToken);
        }

        public async Task<IdentityResult> ChangeUserName(Person user, string username)
        {
            return await userManager.SetUserNameAsync(user, username);
        }

        public async Task<IdentityResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return IdentityResult.Success;
        }

        public bool PasswordMatch(string newPassword, string passwConfirm)
        {
            return newPassword.Equals(passwConfirm);
        }

    }
}
