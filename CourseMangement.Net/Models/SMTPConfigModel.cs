using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Models
{
    public class SMTPConfigModel
    {
        public string From { get; set; }
        public string smtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
    }
}