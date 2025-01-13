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
using BCrypt.Net; 

namespace Account_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly MyDbContext _context;
        
        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;

        
        public AccountController(
            MyDbContext context, 
            IEmailService emailService, 
            
            IConfiguration configuration
            )
        {
            _context = context;
            
            _emailService = emailService;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseMessages.InvalidModelState);

            var userExists = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            if (userExists) return BadRequest(ResponseMessages.UserAlreadyExists);

            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                NormalizedEmail = userDto.Email.ToUpper(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password) // Hash password
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var token = Guid.NewGuid().ToString(); // Simulated token
            var confirmEmailLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new[] { user.Email }, "Confirm Email", confirmEmailLink!);
            await _emailService.SendEmail(message);

            return Ok(ResponseMessages.UserCreatedSuccessfully);
        }

        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            // Simulate email confirmation (you can implement actual token logic if needed)

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return BadRequest(ResponseMessages.UserNotFound);

            // Simulate confirming email
            user.IsEmailConfirmed = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(ResponseMessages.EmailConfirmedSuccessfully);
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(ResponseMessages.InvalidUserNameOrPassword);
            }

            var jwtSettings = _configuration.GetSection("JWT");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }

        [HttpGet]
        [Authorize]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.Select(u => new { u.UserId, u.UserName, u.Email }).ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(ResponseMessages.UserNotFound);

            return Ok(new { user.UserId, user.UserName, user.Email });
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(ResponseMessages.UserNotFound);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(ResponseMessages.UserDeletedSuccessfully);
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == forgetPasswordDto.Email);
            if (user == null) return BadRequest(ResponseMessages.UserNotFound);

            var token = Guid.NewGuid().ToString(); // Simulated token
            var resetPasswordLink = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new[] { user.Email }, "Password Reset Link", resetPasswordLink!);
            await _emailService.SendEmail(message);

            return Ok(ResponseMessages.PasswordResetLinkSent);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == resetPasswordDto.Email);
            if (user == null) return BadRequest(ResponseMessages.UserNotFound);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(ResponseMessages.PasswordResetSuccessfully);
        }

        [HttpGet]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(ResponseMessages.UnauthorizedAccess);

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null) return NotFound(ResponseMessages.UserNotFound);

            return Ok(new { user.UserId, user.UserName, user.Email });
        }

    }
}