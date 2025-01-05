using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Account_Apis.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }
    }
}