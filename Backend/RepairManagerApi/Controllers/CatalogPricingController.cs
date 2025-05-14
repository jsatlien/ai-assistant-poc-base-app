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
    [Route("api/catalog-pricing")]
    [ApiController]
    public class CatalogPricingController : ControllerBase
    {
        private readonly RepairManagerContext _context;

        public CatalogPricingController(RepairManagerContext context)
        {
            _context = context;
        }

        // GET: api/catalog-pricing
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogPricing>>> GetCatalogPricing()
        {
            return await _context.CatalogPricing
                .ToListAsync();
        }

        // GET: api/catalog-pricing/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogPricing>> GetCatalogPricing(int id)
        {
            var catalogPricing = await _context.CatalogPricing.FindAsync(id);

            if (catalogPricing == null)
            {
                return NotFound();
            }

            return catalogPricing;
        }

        // GET: api/catalog-pricing/item/part/5
        [HttpGet("item/{itemType}/{itemId}")]
        public async Task<ActionResult<CatalogPricing>> GetPricingForItem(string itemType, int itemId)
        {
            var now = DateTime.UtcNow;
            
            if (!Enum.TryParse<CatalogItemType>(itemType, true, out var catalogItemType))
            {
                return BadRequest("Invalid item type. Must be 'Part', 'Device', or 'Service'.");
            }
            
            var query = _context.CatalogPricing
                .Where(cp => cp.ItemType == catalogItemType &&
                             cp.EffectiveDate <= now &&
                             (!cp.ExpirationDate.HasValue || cp.ExpirationDate >= now));
                             
            // Filter based on the specific item type
            if (catalogItemType == CatalogItemType.Part)
            {
                query = query.Where(cp => cp.PartId == itemId);
            }
            else if (catalogItemType == CatalogItemType.Device)
            {
                query = query.Where(cp => cp.DeviceId == itemId);
            }
            else if (catalogItemType == CatalogItemType.Service)
            {
                query = query.Where(cp => cp.ServiceId == itemId);
            }
            
            var catalogPricing = await query
                .OrderByDescending(cp => cp.EffectiveDate)
                .FirstOrDefaultAsync();

            if (catalogPricing == null)
            {
                return NotFound();
            }

            return catalogPricing;
        }

        // POST: api/catalog-pricing
        [HttpPost]
        public async Task<ActionResult<CatalogPricing>> PostCatalogPricing(CatalogPricing catalogPricing)
        {
            // Validate that the item exists based on the item type
            if (catalogPricing.ItemType == CatalogItemType.Part)
            {
                if (!catalogPricing.PartId.HasValue)
                {
                    return BadRequest("PartId is required for Part item type.");
                }
                
                var part = await _context.Parts.FindAsync(catalogPricing.PartId);
                if (part == null)
                {
                    return BadRequest("The specified part does not exist.");
                }
            }
            else if (catalogPricing.ItemType == CatalogItemType.Device)
            {
                if (!catalogPricing.DeviceId.HasValue)
                {
                    return BadRequest("DeviceId is required for Device item type.");
                }
                
                var device = await _context.Devices.FindAsync(catalogPricing.DeviceId);
                if (device == null)
                {
                    return BadRequest("The specified device does not exist.");
                }
            }
            else if (catalogPricing.ItemType == CatalogItemType.Service)
            {
                if (!catalogPricing.ServiceId.HasValue)
                {
                    return BadRequest("ServiceId is required for Service item type.");
                }
                
                var service = await _context.Services.FindAsync(catalogPricing.ServiceId);
                if (service == null)
                {
                    return BadRequest("The specified service does not exist.");
                }
            }
            else
            {
                return BadRequest("Invalid item type. Must be Part, Device, or Service.");
            }

            _context.CatalogPricing.Add(catalogPricing);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCatalogPricing), new { id = catalogPricing.Id }, catalogPricing);
        }

        // PUT: api/catalog-pricing/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatalogPricing(int id, CatalogPricing catalogPricing)
        {
            if (id != catalogPricing.Id)
            {
                return BadRequest();
            }

            // Validate that the item exists based on the item type
            if (catalogPricing.ItemType == CatalogItemType.Part)
            {
                if (!catalogPricing.PartId.HasValue)
                {
                    return BadRequest("PartId is required for Part item type.");
                }
                
                var part = await _context.Parts.FindAsync(catalogPricing.PartId);
                if (part == null)
                {
                    return BadRequest("The specified part does not exist.");
                }
            }
            else if (catalogPricing.ItemType == CatalogItemType.Device)
            {
                if (!catalogPricing.DeviceId.HasValue)
                {
                    return BadRequest("DeviceId is required for Device item type.");
                }
                
                var device = await _context.Devices.FindAsync(catalogPricing.DeviceId);
                if (device == null)
                {
                    return BadRequest("The specified device does not exist.");
                }
            }
            else if (catalogPricing.ItemType == CatalogItemType.Service)
            {
                if (!catalogPricing.ServiceId.HasValue)
                {
                    return BadRequest("ServiceId is required for Service item type.");
                }
                
                var service = await _context.Services.FindAsync(catalogPricing.ServiceId);
                if (service == null)
                {
                    return BadRequest("The specified service does not exist.");
                }
            }
            else
            {
                return BadRequest("Invalid item type. Must be Part, Device, or Service.");
            }

            _context.Entry(catalogPricing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogPricingExists(id))
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

        // DELETE: api/catalog-pricing/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogPricing(int id)
        {
            var catalogPricing = await _context.CatalogPricing.FindAsync(id);
            if (catalogPricing == null)
            {
                return NotFound();
            }

            _context.CatalogPricing.Remove(catalogPricing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CatalogPricingExists(int id)
        {
            return _context.CatalogPricing.Any(e => e.Id == id);
        }
    }
}
