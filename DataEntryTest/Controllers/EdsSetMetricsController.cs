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
    public class EdsSetMetricsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsSetMetricsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsSetMetrics
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsSetMetrics.Include(e => e.FkMetricsType).Include(e => e.FkSet);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsSetMetrics/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsSetMetrics == null)
            {
                return NotFound();
            }

            var edsSetMetrics = await _context.EdsSetMetrics
                .Include(e => e.FkMetricsType)
                .Include(e => e.FkSet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetMetrics == null)
            {
                return NotFound();
            }

            return View(edsSetMetrics);
        }

        // GET: EdsSetMetrics/Create
        public IActionResult Create()
        {
            ViewData["FkMetricsTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name");
            ViewData["FkSetId"] = new SelectList(_context.EdsSet, "Id", "Id");
            return View();
        }

        // POST: EdsSetMetrics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkSetId,FkMetricsTypeId,TargetCustomMetric,ActualCustomMetric")] EdsSetMetrics edsSetMetrics)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsSetMetrics);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkMetricsTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsSetMetrics.FkMetricsTypeId);
            ViewData["FkSetId"] = new SelectList(_context.EdsSet, "Id", "Id", edsSetMetrics.FkSetId);
            return View(edsSetMetrics);
        }

        // GET: EdsSetMetrics/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsSetMetrics == null)
            {
                return NotFound();
            }

            var edsSetMetrics = await _context.EdsSetMetrics.FindAsync(id);
            if (edsSetMetrics == null)
            {
                return NotFound();
            }
            ViewData["FkMetricsTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsSetMetrics.FkMetricsTypeId);
            ViewData["FkSetId"] = new SelectList(_context.EdsSet, "Id", "Id", edsSetMetrics.FkSetId);
            return View(edsSetMetrics);
        }

        // POST: EdsSetMetrics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkSetId,FkMetricsTypeId,TargetCustomMetric,ActualCustomMetric")] EdsSetMetrics edsSetMetrics)
        {
            if (id != edsSetMetrics.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsSetMetrics);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsSetMetricsExists(edsSetMetrics.Id))
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
            ViewData["FkMetricsTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsSetMetrics.FkMetricsTypeId);
            ViewData["FkSetId"] = new SelectList(_context.EdsSet, "Id", "Id", edsSetMetrics.FkSetId);
            return View(edsSetMetrics);
        }

        // GET: EdsSetMetrics/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsSetMetrics == null)
            {
                return NotFound();
            }

            var edsSetMetrics = await _context.EdsSetMetrics
                .Include(e => e.FkMetricsType)
                .Include(e => e.FkSet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetMetrics == null)
            {
                return NotFound();
            }

            return View(edsSetMetrics);
        }

        // POST: EdsSetMetrics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsSetMetrics == null)
            {
                return Problem("Entity set 'testfitappContext.EdsSetMetrics'  is null.");
            }
            var edsSetMetrics = await _context.EdsSetMetrics.FindAsync(id);
            if (edsSetMetrics != null)
            {
                _context.EdsSetMetrics.Remove(edsSetMetrics);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsSetMetricsExists(long id)
        {
          return (_context.EdsSetMetrics?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
