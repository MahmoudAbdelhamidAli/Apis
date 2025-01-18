using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Dtos
{
    public class ForgetPasswordDto
    {
        [Required, EmailAddress, Display(Name ="Registered email address")]
        public string? Email { get; set; }
    }
}