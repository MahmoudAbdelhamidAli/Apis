using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Account_Apis.Models;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace Account_Apis.Service
{
    public class EmailService : IEmailService
    {
        private readonly SMTPConfigModel _emailConfig;

        public EmailService(SMTPConfigModel emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);

            // var email = new MimeMessage();
            // email.From.Add(new MailboxAddress("Your Name", "mahmoud13abdelhamid@gmail.com"));
            // email.To.Add(new MailboxAddress("lol", "mahmoud123abdelhamid@gmail.com"));
            // email.Subject = "Test Email";
            // email.Body = new TextPart("plain")
            // {
            //     Text = "This is a test email."
            // };

            // using (var smtp = new SmtpClient())
            // {
            //     smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            //     smtp.Authenticate("mahmoud13abdelhamid@gmail.com", "Mahmoud123**");
            //     smtp.Send(email);
            //     smtp.Disconnect(true);
            // }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email",_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.smtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}