using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using TRS_backend.API_Models;
using TRS_backend.DBModel;
using TRS_backend.Models;
using TRS_backend.Operational;

namespace TRS_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TRSDbContext _dbContext;
        public UserController(IConfiguration configuration, TRSDbContext context)
        {
            _configuration = configuration;
            _dbContext = context;
        }

        // Look up the user in the database and check if the user exists
        // if the user exists: hash the password and compare it with the stored hash
        // If the password is correct: return a JWT token
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] DTOLoginRequset requestBody)
        {
            // Check if we got a valid input
            if (requestBody.Username is null && requestBody.Email is null || requestBody.Password is null) {
                return BadRequest("Failed to login");
            }
            // Regex to only allow lower- and uppercase letters, numbers, and hyphens
            Regex usernameRegex = new Regex("[^a-zA-Z0-9-]");
            if (requestBody.Username is not null && !usernameRegex.IsMatch(requestBody.Username))
            {
                return BadRequest("Failed to login");
            }
            // Use Mail.MailAddress to validate the email
            if (requestBody.Email is not null) {
                var email = new System.Net.Mail.MailAddress(requestBody.Email);
                if (email is null)
                {
                    return BadRequest("Failed to login");
                }
            }

            // Look up the user in the database by email or username
            var user = await _dbContext.Users.Select(u => u).Where(u => u.Username == requestBody.Username || u.Email == requestBody.Email).FirstAsync();

            // Check if the user exists by checking if it is not null
            if (user is null)
            {
                return BadRequest("Failed to login");
            }
            
            // Hash the given password
            byte[] hashedPassword = Crypto.HashPassword(requestBody.Password, user.Salt);

            //Debug.WriteLine($"Hashed password: { BitConverter.ToString(hashedPassword) }");
            //Debug.WriteLine($"Stored password: { BitConverter.ToString(user.PasswordHash) }");
            //Debug.WriteLine($"Is equal: {hashedPassword.SequenceEqual(user.PasswordHash)}");

            // Check if the password is correct
            if (hashedPassword.SequenceEqual(user.PasswordHash)) {
                
                // Generate and return a JWT token
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(_configuration["JWT:JWTSigningKey"]!);

                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, UserRole.Admin.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["JWT:Issuer"],
                    Audience = _configuration["JWT:Audience"]
                };

                return Ok(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));
            }

            return BadRequest("Failed to login");
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterUser([FromBody] DTORegisterUserRequest requestBody)
        {
            // Validate the input
            if (requestBody.Username is null || requestBody.Email is null || requestBody.Password is null)
            {
                return BadRequest("Failed to register");
            }
            // Regex to only allow lower- and uppercase letters, numbers, and hyphens
            Regex usernameRegex = new Regex("[^a-zA-Z0-9-]");
            if (!usernameRegex.IsMatch(requestBody.Username)) {
                return BadRequest("Failed to register");
            }
            // Use Mail.MailAddress to validate the email
            var email = new System.Net.Mail.MailAddress(requestBody.Email);
            if (email is null) {
                return BadRequest("Failed to register");
            }
            
            // Check if the user already exists
            bool userExists = await _dbContext.Users.AnyAsync(user => user.Username == requestBody.Username || user.Email == requestBody.Email);
            if (userExists) {
                return BadRequest("Failed to register");
            }

            // Ready to register user in the database
            // Hash the password
            byte[] salt = Crypto.GenerateRandomBytes(32);
            byte[] hashedPassword = Crypto.HashPassword(requestBody.Password, salt);

            // Create a new user
            TblUsers newUser = new TblUsers
            {
                Username = requestBody.Username,
                Email = requestBody.Email,
                Role = UserRole.Admin,
                PasswordHash = hashedPassword,
                Salt = salt,
                CreatedAt = DateTime.Now
            };

            return Ok("Successfully registered user.");
        }

    }
}
