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
    public class EdsTrainingSessionsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsTrainingSessionsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsTrainingSessions
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsTrainingSession.Include(e => e.FkEdsDailyPlanNavigation);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsTrainingSessions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsTrainingSession == null)
            {
                return NotFound();
            }

            var edsTrainingSession = await _context.EdsTrainingSession
                .Include(e => e.FkEdsDailyPlanNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsTrainingSession == null)
            {
                return NotFound();
            }

            return View(edsTrainingSession);
        }

        // GET: EdsTrainingSessions/Create
        public IActionResult Create()
        {
            ViewData["FkEdsDailyPlan"] = new SelectList(_context.EdsDailyPlan, "Id", "Id");
            return View();
        }

        // POST: EdsTrainingSessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkEdsDailyPlan,Name,StartDateTime,EndDateTime,IsMoved,IsSkipped,EndTimeStamp,IsCustomerAddedTrainingSession,ReasonForReschedule,ReadonForSkipping")] EdsTrainingSession edsTrainingSession)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsTrainingSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkEdsDailyPlan"] = new SelectList(_context.EdsDailyPlan, "Id", "Id", edsTrainingSession.FkEdsDailyPlan);
            return View(edsTrainingSession);
        }

        // GET: EdsTrainingSessions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsTrainingSession == null)
            {
                return NotFound();
            }

            var edsTrainingSession = await _context.EdsTrainingSession.FindAsync(id);
            if (edsTrainingSession == null)
            {
                return NotFound();
            }
            ViewData["FkEdsDailyPlan"] = new SelectList(_context.EdsDailyPlan, "Id", "Id", edsTrainingSession.FkEdsDailyPlan);
            return View(edsTrainingSession);
        }

        // POST: EdsTrainingSessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkEdsDailyPlan,Name,StartDateTime,EndDateTime,IsMoved,IsSkipped,EndTimeStamp,IsCustomerAddedTrainingSession,ReasonForReschedule,ReadonForSkipping")] EdsTrainingSession edsTrainingSession)
        {
            if (id != edsTrainingSession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsTrainingSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsTrainingSessionExists(edsTrainingSession.Id))
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
            ViewData["FkEdsDailyPlan"] = new SelectList(_context.EdsDailyPlan, "Id", "Id", edsTrainingSession.FkEdsDailyPlan);
            return View(edsTrainingSession);
        }

        // GET: EdsTrainingSessions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsTrainingSession == null)
            {
                return NotFound();
            }

            var edsTrainingSession = await _context.EdsTrainingSession
                .Include(e => e.FkEdsDailyPlanNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsTrainingSession == null)
            {
                return NotFound();
            }

            return View(edsTrainingSession);
        }

        // POST: EdsTrainingSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsTrainingSession == null)
            {
                return Problem("Entity set 'testfitappContext.EdsTrainingSession'  is null.");
            }
            var edsTrainingSession = await _context.EdsTrainingSession.FindAsync(id);
            if (edsTrainingSession != null)
            {
                _context.EdsTrainingSession.Remove(edsTrainingSession);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsTrainingSessionExists(long id)
        {
          return (_context.EdsTrainingSession?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
