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
                .Where(cp => cp.IsActive)
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
            
            var catalogPricing = await _context.CatalogPricing
                .Where(cp => cp.ItemType.ToLower() == itemType.ToLower() && 
                             cp.ItemId == itemId &&
                             cp.IsActive &&
                             cp.EffectiveDate <= now &&
                             (!cp.ExpirationDate.HasValue || cp.ExpirationDate >= now))
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
            // Validate that the item exists
            if (catalogPricing.ItemType.ToLower() == "part")
            {
                var part = await _context.Parts.FindAsync(catalogPricing.ItemId);
                if (part == null)
                {
                    return BadRequest("The specified part does not exist.");
                }
            }
            else if (catalogPricing.ItemType.ToLower() == "device")
            {
                var device = await _context.Devices.FindAsync(catalogPricing.ItemId);
                if (device == null)
                {
                    return BadRequest("The specified device does not exist.");
                }
            }
            else if (catalogPricing.ItemType.ToLower() == "service")
            {
                var service = await _context.Services.FindAsync(catalogPricing.ItemId);
                if (service == null)
                {
                    return BadRequest("The specified service does not exist.");
                }
            }
            else
            {
                return BadRequest("Invalid item type. Must be 'Part', 'Device', or 'Service'.");
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

            // Validate that the item exists
            if (catalogPricing.ItemType.ToLower() == "part")
            {
                var part = await _context.Parts.FindAsync(catalogPricing.ItemId);
                if (part == null)
                {
                    return BadRequest("The specified part does not exist.");
                }
            }
            else if (catalogPricing.ItemType.ToLower() == "device")
            {
                var device = await _context.Devices.FindAsync(catalogPricing.ItemId);
                if (device == null)
                {
                    return BadRequest("The specified device does not exist.");
                }
            }
            else if (catalogPricing.ItemType.ToLower() == "service")
            {
                var service = await _context.Services.FindAsync(catalogPricing.ItemId);
                if (service == null)
                {
                    return BadRequest("The specified service does not exist.");
                }
            }
            else
            {
                return BadRequest("Invalid item type. Must be 'Part', 'Device', or 'Service'.");
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
