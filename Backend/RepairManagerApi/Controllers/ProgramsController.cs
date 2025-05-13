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
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramsController : ControllerBase
    {
        private readonly RepairManagerContext _context;

        public ProgramsController(RepairManagerContext context)
        {
            _context = context;
        }

        // GET: api/Programs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepairProgram>>> GetPrograms()
        {
            return await _context.RepairPrograms
                .Include(p => p.RepairWorkflow)
                .ToListAsync();
        }

        // GET: api/Programs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RepairProgram>> GetProgram(int id)
        {
            var program = await _context.RepairPrograms
                .Include(p => p.RepairWorkflow)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
            {
                return NotFound();
            }

            return program;
        }

        // POST: api/Programs
        [HttpPost]
        public async Task<ActionResult<RepairProgram>> PostProgram(RepairProgram program)
        {
            // Verify that the workflow exists
            bool workflowExists = await _context.RepairWorkflows.AnyAsync(w => w.Id == program.RepairWorkflowId);
            if (!workflowExists)
            {
                return BadRequest("The specified repair workflow does not exist.");
            }

            _context.RepairPrograms.Add(program);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, program);
        }

        // PUT: api/Programs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProgram(int id, RepairProgram program)
        {
            if (id != program.Id)
            {
                return BadRequest();
            }

            // Verify that the workflow exists
            bool workflowExists = await _context.RepairWorkflows.AnyAsync(w => w.Id == program.RepairWorkflowId);
            if (!workflowExists)
            {
                return BadRequest("The specified repair workflow does not exist.");
            }

            _context.Entry(program).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgramExists(id))
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

        // DELETE: api/Programs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var program = await _context.RepairPrograms.FindAsync(id);
            if (program == null)
            {
                return NotFound();
            }

            // Check if this program is used by any work orders
            bool isUsedByWorkOrders = await _context.WorkOrders.AnyAsync(w => w.RepairProgramId == id);
            if (isUsedByWorkOrders)
            {
                return BadRequest("Cannot delete program because it is used by one or more work orders.");
            }

            _context.RepairPrograms.Remove(program);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProgramExists(int id)
        {
            return _context.RepairPrograms.Any(e => e.Id == id);
        }
    }
}
