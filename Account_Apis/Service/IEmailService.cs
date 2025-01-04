using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account_Apis.Service
{
    public interface IEmailService
    {
        // send email
        Task SendEmail(string email, string subject, string message);
    }
}