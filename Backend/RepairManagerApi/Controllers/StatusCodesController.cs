using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairManagerApi.Data;
using RepairManagerApi.Models;

namespace RepairManagerApi.Controllers
{
    [Route("api/status-codes")]
    [ApiController]
    public class StatusCodesController : ControllerBase
    {
        private readonly RepairManagerContext _context;

        public StatusCodesController(RepairManagerContext context)
        {
            _context = context;
        }

        // GET: api/status-codes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusCode>>> GetStatusCodes()
        {
            return await _context.StatusCodes
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }

        // GET: api/status-codes/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<StatusCode>>> GetActiveStatusCodes()
        {
            return await _context.StatusCodes
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }

        // GET: api/status-codes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusCode>> GetStatusCode(int id)
        {
            var statusCode = await _context.StatusCodes.FindAsync(id);

            if (statusCode == null)
            {
                return NotFound();
            }

            return statusCode;
        }

        // POST: api/status-codes
        [HttpPost]
        public async Task<ActionResult<StatusCode>> PostStatusCode(StatusCode statusCode)
        {
            _context.StatusCodes.Add(statusCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStatusCode), new { id = statusCode.Id }, statusCode);
        }

        // PUT: api/status-codes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStatusCode(int id, StatusCode statusCode)
        {
            if (id != statusCode.Id)
            {
                return BadRequest();
            }

            _context.Entry(statusCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusCodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/status-codes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatusCode(int id)
        {
            var statusCode = await _context.StatusCodes.FindAsync(id);
            if (statusCode == null)
            {
                return NotFound();
            }

            _context.StatusCodes.Remove(statusCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StatusCodeExists(int id)
        {
            return _context.StatusCodes.Any(e => e.Id == id);
        }
    }
}
