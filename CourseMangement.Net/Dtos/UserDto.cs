using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Dtos
{
    public record UserDto
    {
        [Required]
        public string? UserName { get; set; } 
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        
        public string? Password { get; set; }
    }
}