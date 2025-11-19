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
    public class SubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            return await _context.Subscriptions
                .Include(s => s.Instrument)
                .ToListAsync();
        }

        [HttpGet("{subscriptionId}/{traNo}")]
        public async Task<ActionResult<Subscription>> GetSubscription(string subscriptionId, int traNo)
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.Instrument)
                .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.SubscriptionIdTraNo == traNo);

            if (subscription == null)
                return NotFound();

            return subscription;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<Subscription>> CreateSubscription(Subscription subscription)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubscription),
                new { subscriptionId = subscription.SubscriptionId, traNo = subscription.SubscriptionIdTraNo }, subscription);
        }

        [HttpPut("{subscriptionId}/{traNo}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateSubscription(string subscriptionId, int traNo, Subscription subscription)
        {
            if (subscriptionId != subscription.SubscriptionId || traNo != subscription.SubscriptionIdTraNo)
                return BadRequest();

            _context.Entry(subscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(subscriptionId, traNo))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{subscriptionId}/{traNo}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubscription(string subscriptionId, int traNo)
        {
            var subscription = await _context.Subscriptions.FindAsync(subscriptionId, traNo);
            if (subscription == null)
                return NotFound();

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionExists(string subscriptionId, int traNo)
        {
            return _context.Subscriptions.Any(e => e.SubscriptionId == subscriptionId && e.SubscriptionIdTraNo == traNo);
        }
    }
}
