using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Models
{
    public class User
    {
        
        public int UserId { get; set; } = 0;
        public string UserName { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }
        
        public string PasswordHash { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;
        
        public int IsActive { get; set; } = 1;
        public DateTime CreatedDate { get; set; } = DateTime.Now;


        // // foreign key

        // public int CourseId { get; set; }

        // public Course Course { get; set; }


        // // Navigation property for courses
        // public ICollection<Course> Courses { get; set; }

        public ICollection<UserCourse> UserCourses { get; set; }
    }
}