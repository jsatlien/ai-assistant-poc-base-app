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
    [Route("api/service-categories")]
    [ApiController]
    public class ServiceCategoriesController : ControllerBase
    {
        private readonly RepairManagerContext _context;

        public ServiceCategoriesController(RepairManagerContext context)
        {
            _context = context;
        }

        // GET: api/service-categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceCategory>>> GetServiceCategories()
        {
            return await _context.ServiceCategories.ToListAsync();
        }

        // GET: api/service-categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceCategory>> GetServiceCategory(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);

            if (serviceCategory == null)
            {
                return NotFound();
            }

            return serviceCategory;
        }

        // POST: api/service-categories
        [HttpPost]
        public async Task<ActionResult<ServiceCategory>> PostServiceCategory(ServiceCategory serviceCategory)
        {
            _context.ServiceCategories.Add(serviceCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceCategory), new { id = serviceCategory.Id }, serviceCategory);
        }

        // PUT: api/service-categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceCategory(int id, ServiceCategory serviceCategory)
        {
            if (id != serviceCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(serviceCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceCategoryExists(id))
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

        // DELETE: api/service-categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceCategory(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }

            _context.ServiceCategories.Remove(serviceCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceCategoryExists(int id)
        {
            return _context.ServiceCategories.Any(e => e.Id == id);
        }
    }
}
