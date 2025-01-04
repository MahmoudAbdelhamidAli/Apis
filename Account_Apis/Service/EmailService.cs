using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Account_Apis.Models;
using Microsoft.Extensions.Options;

namespace Account_Apis.Service
{
    public class EmailService : IEmailService
    {
        private readonly SMTPConfigModel _smtpConfig;
        public EmailService(IOptions<SMTPConfigModel> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }

        public async Task SendEmail(string email, string subject, string message)
        {
            // add mail message
            MailMessage mail = new MailMessage
            {
                Subject = subject,
                Body = message,
                From = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
                IsBodyHtml = _smtpConfig.IsBodyHTML
            };
            mail.To.Add(email);
            // add smtp client
            NetworkCredential networkCredential = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password);

            SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtpConfig.Host,
                Port = _smtpConfig.Port,
                EnableSsl = _smtpConfig.EnableSSL,
                UseDefaultCredentials = _smtpConfig.UseDefaultCredentials,
                Credentials = networkCredential
            };
            // send email
            mail.BodyEncoding = Encoding.Default;
            await smtpClient.SendMailAsync(mail);

        }
    }
}