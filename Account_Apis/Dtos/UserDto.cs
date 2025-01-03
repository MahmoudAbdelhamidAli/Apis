using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Account_Apis.Dtos
{
    public record UserDto
    {
        [Required]
        [Range(1, 20)]
        public string? FirstName { get; set; } 
        [Required]
        [Range(1, 20)]
        public string? LastName { get; set; }
        [Required]
        [Range(1, 50)]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [Range(1, 20)]
        public string? Password { get; set; }
    }
}