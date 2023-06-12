using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataEntryTest.Models;

namespace DataEntryTest.Controllers
{
    public class EdsForcesController : Controller
    {
        private readonly testfitappContext _context;

        public EdsForcesController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsForces
        public async Task<IActionResult> Index()
        {
              return _context.EdsForce != null ? 
                          View(await _context.EdsForce.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsForce'  is null.");
        }

        // GET: EdsForces/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsForce == null)
            {
                return NotFound();
            }

            var edsForce = await _context.EdsForce
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsForce == null)
            {
                return NotFound();
            }

            return View(edsForce);
        }

        // GET: EdsForces/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsForces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsForce edsForce)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsForce);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsForce);
        }

        // GET: EdsForces/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsForce == null)
            {
                return NotFound();
            }

            var edsForce = await _context.EdsForce.FindAsync(id);
            if (edsForce == null)
            {
                return NotFound();
            }
            return View(edsForce);
        }

        // POST: EdsForces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsForce edsForce)
        {
            if (id != edsForce.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsForce);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsForceExists(edsForce.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(edsForce);
        }

        // GET: EdsForces/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsForce == null)
            {
                return NotFound();
            }

            var edsForce = await _context.EdsForce
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsForce == null)
            {
                return NotFound();
            }

            return View(edsForce);
        }

        // POST: EdsForces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsForce == null)
            {
                return Problem("Entity set 'testfitappContext.EdsForce'  is null.");
            }
            var edsForce = await _context.EdsForce.FindAsync(id);
            if (edsForce != null)
            {
                _context.EdsForce.Remove(edsForce);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsForceExists(long id)
        {
          return (_context.EdsForce?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
