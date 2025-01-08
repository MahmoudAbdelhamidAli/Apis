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
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;
        // private readonly IAccountRepository _accountRepository;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;

        
        public UsersController(
            MyDbContext context, 
            IEmailService emailService, 
            UserManager<IdentityUser> userManager
            // IAccountRepository accountRepository
            , IConfiguration configuration
            )
        {
            _context = context;
            // _accountRepository = accountRepository;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }
        

        // sign up using _userManager
        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessages.InvalidModelState);
            }

            // verify if user already exists or not 
            
            var userExists= await _userManager.FindByEmailAsync(userDto.Email!);

            if (userExists != null)
            {
                return BadRequest(ResponseMessages.UserAlreadyExists);
            }
            // add user to the database
            IdentityUser user = new ()
            {
                UserName = userDto.UserName,
                SecurityStamp=Guid.NewGuid().ToString(),
                Email = userDto.Email
            };
            var result = await _userManager.CreateAsync(user, userDto.Password!);

            if(result.Succeeded)
            {
                    // send email confirmation
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var comfirmEmailLink = Url.Action(nameof(ConfirmEmail), "Users", new {token,email =user.Email }, Request.Scheme);
                    var message = new Message(new string[]{ user.Email! }, "Confirm Email", comfirmEmailLink!);
                    await _emailService.SendEmail(message);

                    return Ok(ResponseMessages.UserCreatedSuccessfully); 

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }
                return Ok(ModelState);
            }
            
        }

        // confirm email
        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail( string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok(ResponseMessages.EmailConfirmedSuccessfully);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("error", error.Description);
                    }
                    return Ok(ModelState);
                }
            }
            else
            {
                return BadRequest(ResponseMessages.UserNotFound);
            }

        }


        // login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // verify if user already exists or not

            var user = await _userManager.FindByNameAsync(loginDto.UserName!);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password!))
            {
                var jwtSettings = _configuration.GetSection("JWT");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]!);

                //  Define the Token Descriptor
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {   
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName!)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                // create JwtSecurityToken
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token); 

                // send email confirmation
                // var tokenLink = Url.Action(nameof(ConfirmEmail), "Users", new {tokenString ,email = user.Email }, Request.Scheme);

                // if (string.IsNullOrEmpty(tokenLink))
                // {
                //     return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate login link with the Token.");
                // }

                // var message = new Message(new string[]{ user.Email! }, "Token Link", tokenLink !);
                // await _emailService.SendEmail(message);
                return Ok(new {token = tokenString});

                
            }
            return Unauthorized();
        }

        
        // get all users
        [HttpGet]
        [Authorize]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        // get user by id
        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(ResponseMessages.UserNotFound);
            }
            else
            {
                return Ok(user);
            }
            
        }

        // delete user
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(ResponseMessages.UserNotFound);
            }
            else
            {
                
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(ResponseMessages.UserDeletedSuccessfully);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("error", error.Description);
                    }
                    return Ok(ModelState);
                }
            }
        }

        
        // test email sending 
        [HttpGet]
        [Route("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            var message = new Message(["mahmoud123abdelhamid@gmail.com"], "Test email", "LOOOOL");
            await _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status200OK);
        }
        


        // forgot password 
        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // verify if user already exists or not 
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email!);
            ;
            

            if (user != null)
            {

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var ForgetPasswordLink = Url.Action(nameof(ResetPassword), "Users", new {token ,email = user.Email }, Request.Scheme);

                if (string.IsNullOrEmpty(ForgetPasswordLink))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ResponseMessages.FailedToGenerateResetLink);
                }

                var message = new Message(new string[]{ user.Email! }, "Forget Password Link", ForgetPasswordLink!);
                await _emailService.SendEmail(message);
                return Ok(ResponseMessages.PasswordResetLinkSent);

            }
            else
            {
                return BadRequest(ResponseMessages.UserNotFound);
            }
        }

        // Get reset password
        [HttpGet]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new ResetPasswordDto {
                Token = token,
                Email = email
            };

            return Ok(model);
            
        }

        // reset password (Update password)
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);
            

            if (user != null)
            {

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("error", error.Description);
                    }
                    return Ok(ModelState);
                }
                else
                {
                    return BadRequest(ResponseMessages.PasswordResetSuccessfully);
                }   

            }
            else
            {
                return BadRequest(ResponseMessages.UserNotFound);
            }
        }

        // Method to retrieve profile information of the logged-in user
        [HttpGet]
        [Route("profile")]
        [Authorize] 
        public async Task<IActionResult> GetUserProfile()
        {
            // Retrieve the user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ResponseMessages.UnauthorizedAccess);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(ResponseMessages.UserNotFound);
            }

            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!
            };

            return Ok(userProfile);
        }



    }
}