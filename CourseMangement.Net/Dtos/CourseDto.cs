using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Dtos
{
    public class CourseDto
    {
        [Required]
        public string CourseName { get; set; }
        [Required]
        public string Description { get; set; }
    }
}