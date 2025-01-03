using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Account_Apis.Dtos
{
    public class LoginDto
    {
        [Required]
        [Range(1, 50)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Range(1, 20)]
        public string? Password { get; set; }
    }
}