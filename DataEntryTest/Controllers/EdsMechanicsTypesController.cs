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
    public class EdsMechanicsTypesController : Controller
    {
        private readonly testfitappContext _context;

        public EdsMechanicsTypesController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsMechanicsTypes
        public async Task<IActionResult> Index()
        {
              return _context.EdsMechanicsType != null ? 
                          View(await _context.EdsMechanicsType.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsMechanicsType'  is null.");
        }

        // GET: EdsMechanicsTypes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsMechanicsType == null)
            {
                return NotFound();
            }

            var edsMechanicsType = await _context.EdsMechanicsType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsMechanicsType == null)
            {
                return NotFound();
            }

            return View(edsMechanicsType);
        }

        // GET: EdsMechanicsTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsMechanicsTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsMechanicsType edsMechanicsType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsMechanicsType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsMechanicsType);
        }

        // GET: EdsMechanicsTypes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsMechanicsType == null)
            {
                return NotFound();
            }

            var edsMechanicsType = await _context.EdsMechanicsType.FindAsync(id);
            if (edsMechanicsType == null)
            {
                return NotFound();
            }
            return View(edsMechanicsType);
        }

        // POST: EdsMechanicsTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsMechanicsType edsMechanicsType)
        {
            if (id != edsMechanicsType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsMechanicsType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsMechanicsTypeExists(edsMechanicsType.Id))
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
            return View(edsMechanicsType);
        }

        // GET: EdsMechanicsTypes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsMechanicsType == null)
            {
                return NotFound();
            }

            var edsMechanicsType = await _context.EdsMechanicsType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsMechanicsType == null)
            {
                return NotFound();
            }

            return View(edsMechanicsType);
        }

        // POST: EdsMechanicsTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsMechanicsType == null)
            {
                return Problem("Entity set 'testfitappContext.EdsMechanicsType'  is null.");
            }
            var edsMechanicsType = await _context.EdsMechanicsType.FindAsync(id);
            if (edsMechanicsType != null)
            {
                _context.EdsMechanicsType.Remove(edsMechanicsType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsMechanicsTypeExists(long id)
        {
          return (_context.EdsMechanicsType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
