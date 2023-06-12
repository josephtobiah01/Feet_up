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
    public class EdsSetsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsSetsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsSets
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsSet.Include(e => e.FkExercise);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsSets/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsSet == null)
            {
                return NotFound();
            }

            var edsSet = await _context.EdsSet
                .Include(e => e.FkExercise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSet == null)
            {
                return NotFound();
            }

            return View(edsSet);
        }

        // GET: EdsSets/Create
        public IActionResult Create()
        {
            ViewData["FkExerciseId"] = new SelectList(_context.EdsExercise, "Id", "Id");
            return View();
        }

        // POST: EdsSets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkExerciseId,SetSequenceNumber,IsComplete,IsSkipped,IsCustomerAddedSet,EndTimeStamp")] EdsSet edsSet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkExerciseId"] = new SelectList(_context.EdsExercise, "Id", "Id", edsSet.FkExerciseId);
            return View(edsSet);
        }

        // GET: EdsSets/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsSet == null)
            {
                return NotFound();
            }

            var edsSet = await _context.EdsSet.FindAsync(id);
            if (edsSet == null)
            {
                return NotFound();
            }
            ViewData["FkExerciseId"] = new SelectList(_context.EdsExercise, "Id", "Id", edsSet.FkExerciseId);
            return View(edsSet);
        }

        // POST: EdsSets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkExerciseId,SetSequenceNumber,IsComplete,IsSkipped,IsCustomerAddedSet,EndTimeStamp")] EdsSet edsSet)
        {
            if (id != edsSet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsSetExists(edsSet.Id))
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
            ViewData["FkExerciseId"] = new SelectList(_context.EdsExercise, "Id", "Id", edsSet.FkExerciseId);
            return View(edsSet);
        }

        // GET: EdsSets/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsSet == null)
            {
                return NotFound();
            }

            var edsSet = await _context.EdsSet
                .Include(e => e.FkExercise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSet == null)
            {
                return NotFound();
            }

            return View(edsSet);
        }

        // POST: EdsSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsSet == null)
            {
                return Problem("Entity set 'testfitappContext.EdsSet'  is null.");
            }
            var edsSet = await _context.EdsSet.FindAsync(id);
            if (edsSet != null)
            {
                _context.EdsSet.Remove(edsSet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsSetExists(long id)
        {
          return (_context.EdsSet?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
