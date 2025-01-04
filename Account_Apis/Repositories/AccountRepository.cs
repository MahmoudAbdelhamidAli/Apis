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
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public AccountRepository(UserManager<AppUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;

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
            var email = user.Email;
            var subject = "Reset Password";
            var body = $"Please reset your password by clicking here: ......Tobe continued";
            await _emailService.SendEmailAsync(email, subject, body);
        }

        
    }
}