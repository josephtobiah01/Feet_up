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
    public class Eds12weekPlanController : Controller
    {
        private readonly testfitappContext _context;

        public Eds12weekPlanController(testfitappContext context)
        {
            _context = context;
        }

        // GET: Eds12weekPlan
        public async Task<IActionResult> Index()
        {
              return _context.Eds12weekPlan != null ? 
                          View(await _context.Eds12weekPlan.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.Eds12weekPlan'  is null.");
        }

        // GET: Eds12weekPlan/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Eds12weekPlan == null)
            {
                return NotFound();
            }

            var eds12weekPlan = await _context.Eds12weekPlan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eds12weekPlan == null)
            {
                return NotFound();
            }

            return View(eds12weekPlan);
        }

        // GET: Eds12weekPlan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Eds12weekPlan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsTemplate,IsCurrent,DurationWeeks,StartDate,EndDate,CustomerId")] Eds12weekPlan eds12weekPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eds12weekPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eds12weekPlan);
        }

        // GET: Eds12weekPlan/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Eds12weekPlan == null)
            {
                return NotFound();
            }

            var eds12weekPlan = await _context.Eds12weekPlan.FindAsync(id);
            if (eds12weekPlan == null)
            {
                return NotFound();
            }
            return View(eds12weekPlan);
        }

        // POST: Eds12weekPlan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsTemplate,IsCurrent,DurationWeeks,StartDate,EndDate,CustomerId")] Eds12weekPlan eds12weekPlan)
        {
            if (id != eds12weekPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eds12weekPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Eds12weekPlanExists(eds12weekPlan.Id))
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
            return View(eds12weekPlan);
        }

        // GET: Eds12weekPlan/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Eds12weekPlan == null)
            {
                return NotFound();
            }

            var eds12weekPlan = await _context.Eds12weekPlan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eds12weekPlan == null)
            {
                return NotFound();
            }

            return View(eds12weekPlan);
        }

        // POST: Eds12weekPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Eds12weekPlan == null)
            {
                return Problem("Entity set 'testfitappContext.Eds12weekPlan'  is null.");
            }
            var eds12weekPlan = await _context.Eds12weekPlan.FindAsync(id);
            if (eds12weekPlan != null)
            {
                _context.Eds12weekPlan.Remove(eds12weekPlan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Eds12weekPlanExists(long id)
        {
          return (_context.Eds12weekPlan?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
