using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgreementController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly FluentValidation.IValidator<Agreement> _validator;

        public AgreementController(CarRentalContext context, FluentValidation.IValidator<Agreement> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAgreements()
        {
            try
            {
                var agreements = _context.Agreements.ToList();
                return Ok(agreements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load agreements.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgreementById(int id)
        {
            try
            {
                var agreement = _context.Agreements.Find(id);
                if (agreement == null)
                {
                    return NotFound(new { message = "Agreement not found." });
                }
                // Return bare object for simpler frontend deserialization
                return Ok(agreement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load agreement.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertAgreement([FromBody] Agreement agreement)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(agreement);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                agreement.Created = DateTime.UtcNow;
                agreement.Modified = DateTime.UtcNow;

                _context.Agreements.Add(agreement);

                try
                {
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Agreement created successfully.",
                        agreementId = agreement.AgreementId
                    });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Invalid foreign key. Make sure BookingID and UserID exist." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create agreement.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgreement(int id, [FromBody] Agreement agreement)
        {
            try
            {
                if (id != agreement.AgreementId)
                {
                    return BadRequest(new { message = "Agreement ID mismatch." });
                }

                var validationResult = await _validator.ValidateAsync(agreement);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                var existing = await _context.Agreements.FindAsync(id);
                if (existing == null)
                {
                    return NotFound(new { message = "Agreement not found." });
                }

                existing.BookingId = agreement.BookingId;
                existing.CustomerId = agreement.CustomerId;
                existing.TermsAccepted = agreement.TermsAccepted;
                existing.AgreementDate = agreement.AgreementDate;
                existing.AgreementPdfpath = agreement.AgreementPdfpath;
                existing.Modified = DateTime.UtcNow;

                _context.Agreements.Update(existing);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Agreement updated successfully." });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Failed to update agreement due to database constraints." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update agreement.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgreement(int id)
        {
            try
            {
                var agreement = await _context.Agreements.FindAsync(id);
                if (agreement == null)
                {
                    return NotFound(new { message = "Agreement not found." });
                }

                try
                {
                    _context.Agreements.Remove(agreement);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Agreement deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete agreement. It may be referenced elsewhere." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete agreement.", detail = ex.Message });
            }
        }

        [HttpGet("by-customer/{customerId}")]
        public IActionResult GetBookingsByCustomer(int customerId)
        {
            try
            {
                var bookings = _context.Bookings
                    .Where(b => b.CustomerId == customerId)
                    .Select(b => new
                    {
                        bookingId = b.BookingId,
                        vehicleId = b.VehicleId,
                        display = $"Booking #{b.BookingId} (Vehicle #{b.VehicleId})"
                    })
                    .ToList();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load bookings.", detail = ex.Message });
            }
        }

    }
}
