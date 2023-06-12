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
    public class EdsExerciseTypesController : Controller
    {
        private readonly testfitappContext _context;

        public EdsExerciseTypesController(testfitappContext context)
        {
            _context = context;
        }

        // GET: EdsExerciseTypes
        public async Task<IActionResult> Index()
        {
            var testfitappContext = _context.EdsExerciseType.Include(e => e.FkEquipment).Include(e => e.FkExerciseClass).Include(e => e.FkForce).Include(e => e.FkLevel).Include(e => e.FkMainMuscleWorked).Include(e => e.FkMechanicsType).Include(e => e.FkOtherMuscleWorked).Include(e => e.FkSport);
            return View(await testfitappContext.ToListAsync());
        }

        // GET: EdsExerciseTypes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EdsExerciseType == null)
            {
                return NotFound();
            }

            var edsExerciseType = await _context.EdsExerciseType
                .Include(e => e.FkEquipment)
                .Include(e => e.FkExerciseClass)
                .Include(e => e.FkForce)
                .Include(e => e.FkLevel)
                .Include(e => e.FkMainMuscleWorked)
                .Include(e => e.FkMechanicsType)
                .Include(e => e.FkOtherMuscleWorked)
                .Include(e => e.FkSport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsExerciseType == null)
            {
                return NotFound();
            }

            return View(edsExerciseType);
        }

        // GET: EdsExerciseTypes/Create
        public IActionResult Create()
        {
            ViewData["FkEquipmentId"] = new SelectList(_context.EdsEquipment, "Id", "Name");
            ViewData["FkExerciseClassId"] = new SelectList(_context.EdsExerciseClass, "Id", "Name");
            ViewData["FkForceId"] = new SelectList(_context.EdsForce, "Id", "Name");
            ViewData["FkLevelId"] = new SelectList(_context.EdsLevel, "Id", "Name");
            ViewData["FkMainMuscleWorkedId"] = new SelectList(_context.EdsMainMuscleWorked, "Id", "Name");
            ViewData["FkMechanicsTypeId"] = new SelectList(_context.EdsMechanicsType, "Id", "Name");
            ViewData["FkOtherMuscleWorkedId"] = new SelectList(_context.EdsOtherMuscleWorked, "Id", "Name");
            ViewData["FkSportId"] = new SelectList(_context.EdsSport, "Id", "Name");
            return View();
        }

        // POST: EdsExerciseTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FkExerciseClassId,FkMainMuscleWorkedId,FkOtherMuscleWorkedId,FkEquipmentId,FkMechanicsTypeId,FkLevelId,FkSportId,FkForceId,ExplainerVideoFr,ExplainerTextFr,HasSetDefaultTemplate,IsSetCollapsed,IsDeleted")] EdsExerciseType edsExerciseType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(edsExerciseType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkEquipmentId"] = new SelectList(_context.EdsEquipment, "Id", "Name", edsExerciseType.FkEquipmentId);
            ViewData["FkExerciseClassId"] = new SelectList(_context.EdsExerciseClass, "Id", "Name", edsExerciseType.FkExerciseClassId);
            ViewData["FkForceId"] = new SelectList(_context.EdsForce, "Id", "Name", edsExerciseType.FkForceId);
            ViewData["FkLevelId"] = new SelectList(_context.EdsLevel, "Id", "Name", edsExerciseType.FkLevelId);
            ViewData["FkMainMuscleWorkedId"] = new SelectList(_context.EdsMainMuscleWorked, "Id", "Name", edsExerciseType.FkMainMuscleWorkedId);
            ViewData["FkMechanicsTypeId"] = new SelectList(_context.EdsMechanicsType, "Id", "Name", edsExerciseType.FkMechanicsTypeId);
            ViewData["FkOtherMuscleWorkedId"] = new SelectList(_context.EdsOtherMuscleWorked, "Id", "Name", edsExerciseType.FkOtherMuscleWorkedId);
            ViewData["FkSportId"] = new SelectList(_context.EdsSport, "Id", "Name", edsExerciseType.FkSportId);
            return View(edsExerciseType);
        }

        // GET: EdsExerciseTypes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EdsExerciseType == null)
            {
                return NotFound();
            }

            var edsExerciseType = await _context.EdsExerciseType.FindAsync(id);
            if (edsExerciseType == null)
            {
                return NotFound();
            }
            ViewData["FkEquipmentId"] = new SelectList(_context.EdsEquipment, "Id", "Name", edsExerciseType.FkEquipmentId);
            ViewData["FkExerciseClassId"] = new SelectList(_context.EdsExerciseClass, "Id", "Name", edsExerciseType.FkExerciseClassId);
            ViewData["FkForceId"] = new SelectList(_context.EdsForce, "Id", "Name", edsExerciseType.FkForceId);
            ViewData["FkLevelId"] = new SelectList(_context.EdsLevel, "Id", "Name", edsExerciseType.FkLevelId);
            ViewData["FkMainMuscleWorkedId"] = new SelectList(_context.EdsMainMuscleWorked, "Id", "Name", edsExerciseType.FkMainMuscleWorkedId);
            ViewData["FkMechanicsTypeId"] = new SelectList(_context.EdsMechanicsType, "Id", "Name", edsExerciseType.FkMechanicsTypeId);
            ViewData["FkOtherMuscleWorkedId"] = new SelectList(_context.EdsOtherMuscleWorked, "Id", "Name", edsExerciseType.FkOtherMuscleWorkedId);
            ViewData["FkSportId"] = new SelectList(_context.EdsSport, "Id", "Name", edsExerciseType.FkSportId);
            return View(edsExerciseType);
        }

        // POST: EdsExerciseTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,FkExerciseClassId,FkMainMuscleWorkedId,FkOtherMuscleWorkedId,FkEquipmentId,FkMechanicsTypeId,FkLevelId,FkSportId,FkForceId,ExplainerVideoFr,ExplainerTextFr,HasSetDefaultTemplate,IsSetCollapsed,IsDeleted")] EdsExerciseType edsExerciseType)
        {
            if (id != edsExerciseType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(edsExerciseType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EdsExerciseTypeExists(edsExerciseType.Id))
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
            ViewData["FkEquipmentId"] = new SelectList(_context.EdsEquipment, "Id", "Name", edsExerciseType.FkEquipmentId);
            ViewData["FkExerciseClassId"] = new SelectList(_context.EdsExerciseClass, "Id", "Name", edsExerciseType.FkExerciseClassId);
            ViewData["FkForceId"] = new SelectList(_context.EdsForce, "Id", "Name", edsExerciseType.FkForceId);
            ViewData["FkLevelId"] = new SelectList(_context.EdsLevel, "Id", "Name", edsExerciseType.FkLevelId);
            ViewData["FkMainMuscleWorkedId"] = new SelectList(_context.EdsMainMuscleWorked, "Id", "Name", edsExerciseType.FkMainMuscleWorkedId);
            ViewData["FkMechanicsTypeId"] = new SelectList(_context.EdsMechanicsType, "Id", "Name", edsExerciseType.FkMechanicsTypeId);
            ViewData["FkOtherMuscleWorkedId"] = new SelectList(_context.EdsOtherMuscleWorked, "Id", "Name", edsExerciseType.FkOtherMuscleWorkedId);
            ViewData["FkSportId"] = new SelectList(_context.EdsSport, "Id", "Name", edsExerciseType.FkSportId);
            return View(edsExerciseType);
        }

        // GET: EdsExerciseTypes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EdsExerciseType == null)
            {
                return NotFound();
            }

            var edsExerciseType = await _context.EdsExerciseType
                .Include(e => e.FkEquipment)
                .Include(e => e.FkExerciseClass)
                .Include(e => e.FkForce)
                .Include(e => e.FkLevel)
                .Include(e => e.FkMainMuscleWorked)
                .Include(e => e.FkMechanicsType)
                .Include(e => e.FkOtherMuscleWorked)
                .Include(e => e.FkSport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (edsExerciseType == null)
            {
                return NotFound();
            }

            return View(edsExerciseType);
        }

        // POST: EdsExerciseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EdsExerciseType == null)
            {
                return Problem("Entity set 'testfitappContext.EdsExerciseType'  is null.");
            }
            var edsExerciseType = await _context.EdsExerciseType.FindAsync(id);
            if (edsExerciseType != null)
            {
                _context.EdsExerciseType.Remove(edsExerciseType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EdsExerciseTypeExists(long id)
        {
          return (_context.EdsExerciseType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
