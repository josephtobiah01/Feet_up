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
    public class EdsExercisesController : Controller
    {
        private readonly testfitappContext _context;

        public EdsExercisesController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsExercises
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsExercise.Include(e => e.FkExerciseType).Include(e => e.FkTraining);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsExercises/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsExercise == null)
            {
                return NotFound();
            }

            var edsExercise = await _context.EdsExercise
                .Include(e => e.FkExerciseType)
                .Include(e => e.FkTraining)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsExercise == null)
            {
                return NotFound();
            }

            return View(edsExercise);
        }

        // GET: EdsExercises/Create
        public IActionResult Create()
        {
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name");
            ViewData["FkTrainingId"] = new SelectList(_context.EdsTrainingSession, "Id", "Name");
            return View();
        }

        // POST: EdsExercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkTrainingId,FkExerciseTypeId,IsSkipped,IsComplete,EndTimeStamp,IsCustomerAddedExercise")] EdsExercise edsExercise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsExercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsExercise.FkExerciseTypeId);
            ViewData["FkTrainingId"] = new SelectList(_context.EdsTrainingSession, "Id", "Name", edsExercise.FkTrainingId);
            return View(edsExercise);
        }

        // GET: EdsExercises/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsExercise == null)
            {
                return NotFound();
            }

            var edsExercise = await _context.EdsExercise.FindAsync(id);
            if (edsExercise == null)
            {
                return NotFound();
            }
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsExercise.FkExerciseTypeId);
            ViewData["FkTrainingId"] = new SelectList(_context.EdsTrainingSession, "Id", "Name", edsExercise.FkTrainingId);
            return View(edsExercise);
        }

        // POST: EdsExercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkTrainingId,FkExerciseTypeId,IsSkipped,IsComplete,EndTimeStamp,IsCustomerAddedExercise")] EdsExercise edsExercise)
        {
            if (id != edsExercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsExercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsExerciseExists(edsExercise.Id))
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
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsExercise.FkExerciseTypeId);
            ViewData["FkTrainingId"] = new SelectList(_context.EdsTrainingSession, "Id", "Name", edsExercise.FkTrainingId);
            return View(edsExercise);
        }

        // GET: EdsExercises/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsExercise == null)
            {
                return NotFound();
            }

            var edsExercise = await _context.EdsExercise
                .Include(e => e.FkExerciseType)
                .Include(e => e.FkTraining)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsExercise == null)
            {
                return NotFound();
            }

            return View(edsExercise);
        }

        // POST: EdsExercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsExercise == null)
            {
                return Problem("Entity set 'testfitappContext.EdsExercise'  is null.");
            }
            var edsExercise = await _context.EdsExercise.FindAsync(id);
            if (edsExercise != null)
            {
                _context.EdsExercise.Remove(edsExercise);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsExerciseExists(long id)
        {
          return (_context.EdsExercise?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
