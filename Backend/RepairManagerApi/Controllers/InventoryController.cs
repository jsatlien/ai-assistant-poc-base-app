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
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly RepairManagerContext _context;

        public InventoryController(RepairManagerContext context)
        {
            _context = context;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems()
        {
            var inventoryItems = await _context.InventoryItems
                .Include(i => i.Group)
                .ToListAsync();

            // Populate the CatalogItemName property based on the item type
            foreach (var item in inventoryItems)
            {
                if (item.CatalogItemType == CatalogItemType.Part && item.PartId.HasValue)
                {
                    var part = await _context.Parts.FindAsync(item.PartId);
                    if (part != null)
                    {
                        item.CatalogItemName = part.Name;
                    }
                }
                else if (item.CatalogItemType == CatalogItemType.Device && item.DeviceId.HasValue)
                {
                    var device = await _context.Devices.FindAsync(item.DeviceId);
                    if (device != null)
                    {
                        item.CatalogItemName = device.Name;
                    }
                }
                else if (item.CatalogItemType == CatalogItemType.Service && item.ServiceId.HasValue)
                {
                    var service = await _context.Services.FindAsync(item.ServiceId);
                    if (service != null)
                    {
                        item.CatalogItemName = service.Name;
                    }
                }
            }

            return inventoryItems;
        }

        // GET: api/inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
        {
            var inventoryItem = await _context.InventoryItems
                .Include(i => i.Group)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            // Populate the CatalogItemName property based on the item type
            if (inventoryItem.CatalogItemType == CatalogItemType.Part && inventoryItem.PartId.HasValue)
            {
                var part = await _context.Parts.FindAsync(inventoryItem.PartId);
                if (part != null)
                {
                    inventoryItem.CatalogItemName = part.Name;
                }
            }
            else if (inventoryItem.CatalogItemType == CatalogItemType.Device && inventoryItem.DeviceId.HasValue)
            {
                var device = await _context.Devices.FindAsync(inventoryItem.DeviceId);
                if (device != null)
                {
                    inventoryItem.CatalogItemName = device.Name;
                }
            }
            else if (inventoryItem.CatalogItemType == CatalogItemType.Service && inventoryItem.ServiceId.HasValue)
            {
                var service = await _context.Services.FindAsync(inventoryItem.ServiceId);
                if (service != null)
                {
                    inventoryItem.CatalogItemName = service.Name;
                }
            }

            return inventoryItem;
        }

        // GET: api/inventory/group/5
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItemsByGroup(int groupId)
        {
            var inventoryItems = await _context.InventoryItems
                .Include(i => i.Group)
                .Where(i => i.GroupId == groupId)
                .ToListAsync();

            // Populate the CatalogItemName property based on the item type
            foreach (var item in inventoryItems)
            {
                if (item.CatalogItemType == CatalogItemType.Part && item.PartId.HasValue)
                {
                    var part = await _context.Parts.FindAsync(item.PartId);
                    if (part != null)
                    {
                        item.CatalogItemName = part.Name;
                    }
                }
                else if (item.CatalogItemType == CatalogItemType.Device && item.DeviceId.HasValue)
                {
                    var device = await _context.Devices.FindAsync(item.DeviceId);
                    if (device != null)
                    {
                        item.CatalogItemName = device.Name;
                    }
                }
                else if (item.CatalogItemType == CatalogItemType.Service && item.ServiceId.HasValue)
                {
                    var service = await _context.Services.FindAsync(item.ServiceId);
                    if (service != null)
                    {
                        item.CatalogItemName = service.Name;
                    }
                }
            }

            return inventoryItems;
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<ActionResult<InventoryItem>> PostInventoryItem(InventoryItem inventoryItem)
        {
            // Validate that the catalog item exists based on the item type
            if (inventoryItem.CatalogItemType == CatalogItemType.Part)
            {
                if (!inventoryItem.PartId.HasValue)
                {
                    return BadRequest("PartId is required for Part item type.");
                }
                
                var part = await _context.Parts.FindAsync(inventoryItem.PartId);
                if (part == null)
                {
                    return BadRequest("The specified part does not exist.");
                }
            }
            else if (inventoryItem.CatalogItemType == CatalogItemType.Device)
            {
                if (!inventoryItem.DeviceId.HasValue)
                {
                    return BadRequest("DeviceId is required for Device item type.");
                }
                
                var device = await _context.Devices.FindAsync(inventoryItem.DeviceId);
                if (device == null)
                {
                    return BadRequest("The specified device does not exist.");
                }
            }
            else if (inventoryItem.CatalogItemType == CatalogItemType.Service)
            {
                if (!inventoryItem.ServiceId.HasValue)
                {
                    return BadRequest("ServiceId is required for Service item type.");
                }
                
                var service = await _context.Services.FindAsync(inventoryItem.ServiceId);
                if (service == null)
                {
                    return BadRequest("The specified service does not exist.");
                }
            }
            else
            {
                return BadRequest("Invalid catalog item type. Must be Part, Device, or Service.");
            }

            // Validate that the group exists
            var group = await _context.Groups.FindAsync(inventoryItem.GroupId);
            if (group == null)
            {
                return BadRequest("The specified group does not exist.");
            }

            inventoryItem.LastUpdated = DateTime.UtcNow;
            _context.InventoryItems.Add(inventoryItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventoryItem), new { id = inventoryItem.Id }, inventoryItem);
        }

        // PUT: api/inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItem(int id, InventoryItem inventoryItem)
        {
            if (id != inventoryItem.Id)
            {
                return BadRequest();
            }

            // Validate that the catalog item exists based on the item type
            if (inventoryItem.CatalogItemType == CatalogItemType.Part)
            {
                if (!inventoryItem.PartId.HasValue)
                {
                    return BadRequest("PartId is required for Part item type.");
                }
                
                var part = await _context.Parts.FindAsync(inventoryItem.PartId);
                if (part == null)
                {
                    return BadRequest("The specified part does not exist.");
                }
            }
            else if (inventoryItem.CatalogItemType == CatalogItemType.Device)
            {
                if (!inventoryItem.DeviceId.HasValue)
                {
                    return BadRequest("DeviceId is required for Device item type.");
                }
                
                var device = await _context.Devices.FindAsync(inventoryItem.DeviceId);
                if (device == null)
                {
                    return BadRequest("The specified device does not exist.");
                }
            }
            else if (inventoryItem.CatalogItemType == CatalogItemType.Service)
            {
                if (!inventoryItem.ServiceId.HasValue)
                {
                    return BadRequest("ServiceId is required for Service item type.");
                }
                
                var service = await _context.Services.FindAsync(inventoryItem.ServiceId);
                if (service == null)
                {
                    return BadRequest("The specified service does not exist.");
                }
            }
            else
            {
                return BadRequest("Invalid catalog item type. Must be Part, Device, or Service.");
            }

            // Validate that the group exists
            var group = await _context.Groups.FindAsync(inventoryItem.GroupId);
            if (group == null)
            {
                return BadRequest("The specified group does not exist.");
            }

            inventoryItem.LastUpdated = DateTime.UtcNow;
            _context.Entry(inventoryItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryItemExists(id))
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

        // DELETE: api/inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return NotFound();
            }

            _context.InventoryItems.Remove(inventoryItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventoryItemExists(int id)
        {
            return _context.InventoryItems.Any(e => e.Id == id);
        }
    }
}
