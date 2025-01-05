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

        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                throw new ArgumentException("Email content cannot be null or empty", nameof(message.Content));
            }
            
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