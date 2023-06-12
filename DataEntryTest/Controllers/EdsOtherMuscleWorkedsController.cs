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
    public class EdsOtherMuscleWorkedsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsOtherMuscleWorkedsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsOtherMuscleWorkeds
        public async Task<IActionResult> Index()
        {
              return _context.EdsOtherMuscleWorked != null ? 
                          View(await _context.EdsOtherMuscleWorked.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsOtherMuscleWorked'  is null.");
        }

        // GET: EdsOtherMuscleWorkeds/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsOtherMuscleWorked == null)
            {
                return NotFound();
            }

            var edsOtherMuscleWorked = await _context.EdsOtherMuscleWorked
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsOtherMuscleWorked == null)
            {
                return NotFound();
            }

            return View(edsOtherMuscleWorked);
        }

        // GET: EdsOtherMuscleWorkeds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsOtherMuscleWorkeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsOtherMuscleWorked edsOtherMuscleWorked)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsOtherMuscleWorked);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsOtherMuscleWorked);
        }

        // GET: EdsOtherMuscleWorkeds/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsOtherMuscleWorked == null)
            {
                return NotFound();
            }

            var edsOtherMuscleWorked = await _context.EdsOtherMuscleWorked.FindAsync(id);
            if (edsOtherMuscleWorked == null)
            {
                return NotFound();
            }
            return View(edsOtherMuscleWorked);
        }

        // POST: EdsOtherMuscleWorkeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsOtherMuscleWorked edsOtherMuscleWorked)
        {
            if (id != edsOtherMuscleWorked.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsOtherMuscleWorked);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsOtherMuscleWorkedExists(edsOtherMuscleWorked.Id))
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
            return View(edsOtherMuscleWorked);
        }

        // GET: EdsOtherMuscleWorkeds/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsOtherMuscleWorked == null)
            {
                return NotFound();
            }

            var edsOtherMuscleWorked = await _context.EdsOtherMuscleWorked
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsOtherMuscleWorked == null)
            {
                return NotFound();
            }

            return View(edsOtherMuscleWorked);
        }

        // POST: EdsOtherMuscleWorkeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsOtherMuscleWorked == null)
            {
                return Problem("Entity set 'testfitappContext.EdsOtherMuscleWorked'  is null.");
            }
            var edsOtherMuscleWorked = await _context.EdsOtherMuscleWorked.FindAsync(id);
            if (edsOtherMuscleWorked != null)
            {
                _context.EdsOtherMuscleWorked.Remove(edsOtherMuscleWorked);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsOtherMuscleWorkedExists(long id)
        {
          return (_context.EdsOtherMuscleWorked?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
