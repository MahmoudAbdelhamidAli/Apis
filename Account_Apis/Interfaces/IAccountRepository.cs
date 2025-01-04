using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account_Apis.Models;

namespace Account_Apis.Interfaces
{
    public interface IAccountRepository
    {
        Task<AppUser> GetUserByEmailAsync(string email);
        Task GenerateForgotPasswordTokenAsync(AppUser user);
    }
}