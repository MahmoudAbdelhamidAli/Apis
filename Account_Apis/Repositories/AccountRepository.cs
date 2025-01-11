using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account_Apis.Interfaces;
using Account_Apis.Models;
using Account_Apis.Service;
using Microsoft.AspNetCore.Identity;

namespace Account_Apis.Repositories
{
    public class AccountRepository : IAccountRepository
    {

        private readonly UserManager<AppUser> _userManager;
        

        private readonly IEmailService _emailService;

        public AccountRepository(UserManager<AppUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;

        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            Console.WriteLine($"Searching for user with email: {email}");

            var user = await _userManager.FindByEmailAsync(email);
            Console.WriteLine($"Found user: {user?.NormalizedEmail ?? "null"}");
            
            return user;
        }


        //  GenerateForgotPasswordTokenAsync
        public async Task GenerateForgotPasswordTokenAsync(AppUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
        }

        // add private method SendForgotPasswordEmail
        private async Task SendForgotPasswordEmail(AppUser user, string token)
        {
            var email = user.Email.Trim(); 
            email = new string(email.Where(c => !char.IsControl(c)).ToArray()); 
            
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email); 
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid email format: {email}");
                return;
            }
            var subject = "Reset Password";
            var body = $"Please reset your password by clicking here: ......Tobe continued";
        
        }






        
    }
}