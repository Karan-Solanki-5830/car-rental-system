using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly FluentValidation.IValidator<Payment> _validator;

        public PaymentController(CarRentalContext context, FluentValidation.IValidator<Payment> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _context.Payments.ToListAsync();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load payments.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found." });
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load payment.", detail = ex.Message });
            }
        }

        [HttpPost]

        public async Task<IActionResult> AddPayment([FromBody] Payment payment)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(payment);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                payment.Created = DateTime.UtcNow;
                payment.Modified = DateTime.UtcNow;

                _context.Payments.Add(payment);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Payment added successfully.", paymentId = payment.PaymentId });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Failed to add payment due to database constraints." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to add payment.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
        {
            try
            {
                if (id != payment.PaymentId)
                {
                    return BadRequest(new { message = "Payment ID mismatch." });
                }

                var validationResult = await _validator.ValidateAsync(payment);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

                var existing = await _context.Payments.FindAsync(id);
                if (existing == null)
                {
                    return NotFound(new { message = "Payment not found." });
                }

                existing.BookingId = payment.BookingId;
                existing.UserId = payment.UserId;
                existing.Amount = payment.Amount;
                existing.PaymentDate = payment.PaymentDate;
                existing.PaymentMethod = payment.PaymentMethod;
                existing.Remarks = payment.Remarks;
                existing.Modified = DateTime.UtcNow;

                _context.Payments.Update(existing);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment updated successfully." });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Failed to update payment due to database constraints." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update payment.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found." });
                }

                try
                {
                    _context.Payments.Remove(payment);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Payment deleted successfully." });
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new { message = "Cannot delete: This payment is linked to other records." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete payment.", detail = ex.Message });
            }
        }

    }

}
