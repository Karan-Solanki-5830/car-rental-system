using CarRental.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelTypeController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly IValidator<FuelType> _validator;

        public FuelTypeController(CarRentalContext context, IValidator<FuelType> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllFuelTypes()
        {
            try
            {
                var fuelTypes = await _context.FuelTypes.ToListAsync();
                return Ok(fuelTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load fuel types.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFuelTypeById(int id)
        {
            try
            {
                var fuelType = await _context.FuelTypes.FindAsync(id);
                if (fuelType == null)
                {
                    return NotFound(new { message = "Fuel type not found." });
                }

                return Ok(fuelType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load fuel type.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFuelType([FromBody] FuelType fuelType)
        {
            try
            {
                var result = await _validator.ValidateAsync(fuelType);
                if (!result.IsValid)
                {
                    return BadRequest(result.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                fuelType.Created = DateTime.UtcNow;
                fuelType.Modified = DateTime.UtcNow;

                _context.FuelTypes.Add(fuelType);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        message = "Fuel type created successfully.",
                        fuelTypeId = fuelType.FuelTypeId
                    });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to create fuel type due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create fuel type.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFuelType(int id, [FromBody] FuelType fuelType)
        {
            try
            {
                if (id != fuelType.FuelTypeId)
                {
                    return BadRequest(new { message = "Fuel type ID mismatch." });
                }

                var result = await _validator.ValidateAsync(fuelType);
                if (!result.IsValid)
                {
                    return BadRequest(result.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                var existingFuelType = await _context.FuelTypes.FindAsync(id);
                if (existingFuelType == null)
                {
                    return NotFound(new { message = "Fuel type not found." });
                }

                existingFuelType.FuelName = fuelType.FuelName;
                existingFuelType.UserId = fuelType.UserId;
                existingFuelType.Modified = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Fuel type updated successfully." });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Failed to update fuel type due to database constraints." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update fuel type.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuelType(int id)
        {
            try
            {
                var fuelType = await _context.FuelTypes.FindAsync(id);
                if (fuelType == null)
                {
                    return NotFound(new { message = "Fuel type not found." });
                }

                try
                {
                    _context.FuelTypes.Remove(fuelType);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Fuel type deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete: This fuel type is linked to one or more vehicles." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete fuel type.", detail = ex.Message });
            }
        }
    }
}
