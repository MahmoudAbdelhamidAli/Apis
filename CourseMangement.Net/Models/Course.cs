using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }

        
        
        
        // // // Foreign Key
        // public int UserId { get; set; }

        // // // Navigation property
        // public User User { get; set; }

        // public ICollection<User> Users { get; set; }

        public ICollection<UserCourse> UserCourses { get; set; }

        

        
    }
}