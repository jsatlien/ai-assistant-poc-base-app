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
    public class WorkflowsController : ControllerBase
    {
        private readonly RepairManagerContext _context;

        public WorkflowsController(RepairManagerContext context)
        {
            _context = context;
        }

        // GET: api/Workflows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepairWorkflow>>> GetWorkflows()
        {
            return await _context.RepairWorkflows.ToListAsync();
        }

        // GET: api/Workflows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RepairWorkflow>> GetWorkflow(int id)
        {
            var workflow = await _context.RepairWorkflows.FindAsync(id);

            if (workflow == null)
            {
                return NotFound();
            }

            return workflow;
        }

        // POST: api/Workflows
        [HttpPost]
        public async Task<ActionResult<RepairWorkflow>> PostWorkflow(RepairWorkflow workflow)
        {
            _context.RepairWorkflows.Add(workflow);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWorkflow), new { id = workflow.Id }, workflow);
        }

        // PUT: api/Workflows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkflow(int id, RepairWorkflow workflow)
        {
            if (id != workflow.Id)
            {
                return BadRequest();
            }

            _context.Entry(workflow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkflowExists(id))
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

        // DELETE: api/Workflows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflow(int id)
        {
            var workflow = await _context.RepairWorkflows.FindAsync(id);
            if (workflow == null)
            {
                return NotFound();
            }

            // Check if this workflow is used by any repair programs
            bool isUsedByPrograms = await _context.RepairPrograms.AnyAsync(p => p.RepairWorkflowId == id);
            if (isUsedByPrograms)
            {
                return BadRequest("Cannot delete workflow because it is used by one or more repair programs.");
            }

            _context.RepairWorkflows.Remove(workflow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkflowExists(int id)
        {
            return _context.RepairWorkflows.Any(e => e.Id == id);
        }
    }
}
