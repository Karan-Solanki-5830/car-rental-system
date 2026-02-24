using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceLogController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly FluentValidation.IValidator<MaintenanceLog> _validator;

        public MaintenanceLogController(CarRentalContext context, FluentValidation.IValidator<MaintenanceLog> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _context.MaintenanceLogs.ToListAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load maintenance logs.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogById(int id)
        {
            try
            {
                var log = await _context.MaintenanceLogs.FindAsync(id);
                if (log == null)
                {
                    return NotFound(new { message = "Maintenance log not found." });
                }

                return Ok(log);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load maintenance log.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLog([FromBody] MaintenanceLog log)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(log);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                log.Created = DateTime.Now;
                log.Modified = DateTime.Now;

                _context.MaintenanceLogs.Add(log);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Maintenance log added successfully.", logId = log.Id });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to add maintenance log due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to add maintenance log.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLog(int id, [FromBody] MaintenanceLog log)
        {
            try
            {
                if (id != log.Id)
                {
                    return BadRequest(new { message = "Maintenance log ID mismatch." });
                }

                var validationResult = await _validator.ValidateAsync(log);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                var existing = await _context.MaintenanceLogs.FindAsync(id);
                if (existing == null)
                {
                    return NotFound(new { message = "Maintenance log not found." });
                }

                existing.VehicleId = log.VehicleId;
                existing.UserId = log.UserId;
                existing.Description = log.Description;
                existing.ServiceDate = log.ServiceDate;
                existing.Cost = log.Cost;
                existing.Modified = DateTime.Now;

                _context.MaintenanceLogs.Update(existing);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Maintenance log updated successfully." });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Failed to update maintenance log due to database constraints." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update maintenance log.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            try
            {
                var log = await _context.MaintenanceLogs.FindAsync(id);
                if (log == null)
                {
                    return NotFound(new { message = "Maintenance log not found." });
                }

                try
                {
                    _context.MaintenanceLogs.Remove(log);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Maintenance log deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete: This log is linked to other records." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete maintenance log.", detail = ex.Message });
            }
        }
    }
}
