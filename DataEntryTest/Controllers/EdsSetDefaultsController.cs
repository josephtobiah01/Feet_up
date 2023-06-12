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
    public class EdsSetDefaultsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsSetDefaultsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsSetDefaults
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsSetDefaults.Include(e => e.FkExerciseType);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsSetDefaults/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsSetDefaults == null)
            {
                return NotFound();
            }

            var edsSetDefaults = await _context.EdsSetDefaults
                .Include(e => e.FkExerciseType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetDefaults == null)
            {
                return NotFound();
            }

            return View(edsSetDefaults);
        }

        // GET: EdsSetDefaults/Create
        public IActionResult Create()
        {
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name");
            return View();
        }

        // POST: EdsSetDefaults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkExerciseTypeId,SetSequenceNumber")] EdsSetDefaults edsSetDefaults)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsSetDefaults);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsSetDefaults.FkExerciseTypeId);
            return View(edsSetDefaults);
        }

        // GET: EdsSetDefaults/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsSetDefaults == null)
            {
                return NotFound();
            }

            var edsSetDefaults = await _context.EdsSetDefaults.FindAsync(id);
            if (edsSetDefaults == null)
            {
                return NotFound();
            }
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsSetDefaults.FkExerciseTypeId);
            return View(edsSetDefaults);
        }

        // POST: EdsSetDefaults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkExerciseTypeId,SetSequenceNumber")] EdsSetDefaults edsSetDefaults)
        {
            if (id != edsSetDefaults.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsSetDefaults);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsSetDefaultsExists(edsSetDefaults.Id))
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
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsSetDefaults.FkExerciseTypeId);
            return View(edsSetDefaults);
        }

        // GET: EdsSetDefaults/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsSetDefaults == null)
            {
                return NotFound();
            }

            var edsSetDefaults = await _context.EdsSetDefaults
                .Include(e => e.FkExerciseType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetDefaults == null)
            {
                return NotFound();
            }

            return View(edsSetDefaults);
        }

        // POST: EdsSetDefaults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsSetDefaults == null)
            {
                return Problem("Entity set 'testfitappContext.EdsSetDefaults'  is null.");
            }
            var edsSetDefaults = await _context.EdsSetDefaults.FindAsync(id);
            if (edsSetDefaults != null)
            {
                _context.EdsSetDefaults.Remove(edsSetDefaults);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsSetDefaultsExists(long id)
        {
          return (_context.EdsSetDefaults?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
