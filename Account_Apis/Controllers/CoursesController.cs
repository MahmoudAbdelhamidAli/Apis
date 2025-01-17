using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Account_Apis.Constants;
using Account_Apis.Data;
using Account_Apis.Dtos;
using Account_Apis.Interfaces;
using Account_Apis.Models;
using Account_Apis.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Account_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CoursesController(MyDbContext context)
        {
            _context = context;
        }

        // add course to login account user (by its token)
        [HttpPost]
        [Authorize]
        [Route("add-course")]
        public async Task<IActionResult> AddCourse([FromBody] CourseDto _course)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ResponseMessages.UnauthorizedAccess);
                }

                // Check if the course already exists (optional logic)
                var existingCourse = await _context.Courses
                    .FirstOrDefaultAsync(c => c.CourseName == _course.CourseName && c.Description == _course.Description);

                if (existingCourse == null)
                {
                    // Create the course if it doesn't exist
                    existingCourse = new Course
                    {
                        CourseName = _course.CourseName,
                        Description = _course.Description
                    };

                    await _context.Courses.AddAsync(existingCourse);
                    await _context.SaveChangesAsync();
                }

                // Check if the user is already enrolled in the course
                var userIdInt = int.Parse(userId);
                var isUserEnrolled = await _context.UserCourses
                    .AnyAsync(uc => uc.UserId == userIdInt && uc.CourseId == existingCourse.CourseId);

                if (isUserEnrolled)
                {
                    return BadRequest(ResponseMessages.CourseAlreadyEnrolled);
                }

                // Associate the user with the course
                var userCourse = new UserCourse
                {
                    UserId = userIdInt,
                    CourseId = existingCourse.CourseId
                };

                await _context.UserCourses.AddAsync(userCourse);
                await _context.SaveChangesAsync();

                return Ok(ResponseMessages.CourseAddedSuccessfully);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // get all courses of login account user (by its token)
        [HttpGet]
        [Authorize]
        [Route("my-courses")]
        public async Task<IActionResult> GetUserCourses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ResponseMessages.UnauthorizedAccess);
                }

                var userIdInt = int.Parse(userId);

                // Retrieve courses associated with the user
                var courses = await _context.UserCourses
                    .Where(uc => uc.UserId == userIdInt)
                    .Include(uc => uc.Course) // Include course details
                    .Select(uc => new
                    {
                        uc.Course.CourseId,
                        uc.Course.CourseName,
                        uc.Course.Description
                    })
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        //////////////////////////// 
        // enroll in existing course 
        [HttpPost]
        [Authorize]
        [Route("enroll-course")]
        public async Task<IActionResult> EnrollCourse([FromBody] int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ResponseMessages.UnauthorizedAccess);
                }

                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                {
                    return NotFound(ResponseMessages.CourseNotFound);
                }

                var userCourseExists = await _context.UserCourses
                    .AnyAsync(uc => uc.UserId == int.Parse(userId) && uc.CourseId == courseId);

                if (userCourseExists)
                {
                    return BadRequest(ResponseMessages.CourseAlreadyEnrolled);
                }

                var userCourse = new UserCourse
                {
                    UserId = int.Parse(userId),
                    CourseId = courseId
                };

                await _context.UserCourses.AddAsync(userCourse);
                await _context.SaveChangesAsync();

                return Ok(ResponseMessages.CourseEnrolledSuccessfully);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // get all enrolled courses for a logged in user 
        [HttpGet]
        [Authorize]
        [Route("my-enrolled-courses")]
        public async Task<IActionResult> GetAllEnrolledCoursesOfLoggedinUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ResponseMessages.UnauthorizedAccess);
                }

                var userCourses = await _context.UserCourses
                    .Where(uc => uc.UserId == int.Parse(userId))
                    .Include(uc => uc.Course)
                    .Select(uc => new
                    {
                        uc.Course.CourseId,
                        uc.Course.CourseName,
                        uc.Course.Description,
                        uc.EnrollmentDate
                    })
                    .ToListAsync();

                return Ok(userCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

    }
}
