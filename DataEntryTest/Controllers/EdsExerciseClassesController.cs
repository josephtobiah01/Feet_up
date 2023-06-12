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
    public class EdsExerciseClassesController : Controller
    {
        private readonly testfitappContext _context;

        public EdsExerciseClassesController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsExerciseClasses
        public async Task<IActionResult> Index()
        {
              return _context.EdsExerciseClass != null ? 
                          View(await _context.EdsExerciseClass.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsExerciseClass'  is null.");
        }

        // GET: EdsExerciseClasses/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsExerciseClass == null)
            {
                return NotFound();
            }

            var edsExerciseClass = await _context.EdsExerciseClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsExerciseClass == null)
            {
                return NotFound();
            }

            return View(edsExerciseClass);
        }

        // GET: EdsExerciseClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsExerciseClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsExerciseClass edsExerciseClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsExerciseClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsExerciseClass);
        }

        // GET: EdsExerciseClasses/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsExerciseClass == null)
            {
                return NotFound();
            }

            var edsExerciseClass = await _context.EdsExerciseClass.FindAsync(id);
            if (edsExerciseClass == null)
            {
                return NotFound();
            }
            return View(edsExerciseClass);
        }

        // POST: EdsExerciseClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsExerciseClass edsExerciseClass)
        {
            if (id != edsExerciseClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsExerciseClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsExerciseClassExists(edsExerciseClass.Id))
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
            return View(edsExerciseClass);
        }

        // GET: EdsExerciseClasses/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsExerciseClass == null)
            {
                return NotFound();
            }

            var edsExerciseClass = await _context.EdsExerciseClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsExerciseClass == null)
            {
                return NotFound();
            }

            return View(edsExerciseClass);
        }

        // POST: EdsExerciseClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsExerciseClass == null)
            {
                return Problem("Entity set 'testfitappContext.EdsExerciseClass'  is null.");
            }
            var edsExerciseClass = await _context.EdsExerciseClass.FindAsync(id);
            if (edsExerciseClass != null)
            {
                _context.EdsExerciseClass.Remove(edsExerciseClass);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsExerciseClassExists(long id)
        {
          return (_context.EdsExerciseClass?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
