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
    public class EdsEquipmentsController : Controller
    {
        private readonly testfitappContext _context;

        public EdsEquipmentsController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsEquipments
        public async Task<IActionResult> Index()
        {
              return _context.EdsEquipment != null ? 
                          View(await _context.EdsEquipment.ToListAsync()) :
                          Problem("Entity set 'testfitappContext.EdsEquipment'  is null.");
        }

        // GET: EdsEquipments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsEquipment == null)
            {
                return NotFound();
            }

            var edsEquipment = await _context.EdsEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsEquipment == null)
            {
                return NotFound();
            }

            return View(edsEquipment);
        }

        // GET: EdsEquipments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EdsEquipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] EdsEquipment edsEquipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsEquipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(edsEquipment);
        }

        // GET: EdsEquipments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsEquipment == null)
            {
                return NotFound();
            }

            var edsEquipment = await _context.EdsEquipment.FindAsync(id);
            if (edsEquipment == null)
            {
                return NotFound();
            }
            return View(edsEquipment);
        }

        // POST: EdsEquipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,IsDeleted")] EdsEquipment edsEquipment)
        {
            if (id != edsEquipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsEquipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsEquipmentExists(edsEquipment.Id))
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
            return View(edsEquipment);
        }

        // GET: EdsEquipments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsEquipment == null)
            {
                return NotFound();
            }

            var edsEquipment = await _context.EdsEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsEquipment == null)
            {
                return NotFound();
            }

            return View(edsEquipment);
        }

        // POST: EdsEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsEquipment == null)
            {
                return Problem("Entity set 'testfitappContext.EdsEquipment'  is null.");
            }
            var edsEquipment = await _context.EdsEquipment.FindAsync(id);
            if (edsEquipment != null)
            {
                _context.EdsEquipment.Remove(edsEquipment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsEquipmentExists(long id)
        {
          return (_context.EdsEquipment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
