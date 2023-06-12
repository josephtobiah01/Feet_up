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
    public class EdsLevelsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsLevelsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsLevels
        public async Task<IActionResult> Index()
        {
              return _context.EdsLevel != null ? 
                          View(await _context.EdsLevel.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsLevel'  is null.");
        }

        // GET: EdsLevels/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsLevel == null)
            {
                return NotFound();
            }

            var edsLevel = await _context.EdsLevel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsLevel == null)
            {
                return NotFound();
            }

            return View(edsLevel);
        }

        // GET: EdsLevels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsLevel edsLevel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsLevel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsLevel);
        }

        // GET: EdsLevels/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsLevel == null)
            {
                return NotFound();
            }

            var edsLevel = await _context.EdsLevel.FindAsync(id);
            if (edsLevel == null)
            {
                return NotFound();
            }
            return View(edsLevel);
        }

        // POST: EdsLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsLevel edsLevel)
        {
            if (id != edsLevel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsLevelExists(edsLevel.Id))
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
            return View(edsLevel);
        }

        // GET: EdsLevels/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsLevel == null)
            {
                return NotFound();
            }

            var edsLevel = await _context.EdsLevel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsLevel == null)
            {
                return NotFound();
            }

            return View(edsLevel);
        }

        // POST: EdsLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsLevel == null)
            {
                return Problem("Entity set 'testfitappContext.EdsLevel'  is null.");
            }
            var edsLevel = await _context.EdsLevel.FindAsync(id);
            if (edsLevel != null)
            {
                _context.EdsLevel.Remove(edsLevel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsLevelExists(long id)
        {
          return (_context.EdsLevel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
