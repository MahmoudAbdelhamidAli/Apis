using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseMangement.Net.Models;

namespace CourseMangement.Net.Service
{
    public interface IEmailService
    {
        // send email
        Task SendEmail(Message message);
    }
}