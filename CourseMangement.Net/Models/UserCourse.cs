using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Models
{
    public class UserCourse
    {
        public int UserCourseId { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key to User
        public User User { get; set; } // Navigation Property

        public int CourseId { get; set; } // Foreign Key to Course
        public Course Course { get; set; } // Navigation Property

        public DateTime EnrollmentDate { get; set; } = DateTime.Now; // Additional field
    }
}