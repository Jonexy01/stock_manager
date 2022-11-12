using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stockmanager.Data;
using stockmanager.Models;

namespace stockmanager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly StockManagerDbContext _dbContext;
        public UserController(StockManagerDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers() {
            return await _dbContext.Users.ToListAsync();
        }

        [HttpGet("id")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id) {
            var user = await _dbContext.Users.FindAsync(id);
            var userResponse = new UserResponse()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role,
                Created = user.Created,
            };
            return user == null ? NotFound() : Ok(userResponse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser(AddUserRequest addUserRequest) {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = addUserRequest.Email,
                Username = addUserRequest.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(addUserRequest.Password),
                Role = addUserRequest.Role,
                Created = DateTime.Now,
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            var userResponse = new UserResponse()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role,
                Created = user.Created,
            };
            return Ok(userResponse);
            //return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(Guid id, User user) {
            if (id == user.Id) return BadRequest();
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("id, oldPassword, newPassword")]
        //[Route("/changepassword")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(Guid id, string oldPassword, string newPassword) {
            var userToUpdate = await _dbContext.Users.FindAsync(id);

            if (userToUpdate == null) return NotFound();

            if (userToUpdate.Password == oldPassword) {
                userToUpdate.Password = newPassword;
                await _dbContext.SaveChangesAsync();
                var response = new Response() { detail = "Password changed successfuly" };
                return Ok(response);
            } else
            {
                var response = new Response() { detail = "Old Password is wrong" };
                return BadRequest(response);
            }
            
        }

        [HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userToDelete = await _dbContext.Users.FindAsync(id);
            if (userToDelete == null) return NotFound();

            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();

            //return NoContent();
            var response = new Response() { detail = "User Deleted Successfuly" };
            return Ok(response);
        }
    }
}
