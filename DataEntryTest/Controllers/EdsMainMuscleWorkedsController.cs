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
    public class EdsMainMuscleWorkedsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsMainMuscleWorkedsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsMainMuscleWorkeds
        public async Task<IActionResult> Index()
        {
              return _context.EdsMainMuscleWorked != null ? 
                          View(await _context.EdsMainMuscleWorked.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsMainMuscleWorked'  is null.");
        }

        // GET: EdsMainMuscleWorkeds/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsMainMuscleWorked == null)
            {
                return NotFound();
            }

            var edsMainMuscleWorked = await _context.EdsMainMuscleWorked
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsMainMuscleWorked == null)
            {
                return NotFound();
            }

            return View(edsMainMuscleWorked);
        }

        // GET: EdsMainMuscleWorkeds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsMainMuscleWorkeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsMainMuscleWorked edsMainMuscleWorked)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsMainMuscleWorked);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsMainMuscleWorked);
        }

        // GET: EdsMainMuscleWorkeds/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsMainMuscleWorked == null)
            {
                return NotFound();
            }

            var edsMainMuscleWorked = await _context.EdsMainMuscleWorked.FindAsync(id);
            if (edsMainMuscleWorked == null)
            {
                return NotFound();
            }
            return View(edsMainMuscleWorked);
        }

        // POST: EdsMainMuscleWorkeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsMainMuscleWorked edsMainMuscleWorked)
        {
            if (id != edsMainMuscleWorked.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsMainMuscleWorked);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsMainMuscleWorkedExists(edsMainMuscleWorked.Id))
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
            return View(edsMainMuscleWorked);
        }

        // GET: EdsMainMuscleWorkeds/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsMainMuscleWorked == null)
            {
                return NotFound();
            }

            var edsMainMuscleWorked = await _context.EdsMainMuscleWorked
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsMainMuscleWorked == null)
            {
                return NotFound();
            }

            return View(edsMainMuscleWorked);
        }

        // POST: EdsMainMuscleWorkeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsMainMuscleWorked == null)
            {
                return Problem("Entity set 'testfitappContext.EdsMainMuscleWorked'  is null.");
            }
            var edsMainMuscleWorked = await _context.EdsMainMuscleWorked.FindAsync(id);
            if (edsMainMuscleWorked != null)
            {
                _context.EdsMainMuscleWorked.Remove(edsMainMuscleWorked);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsMainMuscleWorkedExists(long id)
        {
          return (_context.EdsMainMuscleWorked?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
