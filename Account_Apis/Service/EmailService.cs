using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Account_Apis.Service
{
    public class EmailService : IEmailService
    {
        
        public EmailService()
        {
        }

        public async Task SendEmail(string email, string subject, string message)
        {
            // add mail message
            MailMessage mail = new MailMessage
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                From = new MailAddress("...To be continued")
            };
            mail.To.Add(email);
            // add smtp client
            SmtpClient smtpClient = new SmtpClient
            {
                Host = "...To be continued",
                Port = 587,
                Credentials = new System.Net.NetworkCredential("...To be continued", "...To be continued"),
                EnableSsl = true
            };
            // send email
            mail.BodyEncoding = Encoding.Default;
            await smtpClient.SendMailAsync(mail);

        }
    }
}