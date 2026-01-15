using CarRental.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly IValidator<Vehicle> _validator;

        public VehicleController(CarRentalContext context, IValidator<Vehicle> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> GetAllVehicles()
        {
            try
            {
                var vehicles = await _context.Vehicles.ToListAsync();
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load vehicles.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(id);
                if (vehicle == null)
                    return NotFound(new { message = "Vehicle not found." });

                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load vehicle.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] Vehicle vehicle)
        {
            try
            {
                var validation = await _validator.ValidateAsync(vehicle);
                if (!validation.IsValid)
                    return BadRequest(validation.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));

                vehicle.Created = DateTime.Now;
                vehicle.Modified = DateTime.Now;

                _context.Vehicles.Add(vehicle);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Vehicle created successfully.", vehicleId = vehicle.VehicleId });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to create vehicle due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create vehicle.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] Vehicle vehicle)
        {
            try
            {
                if (id != vehicle.VehicleId)
                    return BadRequest(new { message = "Vehicle ID mismatch." });

                var existing = await _context.Vehicles.FindAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Vehicle not found." });

                var validation = await _validator.ValidateAsync(vehicle);
                if (!validation.IsValid)
                    return BadRequest(validation.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));

                // Update vehicle fields
                existing.UserId = vehicle.UserId;
                existing.Brand = vehicle.Brand;
                existing.Model = vehicle.Model;
                existing.Year = vehicle.Year;
                existing.PlateNumber = vehicle.PlateNumber;
                existing.FuelTypeId = vehicle.FuelTypeId;
                existing.VehicleTypeId = vehicle.VehicleTypeId;
                existing.Status = vehicle.Status;
                existing.Mileage = vehicle.Mileage;
                existing.ConditionNote = vehicle.ConditionNote;
                existing.PricePerHour = vehicle.PricePerHour;
                existing.PricePerDay = vehicle.PricePerDay;
                existing.Modified = DateTime.Now;

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Vehicle updated successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to update vehicle due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update vehicle.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(id);
                if (vehicle == null)
                    return NotFound(new { message = "Vehicle not found." });

                try
                {
                    _context.Vehicles.Remove(vehicle);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Vehicle deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete: Vehicle is linked to other records." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete vehicle.", detail = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetVehicleCount()
        {
            try
            {
                var count = await _context.Vehicles.CountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load vehicle count.", detail = ex.Message });
            }
        }
    }
}
