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
    public class EdsSportsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsSportsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsSports
        public async Task<IActionResult> Index()
        {
              return _context.EdsSport != null ? 
                          View(await _context.EdsSport.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsSport'  is null.");
        }

        // GET: EdsSports/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsSport == null)
            {
                return NotFound();
            }

            var edsSport = await _context.EdsSport
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSport == null)
            {
                return NotFound();
            }

            return View(edsSport);
        }

        // GET: EdsSports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsSports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsSport edsSport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsSport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsSport);
        }

        // GET: EdsSports/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsSport == null)
            {
                return NotFound();
            }

            var edsSport = await _context.EdsSport.FindAsync(id);
            if (edsSport == null)
            {
                return NotFound();
            }
            return View(edsSport);
        }

        // POST: EdsSports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsSport edsSport)
        {
            if (id != edsSport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsSport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsSportExists(edsSport.Id))
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
            return View(edsSport);
        }

        // GET: EdsSports/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsSport == null)
            {
                return NotFound();
            }

            var edsSport = await _context.EdsSport
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsSport == null)
            {
                return NotFound();
            }

            return View(edsSport);
        }

        // POST: EdsSports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsSport == null)
            {
                return Problem("Entity set 'testfitappContext.EdsSport'  is null.");
            }
            var edsSport = await _context.EdsSport.FindAsync(id);
            if (edsSport != null)
            {
                _context.EdsSport.Remove(edsSport);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsSportExists(long id)
        {
          return (_context.EdsSport?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
