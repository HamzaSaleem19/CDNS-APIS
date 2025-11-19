using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDNSBlazorApp.Data;
using CDNSBlazorApp.Models;

namespace CDNSBlazorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments
                .Include(p => p.Instrument)
                .ToListAsync();
        }

        [HttpGet("{paymentId}/{traNo}")]
        public async Task<ActionResult<Payment>> GetPayment(string paymentId, int traNo)
        {
            var payment = await _context.Payments
                .Include(p => p.Instrument)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId && p.PaymentTraNo == traNo);

            if (payment == null)
                return NotFound();

            return payment;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<Payment>> CreatePayment(Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment),
                new { paymentId = payment.PaymentId, traNo = payment.PaymentTraNo }, payment);
        }

        [HttpPut("{paymentId}/{traNo}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdatePayment(string paymentId, int traNo, Payment payment)
        {
            if (paymentId != payment.PaymentId || traNo != payment.PaymentTraNo)
                return BadRequest();

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(paymentId, traNo))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{paymentId}/{traNo}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayment(string paymentId, int traNo)
        {
            var payment = await _context.Payments.FindAsync(paymentId, traNo);
            if (payment == null)
                return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(string paymentId, int traNo)
        {
            return _context.Payments.Any(e => e.PaymentId == paymentId && e.PaymentTraNo == traNo);
        }
    }
}
