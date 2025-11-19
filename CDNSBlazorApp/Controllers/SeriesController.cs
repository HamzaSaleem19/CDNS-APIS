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
    public class SeriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Series>>> GetSeries()
        {
            return await _context.Series
                .Include(s => s.Instrument)
                .ToListAsync();
        }

        [HttpGet("{serieId}/{traNo}")]
        public async Task<ActionResult<Series>> GetSeries(string serieId, int traNo)
        {
            var series = await _context.Series
                .Include(s => s.Instrument)
                .FirstOrDefaultAsync(s => s.SerieId == serieId && s.SerieTraNo == traNo);

            if (series == null)
                return NotFound();

            return series;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Series>> CreateSeries(Series series)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Series.Add(series);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSeries),
                new { serieId = series.SerieId, traNo = series.SerieTraNo }, series);
        }

        [HttpPut("{serieId}/{traNo}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateSeries(string serieId, int traNo, Series series)
        {
            if (serieId != series.SerieId || traNo != series.SerieTraNo)
                return BadRequest();

            _context.Entry(series).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeriesExists(serieId, traNo))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{serieId}/{traNo}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSeries(string serieId, int traNo)
        {
            var series = await _context.Series.FindAsync(serieId, traNo);
            if (series == null)
                return NotFound();

            _context.Series.Remove(series);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SeriesExists(string serieId, int traNo)
        {
            return _context.Series.Any(e => e.SerieId == serieId && e.SerieTraNo == traNo);
        }
    }
}
