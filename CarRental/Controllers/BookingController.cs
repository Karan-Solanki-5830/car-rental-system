using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly FluentValidation.IValidator<Booking> _validator;

        public BookingController(CarRentalContext context, FluentValidation.IValidator<Booking> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _context.Bookings.ToListAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load bookings.", detail = ex.Message });
            }
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentBookings()
        {
            try
            {
                var bookings = await _context.Bookings
                    .OrderByDescending(b => b.Created)
                    .Take(5)
                    .ToListAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load recent bookings.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load booking.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertBooking([FromBody] Booking booking)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(booking);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                booking.Created = DateTime.Now;
                booking.Modified = DateTime.Now;

                _context.Bookings.Add(booking);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Booking created successfully.", bookingId = booking.BookingId });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to create booking due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create booking.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking booking)
        {
            try
            {
                if (id != booking.BookingId)
                {
                    return BadRequest(new { message = "Booking ID mismatch." });
                }

                var validationResult = await _validator.ValidateAsync(booking);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                var existing = await _context.Bookings.FindAsync(id);
                if (existing == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }

                existing.CustomerId = booking.CustomerId;
                existing.VehicleId = booking.VehicleId;
                existing.StartDateTime = booking.StartDateTime;
                existing.EndDateTime = booking.EndDateTime;
                existing.Status = booking.Status;
                existing.Modified = DateTime.Now;

                _context.Bookings.Update(existing);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Booking updated successfully." });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Failed to update booking due to database constraints." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update booking.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }

                try
                {
                    _context.Bookings.Remove(booking);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Booking deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete booking. It may be referenced elsewhere." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete booking.", detail = ex.Message });
            }
        }

        [HttpGet("dropdown")]
        public IActionResult GetBookingDropdown()
        {
            try
            {
                var bookings = _context.Bookings
                    .Select(b => new
                    {
                        bookingId = b.BookingId,
                        vehicleId = b.VehicleId,
                        display = $"Booking #{b.BookingId} | Vehicle #{b.VehicleId}"
                    })
                    .ToList();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load booking dropdown.", detail = ex.Message });
            }
        }


        [HttpGet("count")]
        public async Task<IActionResult> GetBookingCount()
        {
            try
            {
                var count = await _context.Bookings.CountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load booking count.", detail = ex.Message });
            }
        }
    }
}
