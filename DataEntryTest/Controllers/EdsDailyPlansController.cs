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
    public class EdsDailyPlansController : Controller
    {
        private readonly testfitappContext _context;
          
        public EdsDailyPlansController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsDailyPlans
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsDailyPlan.Include(e => e.FkEdsWeeklyPlan);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsDailyPlans/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsDailyPlan == null)
            {
                return NotFound();
            }

            var edsDailyPlan = await _context.EdsDailyPlan
                .Include(e => e.FkEdsWeeklyPlan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsDailyPlan == null)
            {
                return NotFound();
            }

            return View(edsDailyPlan);
        }

        // GET: EdsDailyPlans/Create
        public IActionResult Create()
        {
            ViewData["FkEdsWeeklyPlanId"] = new SelectList(_context.EdsWeeklyPlan, "Id", "Id");
            return View();
        }

        // POST: EdsDailyPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkEdsWeeklyPlanId,StartDay,EndDay,IsComplete")] EdsDailyPlan edsDailyPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsDailyPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkEdsWeeklyPlanId"] = new SelectList(_context.EdsWeeklyPlan, "Id", "Id", edsDailyPlan.FkEdsWeeklyPlanId);
            return View(edsDailyPlan);
        }

        // GET: EdsDailyPlans/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsDailyPlan == null)
            {
                return NotFound();
            }

            var edsDailyPlan = await _context.EdsDailyPlan.FindAsync(id);
            if (edsDailyPlan == null)
            {
                return NotFound();
            }
            ViewData["FkEdsWeeklyPlanId"] = new SelectList(_context.EdsWeeklyPlan, "Id", "Id", edsDailyPlan.FkEdsWeeklyPlanId);
            return View(edsDailyPlan);
        }

        // POST: EdsDailyPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkEdsWeeklyPlanId,StartDay,EndDay,IsComplete")] EdsDailyPlan edsDailyPlan)
        {
            if (id != edsDailyPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsDailyPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsDailyPlanExists(edsDailyPlan.Id))
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
            ViewData["FkEdsWeeklyPlanId"] = new SelectList(_context.EdsWeeklyPlan, "Id", "Id", edsDailyPlan.FkEdsWeeklyPlanId);
            return View(edsDailyPlan);
        }

        // GET: EdsDailyPlans/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsDailyPlan == null)
            {
                return NotFound();
            }

            var edsDailyPlan = await _context.EdsDailyPlan
                .Include(e => e.FkEdsWeeklyPlan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsDailyPlan == null)
            {
                return NotFound();
            }

            return View(edsDailyPlan);
        }

        // POST: EdsDailyPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsDailyPlan == null)
            {
                return Problem("Entity set 'testfitappContext.EdsDailyPlan'  is null.");
            }
            var edsDailyPlan = await _context.EdsDailyPlan.FindAsync(id);
            if (edsDailyPlan != null)
            {
                _context.EdsDailyPlan.Remove(edsDailyPlan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsDailyPlanExists(long id)
        {
          return (_context.EdsDailyPlan?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
