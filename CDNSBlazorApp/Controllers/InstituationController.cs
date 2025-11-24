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
    public class InstituationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstituationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instituation>>> GetInstituations()
        {
            return await _context.Instituations
                //.Include(i => i.Instruments)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Instituation>> GetInstituation(string id)
        {
            var instituation = await _context.Instituations
                .Include(i => i.Instruments)
                .FirstOrDefaultAsync(i => i.InstituationId == id);

            if (instituation == null)
                return NotFound();

            return instituation;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Instituation>> CreateInstituation(Instituation instituation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Instituations.Add(instituation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstituation), new { id = instituation.InstituationId }, instituation);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateInstituation(string id, Instituation instituation)
        {
            if (id != instituation.InstituationId)
                return BadRequest();

            _context.Entry(instituation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstituationExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInstituation(string id)
        {
            var instituation = await _context.Instituations.FindAsync(id);
            if (instituation == null)
                return NotFound();

            _context.Instituations.Remove(instituation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InstituationExists(string id)
        {
            return _context.Instituations.Any(e => e.InstituationId == id);
        }
    }
}
