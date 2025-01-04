using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account_Apis.Interfaces;
using Account_Apis.Models;
using Microsoft.AspNetCore.Identity;

namespace Account_Apis.Repositories
{
    public class AccountRepository : IAccountRepository
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountRepository(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }


        //  GenerateForgotPasswordTokenAsync
        public Task GenerateForgotPasswordTokenAsync(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}