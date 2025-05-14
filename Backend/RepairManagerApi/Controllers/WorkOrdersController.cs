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
    [Route("api/work-orders")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly RepairManagerContext _context;
        private static int _lastWorkOrderNumber = 0;

        public WorkOrdersController(RepairManagerContext context)
        {
            _context = context;
        }

        // GET: api/work-orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrders(int? groupId = null)
        {
            var query = _context.WorkOrders
                .Include(w => w.Device)
                .Include(w => w.Service)
                .Include(w => w.RepairProgram)
                .Include(w => w.Group)
                .AsQueryable();

            if (groupId.HasValue)
            {
                query = query.Where(w => w.GroupId == groupId.Value);
            }

            var workOrders = await query.ToListAsync();
            
            // Populate the non-mapped properties for UI display
            foreach (var workOrder in workOrders)
            {
                workOrder.DeviceName = workOrder.Device?.Name;
                workOrder.ServiceName = workOrder.Service?.Name;
            }

            return workOrders;
        }

        // GET: api/work-orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrder>> GetWorkOrder(int id)
        {
            var workOrder = await _context.WorkOrders
                .Include(w => w.Device)
                .Include(w => w.Service)
                .Include(w => w.RepairProgram)
                .Include(w => w.Group)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workOrder == null)
            {
                return NotFound();
            }

            // Populate the non-mapped properties for UI display
            workOrder.DeviceName = workOrder.Device?.Name;
            workOrder.ServiceName = workOrder.Service?.Name;

            return workOrder;
        }

        // POST: api/work-orders
        [HttpPost]
        public async Task<ActionResult<WorkOrder>> PostWorkOrder(WorkOrder workOrder)
        {
            // Generate a unique work order code
            if (string.IsNullOrEmpty(workOrder.Code))
            {
                // Find the highest work order number in the database
                if (_lastWorkOrderNumber == 0)
                {
                    var lastWorkOrder = await _context.WorkOrders
                        .OrderByDescending(w => w.Id)
                        .FirstOrDefaultAsync();
                    
                    if (lastWorkOrder != null && !string.IsNullOrEmpty(lastWorkOrder.Code) && 
                        lastWorkOrder.Code.StartsWith("WO") && lastWorkOrder.Code.Length > 2)
                    {
                        if (int.TryParse(lastWorkOrder.Code.Substring(2), out int lastNumber))
                        {
                            _lastWorkOrderNumber = lastNumber;
                        }
                    }
                }
                
                _lastWorkOrderNumber++;
                workOrder.Code = $"WO{_lastWorkOrderNumber:D5}";
            }
            
            // Set creation timestamp
            workOrder.CreatedAt = DateTime.UtcNow;
            
            // Validate that the group exists
            var group = await _context.Groups.FindAsync(workOrder.GroupId);
            if (group == null)
            {
                return BadRequest("The specified group does not exist.");
            }
            
            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync();

            // Load related entities for the response
            await _context.Entry(workOrder).Reference(w => w.Device).LoadAsync();
            await _context.Entry(workOrder).Reference(w => w.Service).LoadAsync();
            await _context.Entry(workOrder).Reference(w => w.RepairProgram).LoadAsync();
            await _context.Entry(workOrder).Reference(w => w.Group).LoadAsync();
            
            // Populate the non-mapped properties for UI display
            workOrder.DeviceName = workOrder.Device?.Name;
            workOrder.ServiceName = workOrder.Service?.Name;

            return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrder.Id }, workOrder);
        }

        // PUT: api/work-orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkOrder(int id, WorkOrder workOrder)
        {
            if (id != workOrder.Id)
            {
                return BadRequest();
            }

            // Validate that the group exists
            var group = await _context.Groups.FindAsync(workOrder.GroupId);
            if (group == null)
            {
                return BadRequest("The specified group does not exist.");
            }

            // Set update timestamp
            workOrder.UpdatedAt = DateTime.UtcNow;

            // Ensure the code is not changed
            var existingWorkOrder = await _context.WorkOrders.FindAsync(id);
            if (existingWorkOrder == null)
            {
                return NotFound();
            }
            
            // Keep the original code
            workOrder.Code = existingWorkOrder.Code;

            _context.Entry(existingWorkOrder).State = EntityState.Detached;
            _context.Entry(workOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkOrderExists(id))
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

        // PATCH: api/work-orders/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateWorkOrderStatus(int id, [FromBody] StatusUpdateModel statusUpdate)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            
            if (workOrder == null)
            {
                return NotFound();
            }

            workOrder.CurrentStatus = statusUpdate.Status;
            workOrder.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                
                // Load related entities for the response
                await _context.Entry(workOrder).Reference(w => w.Device).LoadAsync();
                await _context.Entry(workOrder).Reference(w => w.Service).LoadAsync();
                
                // Populate the non-mapped properties for UI display
                workOrder.DeviceName = workOrder.Device?.Name;
                workOrder.ServiceName = workOrder.Service?.Name;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(workOrder);
        }

        // DELETE: api/work-orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkOrder(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }

            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrders.Any(e => e.Id == id);
        }
    }

    public class StatusUpdateModel
    {
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
