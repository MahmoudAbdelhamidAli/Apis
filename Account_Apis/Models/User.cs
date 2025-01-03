using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account_Apis.Models
{
    public class User
    {
        
        public int UserId { get; set; } = 0;
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public int IsActive { get; set; } = 1;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}