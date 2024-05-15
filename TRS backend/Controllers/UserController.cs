using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TRS_backend.API_Models;
using TRS_backend.DBModel;

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
        public async Task<ActionResult<string>> Login([FromBody] LoginUserCredentials _credentials)
        {
            // Look up the user in the database by email or username
            var user = await _dbContext.Users.Select(u => u).Where(u => u.Username == _credentials.Username || u.Email == _credentials.Email).FirstAsync();

            // Check if the user exists by checking if it is not null
            if (user is null)
            {
                return NotFound("User not found");
            }
            
            // Hash the given password
            byte[] hashedPassword = Crypto.HashPassword(_credentials.Password, user.Salt);

            Debug.WriteLine($"Hashed password: { BitConverter.ToString(hashedPassword) }");
            Debug.WriteLine($"Stored password: { BitConverter.ToString(user.PasswordHash) }");

            Debug.WriteLine($"Is equal: {hashedPassword.SequenceEqual(user.PasswordHash)}");
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

            return Ok("Login");
        }

        [HttpPost("Register")]
        public ActionResult<string> RegisterUser([FromBody] RegisterUserInfo userInfo)
        {
            

            return Ok("User registered");
        }

    }
}
