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
    public class EdsSetMetricTypesController : Controller
    {
        private readonly testfitappContext _context;

        public EdsSetMetricTypesController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsSetMetricTypes
        public async Task<IActionResult> Index()
        {
              return _context.EdsSetMetricTypes != null ? 
                          View(await _context.EdsSetMetricTypes.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsSetMetricTypes'  is null.");
        }

        // GET: EdsSetMetricTypes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsSetMetricTypes == null)
            {
                return NotFound();
            }

            var edsSetMetricTypes = await _context.EdsSetMetricTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetMetricTypes == null)
            {
                return NotFound();
            }

            return View(edsSetMetricTypes);
        }

        // GET: EdsSetMetricTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsSetMetricTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted,IsRepetition,IsWeight,IsResistance,IsDistance,IsTime")] EdsSetMetricTypes edsSetMetricTypes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsSetMetricTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsSetMetricTypes);
        }

        // GET: EdsSetMetricTypes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsSetMetricTypes == null)
            {
                return NotFound();
            }

            var edsSetMetricTypes = await _context.EdsSetMetricTypes.FindAsync(id);
            if (edsSetMetricTypes == null)
            {
                return NotFound();
            }
            return View(edsSetMetricTypes);
        }

        // POST: EdsSetMetricTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted,IsRepetition,IsWeight,IsResistance,IsDistance,IsTime")] EdsSetMetricTypes edsSetMetricTypes)
        {
            if (id != edsSetMetricTypes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsSetMetricTypes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsSetMetricTypesExists(edsSetMetricTypes.Id))
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
            return View(edsSetMetricTypes);
        }

        // GET: EdsSetMetricTypes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsSetMetricTypes == null)
            {
                return NotFound();
            }

            var edsSetMetricTypes = await _context.EdsSetMetricTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSetMetricTypes == null)
            {
                return NotFound();
            }

            return View(edsSetMetricTypes);
        }

        // POST: EdsSetMetricTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsSetMetricTypes == null)
            {
                return Problem("Entity set 'testfitappContext.EdsSetMetricTypes'  is null.");
            }
            var edsSetMetricTypes = await _context.EdsSetMetricTypes.FindAsync(id);
            if (edsSetMetricTypes != null)
            {
                _context.EdsSetMetricTypes.Remove(edsSetMetricTypes);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsSetMetricTypesExists(long id)
        {
          return (_context.EdsSetMetricTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
