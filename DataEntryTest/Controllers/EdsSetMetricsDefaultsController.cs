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
    public class EdsSetMetricsDefaultsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsSetMetricsDefaultsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsSetMetricsDefaults
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsSetMetricsDefault.Include(e => e.FkSetDefaults).Include(e => e.FkSetMetricType);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsSetMetricsDefaults/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsSetMetricsDefault == null)
            {
                return NotFound();
            }

            var edsSetMetricsDefault = await _context.EdsSetMetricsDefault
                .Include(e => e.FkSetDefaults)
                .Include(e => e.FkSetMetricType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetMetricsDefault == null)
            {
                return NotFound();
            }

            return View(edsSetMetricsDefault);
        }

        // GET: EdsSetMetricsDefaults/Create
        public IActionResult Create()
        {
            ViewData["FkSetDefaultsId"] = new SelectList(_context.EdsSetDefaults, "Id", "Id");
            ViewData["FkSetMetricTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name");
            return View();
        }

        // POST: EdsSetMetricsDefaults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkSetDefaultsId,FkSetMetricTypeId,DefaultCustomMetric")] EdsSetMetricsDefault edsSetMetricsDefault)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsSetMetricsDefault);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkSetDefaultsId"] = new SelectList(_context.EdsSetDefaults, "Id", "Id", edsSetMetricsDefault.FkSetDefaultsId);
            ViewData["FkSetMetricTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsSetMetricsDefault.FkSetMetricTypeId);
            return View(edsSetMetricsDefault);
        }

        // GET: EdsSetMetricsDefaults/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsSetMetricsDefault == null)
            {
                return NotFound();
            }

            var edsSetMetricsDefault = await _context.EdsSetMetricsDefault.FindAsync(id);
            if (edsSetMetricsDefault == null)
            {
                return NotFound();
            }
            ViewData["FkSetDefaultsId"] = new SelectList(_context.EdsSetDefaults, "Id", "Id", edsSetMetricsDefault.FkSetDefaultsId);
            ViewData["FkSetMetricTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsSetMetricsDefault.FkSetMetricTypeId);
            return View(edsSetMetricsDefault);
        }

        // POST: EdsSetMetricsDefaults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FkSetDefaultsId,FkSetMetricTypeId,DefaultCustomMetric")] EdsSetMetricsDefault edsSetMetricsDefault)
        {
            if (id != edsSetMetricsDefault.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsSetMetricsDefault);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsSetMetricsDefaultExists(edsSetMetricsDefault.Id))
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
            ViewData["FkSetDefaultsId"] = new SelectList(_context.EdsSetDefaults, "Id", "Id", edsSetMetricsDefault.FkSetDefaultsId);
            ViewData["FkSetMetricTypeId"] = new SelectList(_context.EdsSetMetricTypes, "Id", "Name", edsSetMetricsDefault.FkSetMetricTypeId);
            return View(edsSetMetricsDefault);
        }

        // GET: EdsSetMetricsDefaults/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsSetMetricsDefault == null)
            {
                return NotFound();
            }

            var edsSetMetricsDefault = await _context.EdsSetMetricsDefault
                .Include(e => e.FkSetDefaults)
                .Include(e => e.FkSetMetricType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetMetricsDefault == null)
            {
                return NotFound();
            }

            return View(edsSetMetricsDefault);
        }

        // POST: EdsSetMetricsDefaults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsSetMetricsDefault == null)
            {
                return Problem("Entity set 'testfitappContext.EdsSetMetricsDefault'  is null.");
            }
            var edsSetMetricsDefault = await _context.EdsSetMetricsDefault.FindAsync(id);
            if (edsSetMetricsDefault != null)
            {
                _context.EdsSetMetricsDefault.Remove(edsSetMetricsDefault);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsSetMetricsDefaultExists(long id)
        {
          return (_context.EdsSetMetricsDefault?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
