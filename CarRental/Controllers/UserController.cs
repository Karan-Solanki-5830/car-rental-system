using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRental.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly IValidator<User> _validator;
        private readonly IConfiguration _configuration;

        public UserController(CarRentalContext context, IValidator<User> validator, IConfiguration configuration)
        {
            _context = context;
            _validator = validator;
            _configuration = configuration;

        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var expiryMinutes = Convert.ToDouble(jwtSettings["TokenExpiryMinutes"]);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest loginUser)
        {
            try
            {
                if (string.IsNullOrEmpty(loginUser.Name) || string.IsNullOrEmpty(loginUser.Password))
                    return BadRequest(new { message = "Username and password are required." });

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Name == loginUser.Name && u.Password == loginUser.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid username or password" });

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    message = "Login successful.",
                    token,
                    user = new
                    {
                        user.UserId,
                        user.Name,
                        user.Email,
                        user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to login.", detail = ex.Message });
            }
        }



        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> PostUser([FromForm] User user, IFormFile? profileImage)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(user);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                // Handle profile image upload
                if (profileImage != null && profileImage.Length > 0)
                {
                    try
                    {
                        user.ProfileImagePath = ProfileImageHelper.SaveProfileImage(profileImage);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(new { message = ex.Message });
                    }
                }

                user.Created = DateTime.Now;
                user.Modified = DateTime.Now;

                _context.Users.Add(user);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to create user due to database constraints." });
                }

                if (user.Role.ToLower() == "customer")
                {
                    Console.WriteLine("Customer creation logic triggered.");
                    try
                    {
                        await CreateOrUpdateCustomer(user);
                    }
                    catch (DbUpdateException)
                    {
                        return BadRequest(new { message = "User created but failed to sync customer record due to database constraints." });
                    }
                }

                return Ok(new { message = "User created successfully.", data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create user.", detail = ex.Message });
            }
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromForm] User user, IFormFile? profileImage)
        {
            try
            {
                if (id != user.UserId)
                    return BadRequest(new { message = "User ID mismatch." });

                var validationResult = await _validator.ValidateAsync(user);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    return NotFound(new { message = "User not found." });

                // Handle profile image upload
                if (profileImage != null && profileImage.Length > 0)
                {
                    try
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingUser.ProfileImagePath))
                        {
                            ProfileImageHelper.DeleteProfileImage(existingUser.ProfileImagePath);
                        }
                        existingUser.ProfileImagePath = ProfileImageHelper.SaveProfileImage(profileImage);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(new { message = ex.Message });
                    }
                }

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.Phone = user.Phone;
                existingUser.Role = user.Role;
                existingUser.Modified = DateTime.Now;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to update user due to database constraints." });
                }

                if (user.Role.ToLower() == "customer")
                {
                    try
                    {
                        await CreateOrUpdateCustomer(existingUser);
                    }
                    catch (DbUpdateException)
                    {
                        return BadRequest(new { message = "User updated but failed to sync customer record due to database constraints." });
                    }
                }

                return Ok(new { message = "User updated successfully.", data = existingUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update user.", detail = ex.Message });
            }
        }

        // GET: api/User
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                // Update profile image paths
                foreach (var user in users)
                {
                    user.ProfileImagePath = ProfileImageHelper.GetProfileImagePath(user.ProfileImagePath);
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load users.", detail = ex.Message });
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                // Update profile image path
                user.ProfileImagePath = ProfileImageHelper.GetProfileImagePath(user.ProfileImagePath);

                // Return bare object for simpler frontend deserialization
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load user.", detail = ex.Message });
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (user.Role.ToLower() == "customer")
                {
                    var customer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Email.ToLower() == user.Email.ToLower());

                    if (customer != null)
                        _context.Customers.Remove(customer);

                }

                _context.Users.Remove(user);
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "User deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete user due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete user.", detail = ex.Message });
            }
        }

        private async Task CreateOrUpdateCustomer(User user)
        {
            Console.WriteLine("Inside CreateOrUpdateCustomer with: " + user.Email);

            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == user.Email.ToLower());

            if (existingCustomer == null)
            {
                Console.WriteLine("Creating new customer...");
                var newCustomer = new Customer
                {
                    FullName = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };
                _context.Customers.Add(newCustomer);
            }
            else
            {
                Console.WriteLine("Updating existing customer...");
                existingCustomer.FullName = user.Name;
                existingCustomer.Phone = user.Phone;
                existingCustomer.Modified = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetUserCount()
        {
            try
            {
                var count = await _context.Users.CountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load user count.", detail = ex.Message });
            }
        }
        public static class ProfileImageHelper
        {
            public static string DirectoryName = "ProfileImages";
            public static string DefaultProfilePath = $"{DirectoryName}/default.jpg";

            public static string SaveProfileImage(IFormFile imageFile)
            {
                if (imageFile == null || imageFile.Length == 0)
                    return null;

                string imagesPath = Path.Combine("wwwroot", DirectoryName);
                if (!Directory.Exists(imagesPath))
                    Directory.CreateDirectory(imagesPath);

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                    throw new ArgumentException("Invalid file type. Only JPG, PNG, and GIF files are allowed.");

                // Validate file size (max 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("File size too large. Maximum size is 5MB.");

                string uniqueFile = $"{Guid.NewGuid()}{fileExtension}";
                string fullPath = Path.Combine(imagesPath, uniqueFile);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                return $"{DirectoryName}/{uniqueFile}";
            }

            public static void DeleteProfileImage(string filePath)
            {
                if (string.IsNullOrEmpty(filePath) || filePath == DefaultProfilePath)
                    return;

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            public static string GetProfileImagePath(string? profileImagePath)
            {
                if (string.IsNullOrEmpty(profileImagePath))
                {
                    return DefaultProfilePath;
                }

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", profileImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                return System.IO.File.Exists(fullPath) ? profileImagePath : DefaultProfilePath;
            }
        }



        [HttpDelete("profile-picture/{id}")]
        public async Task<IActionResult> DeleteProfilePicture(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (!string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    ProfileImageHelper.DeleteProfileImage(user.ProfileImagePath);
                    user.ProfileImagePath = null;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Profile picture deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete profile picture.", detail = ex.Message });
            }
        }
    }
}
