using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using stockmanager.Data;
using stockmanager.Models;
using System.Security.Claims;
using System.Text;

namespace stockmanager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly StockManagerDbContext _dbContext;
        public LoginController(IConfiguration config, StockManagerDbContext dbContext)
        {
            _configuration = config;
            _dbContext = dbContext;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(UserLogin userCredential)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userCredential.Email);

            if (user == null) return NotFound();

            var responseWrongPassword = new Response() { detail = "Provided password is wrong" };

            if (!BCrypt.Net.BCrypt.Verify(userCredential.Password, user.Password)) return BadRequest(responseWrongPassword);

            var claims = new[] {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                //_configuration["Jwt:Subject"]
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("Email", user.Email),
                new Claim("Username", user.Username),
            };

            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(1000),
                        signingCredentials: signIn);

            var responseLogin = new Response() { detail = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token) };

            return Ok(responseLogin);
        }
    }
}
