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
    public class EdsWeeklyPlansController : Controller
    {
        private readonly testfitappContext _context;

        public EdsWeeklyPlansController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsWeeklyPlans
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsWeeklyPlan.Include(e => e.FkEds12weekPlanNavigation);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsWeeklyPlans/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsWeeklyPlan == null)
            {
                return NotFound();
            }

            var edsWeeklyPlan = await _context.EdsWeeklyPlan
                .Include(e => e.FkEds12weekPlanNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsWeeklyPlan == null)
            {
                return NotFound();
            }

            return View(edsWeeklyPlan);
        }

        // GET: EdsWeeklyPlans/Create
        public IActionResult Create()
        {
            ViewData["FkEds12weekPlan"] = new SelectList(_context.Eds12weekPlan, "Id", "Name");
            return View();
        }

        // POST: EdsWeeklyPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,FkEds12weekPlan")] EdsWeeklyPlan edsWeeklyPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsWeeklyPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkEds12weekPlan"] = new SelectList(_context.Eds12weekPlan, "Id", "Name", edsWeeklyPlan.FkEds12weekPlan);
            return View(edsWeeklyPlan);
        }

        // GET: EdsWeeklyPlans/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsWeeklyPlan == null)
            {
                return NotFound();
            }

            var edsWeeklyPlan = await _context.EdsWeeklyPlan.FindAsync(id);
            if (edsWeeklyPlan == null)
            {
                return NotFound();
            }
            ViewData["FkEds12weekPlan"] = new SelectList(_context.Eds12weekPlan, "Id", "Name", edsWeeklyPlan.FkEds12weekPlan);
            return View(edsWeeklyPlan);
        }

        // POST: EdsWeeklyPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,StartDate,EndDate,FkEds12weekPlan")] EdsWeeklyPlan edsWeeklyPlan)
        {
            if (id != edsWeeklyPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsWeeklyPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsWeeklyPlanExists(edsWeeklyPlan.Id))
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
            ViewData["FkEds12weekPlan"] = new SelectList(_context.Eds12weekPlan, "Id", "Name", edsWeeklyPlan.FkEds12weekPlan);
            return View(edsWeeklyPlan);
        }

        // GET: EdsWeeklyPlans/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsWeeklyPlan == null)
            {
                return NotFound();
            }

            var edsWeeklyPlan = await _context.EdsWeeklyPlan
                .Include(e => e.FkEds12weekPlanNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsWeeklyPlan == null)
            {
                return NotFound();
            }

            return View(edsWeeklyPlan);
        }

        // POST: EdsWeeklyPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsWeeklyPlan == null)
            {
                return Problem("Entity set 'testfitappContext.EdsWeeklyPlan'  is null.");
            }
            var edsWeeklyPlan = await _context.EdsWeeklyPlan.FindAsync(id);
            if (edsWeeklyPlan != null)
            {
                _context.EdsWeeklyPlan.Remove(edsWeeklyPlan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsWeeklyPlanExists(long id)
        {
          return (_context.EdsWeeklyPlan?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
