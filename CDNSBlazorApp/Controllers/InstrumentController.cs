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
    public class InstrumentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstrumentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instrument>>> GetInstruments()
        {
            return await _context.Instruments
                .Include(i => i.Instituation)
                .Include(i => i.Series)
                .Include(i => i.SeriesPatterns)
                .Include(i => i.Subscriptions)
                .Include(i => i.Payments)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Instrument>> GetInstrument(string id)
        {
            var instrument = await _context.Instruments
                .Include(i => i.Instituation)
                .Include(i => i.Series)
                .Include(i => i.SeriesPatterns)
                .Include(i => i.Subscriptions)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.InstrumentId == id);

            if (instrument == null)
                return NotFound();

            return instrument;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Instrument>> CreateInstrument(Instrument instrument)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Calculate maturity period if dates are provided
            if (instrument.DSign.HasValue && instrument.DFinalMaturity.HasValue)
            {
                var years = (instrument.DFinalMaturity.Value - instrument.DSign.Value).Days / 365;
                instrument.MaturityPeriod = years.ToString();
            }

            // Set AmtNet same as Amt if not provided
            if (instrument.AmtNet == 0)
                instrument.AmtNet = instrument.Amt;

            _context.Instruments.Add(instrument);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstrument), new { id = instrument.InstrumentId }, instrument);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateInstrument(string id, Instrument instrument)
        {
            if (id != instrument.InstrumentId)
                return BadRequest();

            // Calculate maturity period if dates are provided
            if (instrument.DSign.HasValue && instrument.DFinalMaturity.HasValue)
            {
                var years = (instrument.DFinalMaturity.Value - instrument.DSign.Value).Days / 365;
                instrument.MaturityPeriod = years.ToString();
            }

            _context.Entry(instrument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstrumentExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInstrument(string id)
        {
            var instrument = await _context.Instruments.FindAsync(id);
            if (instrument == null)
                return NotFound();

            _context.Instruments.Remove(instrument);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InstrumentExists(string id)
        {
            return _context.Instruments.Any(e => e.InstrumentId == id);
        }
    }
}
