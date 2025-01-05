using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account_Apis.Models;

namespace Account_Apis.Service
{
    public interface IEmailService
    {
        // send email
        Task SendEmail(Message message);
    }
}