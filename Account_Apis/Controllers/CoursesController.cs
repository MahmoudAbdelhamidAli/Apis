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

                var course = new Course
                {
                    CourseName = _course.CourseName,
                    Description = _course.Description,
                    UserId = Convert.ToInt32(userId)
                };

                await _context.Courses.AddAsync(course);
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

                var courses = await _context.Courses
                    .Where(c => c.UserId == int.Parse(userId))
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
