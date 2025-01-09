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
        // private readonly IAccountRepository _accountRepository;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;

        
        public AccountController(
            MyDbContext context, 
            IEmailService emailService, 
            // IAccountRepository accountRepository
            IConfiguration configuration
            )
        {
            _context = context;
            // _accountRepository = accountRepository;
            _emailService = emailService;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("lol------------------sign-up")]
        public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid input data.");

            var userExists = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            if (userExists) return BadRequest("User already exists.");

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

            return Ok("User created successfully. Please confirm your email.");
        }

        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            // Simulate email confirmation (you can implement actual token logic if needed)

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return BadRequest("User not found.");

            // Simulate confirming email
            user.IsEmailConfirmed = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Email confirmed successfully.");
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            var jwtSettings = _configuration.GetSection("JWT");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim(ClaimTypes.NameIdentifier, user.UserId),
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
            if (user == null) return NotFound("User not found.");

            return Ok(new { user.UserId, user.UserName, user.Email });
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

    }
}