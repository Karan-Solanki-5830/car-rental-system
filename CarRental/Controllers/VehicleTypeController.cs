using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleTypeController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly FluentValidation.IValidator<VehicleType> _validator;

        public VehicleTypeController(CarRentalContext context, FluentValidation.IValidator<VehicleType> validator)
        {
            _context = context;
            _validator = validator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllVehicleTypes()
        {
            try
            {
                var types = await _context.VehicleTypes.ToListAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load vehicle types.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleTypeById(int id)
        {
            try
            {
                var type = await _context.VehicleTypes.FindAsync(id);
                if (type == null)
                    return NotFound(new { message = "Vehicle type not found." });

                return Ok(type);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load vehicle type.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicleType([FromBody] VehicleType type)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(type);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                type.Created = DateTime.UtcNow;
                type.Modified = DateTime.UtcNow;

                _context.VehicleTypes.Add(type);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Vehicle type created successfully.", vehicleTypeId = type.VehicleTypeId });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to create vehicle type due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create vehicle type.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateVehicleType(int id, [FromBody] VehicleType type)
        {
            try
            {
                if (id != type.VehicleTypeId)
                    return BadRequest(new { message = "Vehicle type ID mismatch." });

                var validationResult = await _validator.ValidateAsync(type);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                var existing = await _context.VehicleTypes.FindAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Vehicle type not found." });

                existing.TypeName = type.TypeName;
                existing.UserId = type.UserId;
                existing.Modified = DateTime.UtcNow;

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Vehicle type updated successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to update vehicle type due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update vehicle type.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicleType(int id)
        {
            try
            {
                var vehicleType = await _context.VehicleTypes.FindAsync(id);
                if (vehicleType == null)
                    return NotFound(new { message = "Vehicle type not found." });

                try
                {
                    _context.VehicleTypes.Remove(vehicleType);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Vehicle type deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete: Vehicle type is linked to other records." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete vehicle type.", detail = ex.Message });
            }
        }
    }
}
