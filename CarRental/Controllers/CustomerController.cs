using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public CustomerController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]

        public async Task<IActionResult> GetCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                await SyncCustomersFromUsers();

                var totalRecords = await _context.Customers.CountAsync();

                var customersWithRoles = await _context.Customers
                    .Select(c => new
                    {
                        c.CustomerId,
                        c.FullName,
                        c.Email,
                        c.Phone,
                        c.Created,
                        c.Modified,
                        Role = _context.Users
                            .Where(u => u.Email.ToLower() == c.Email.ToLower())
                            .Select(u => u.Role)
                            .FirstOrDefault() ?? "Customer"
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    totalRecords,
                    page,
                    pageSize,
                    customers = customersWithRoles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load customers.", detail = ex.Message });
            }
        }

        private async Task SyncCustomersFromUsers()
        {
            var customerUsers = await _context.Users
                .Where(u => u.Role.ToLower() == "customer")
                .ToListAsync();

            bool changesMade = false;
            foreach (var user in customerUsers)
            {
                var exists = await _context.Customers.AnyAsync(c => c.Email.ToLower() == user.Email.ToLower());
                if (!exists)
                {
                    _context.Customers.Add(new Customer
                    {
                        FullName = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    });
                    changesMade = true;
                }
            }

            if (changesMade)
            {
                await _context.SaveChangesAsync();
            }
        }


        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found." });
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load customer.", detail = ex.Message });
            }
        }

        [HttpGet("search")]

        public async Task<IActionResult> SearchCustomers([FromQuery] string? name, [FromQuery] string? email, [FromQuery] string? phone)
        {
            try
            {
                var customers = _context.Customers.AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                    customers = customers.Where(c => c.FullName.Contains(name));

                if (!string.IsNullOrWhiteSpace(email))
                    customers = customers.Where(c => c.Email.Contains(email));

                if (!string.IsNullOrWhiteSpace(phone))
                    customers = customers.Where(c => c.Phone.Contains(phone));

                var result = await customers
                    .Select(c => new
                    {
                        c.CustomerId,
                        c.FullName,
                        c.Email,
                        c.Phone,
                        c.Created,
                        c.Modified,
                        Role = _context.Users
                            .Where(u => u.Email.ToLower() == c.Email.ToLower())
                            .Select(u => u.Role)
                            .FirstOrDefault() ?? "Customer"
                    })
                    .ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to search customers.", detail = ex.Message });
            }
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetCustomerCount()
        {
            try
            {
                var count = await _context.Customers.CountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load customer count.", detail = ex.Message });
            }
        }
    }
}
