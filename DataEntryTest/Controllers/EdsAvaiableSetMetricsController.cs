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
    public class EdsAvaiableSetMetricsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsAvaiableSetMetricsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsAvaiableSetMetrics
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsAvaiableSetMetrics.Include(e => e.FkExerciseType).Include(e => e.FkSetMetricsTypes);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsAvaiableSetMetrics/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsAvaiableSetMetrics == null)
            {
                return NotFound();
            }

            var edsAvaiableSetMetrics = await _context.EdsAvaiableSetMetrics
                .Include(e => e.FkExerciseType)
                .Include(e => e.FkSetMetricsTypes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsAvaiableSetMetrics == null)
            {
                return NotFound();
            }

            return View(edsAvaiableSetMetrics);
        }

        // GET: EdsAvaiableSetMetrics/Create
        public IActionResult Create()
        {
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name");
            ViewData["FkSetMetricsTypesId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name");
            return View();
        }

        // POST: EdsAvaiableSetMetrics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkExerciseTypeId,FkSetMetricsTypesId,IsDeleted")] EdsAvaiableSetMetrics edsAvaiableSetMetrics)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsAvaiableSetMetrics);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsAvaiableSetMetrics.FkExerciseTypeId);
            ViewData["FkSetMetricsTypesId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsAvaiableSetMetrics.FkSetMetricsTypesId);
            return View(edsAvaiableSetMetrics);
        }

        // GET: EdsAvaiableSetMetrics/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsAvaiableSetMetrics == null)
            {
                return NotFound();
            }

            var edsAvaiableSetMetrics = await _context.EdsAvaiableSetMetrics.FindAsync(id);
            if (edsAvaiableSetMetrics == null)
            {
                return NotFound();
            }
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsAvaiableSetMetrics.FkExerciseTypeId);
            ViewData["FkSetMetricsTypesId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsAvaiableSetMetrics.FkSetMetricsTypesId);
            return View(edsAvaiableSetMetrics);
        }

        // POST: EdsAvaiableSetMetrics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkExerciseTypeId,FkSetMetricsTypesId,IsDeleted")] EdsAvaiableSetMetrics edsAvaiableSetMetrics)
        {
            if (id != edsAvaiableSetMetrics.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsAvaiableSetMetrics);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsAvaiableSetMetricsExists(edsAvaiableSetMetrics.Id))
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
            ViewData["FkExerciseTypeId"] = new SelectList(_context.EdsExerciseType, "Id", "Name", edsAvaiableSetMetrics.FkExerciseTypeId);
            ViewData["FkSetMetricsTypesId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsAvaiableSetMetrics.FkSetMetricsTypesId);
            return View(edsAvaiableSetMetrics);
        }

        // GET: EdsAvaiableSetMetrics/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsAvaiableSetMetrics == null)
            {
                return NotFound();
            }

            var edsAvaiableSetMetrics = await _context.EdsAvaiableSetMetrics
                .Include(e => e.FkExerciseType)
                .Include(e => e.FkSetMetricsTypes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsAvaiableSetMetrics == null)
            {
                return NotFound();
            }

            return View(edsAvaiableSetMetrics);
        }

        // POST: EdsAvaiableSetMetrics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsAvaiableSetMetrics == null)
            {
                return Problem("Entity set 'testfitappContext.EdsAvaiableSetMetrics'  is null.");
            }
            var edsAvaiableSetMetrics = await _context.EdsAvaiableSetMetrics.FindAsync(id);
            if (edsAvaiableSetMetrics != null)
            {
                _context.EdsAvaiableSetMetrics.Remove(edsAvaiableSetMetrics);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsAvaiableSetMetricsExists(long id)
        {
          return (_context.EdsAvaiableSetMetrics?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
