using DAOLayer.Net7.Logs;
using DAOLayer.Net7.Supplement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParentMiddleWare.Models;


namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SupplementController : ControllerBase
    {
        private readonly SupplementContext _sContext;
        private readonly LogsContext _lContext;

        public SupplementController(SupplementContext sContext, LogsContext lContext)
        {
            _sContext = sContext;
            _lContext = lContext;
        }


        [HttpPost]
        [Route("TakeDose")]
        public async Task<bool> TakeDose(long DoseId, float UnitCountActual, bool isCustomerAdded = false, bool isFreeEntry = false, string FreeEntryName = "")
        {
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            dose.UnitCountActual = UnitCountActual;
            dose.IsComplete = true;
            dose.CompletionTime = DateTime.Now;
            await _sContext.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("TakeDoseUndo")]
        public async Task<bool> TakeDoseUndo(long DoseId)
        {
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            dose.UnitCountActual = 0;
            dose.IsComplete = false;
            dose.CompletionTime = null;
            await _sContext.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("SnoozeDose")]
        public async Task<bool> SnoozeDose(long DoseId, int MinutesSnoozed)
        {
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            dose.IsSnoozed = true;
            if (dose.SnoozedTime.HasValue)
            {
                dose.SnoozedTime = dose.SnoozedTime.Value.AddMinutes(MinutesSnoozed);
            }
            else
            {
                dose.SnoozedTime = dose.ScheduledTime.Value.AddMinutes(MinutesSnoozed);
            }
            await _sContext.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("UndoSnooze")]
        public async Task<bool> UndoSnooze(long DoseId)
        {
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            dose.IsSnoozed = false;
            dose.SnoozedTime = null;
            await _sContext.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("UndoSkipSupplement")]
        public async Task<bool> UndoSkipSupplement(long DoseId, int MinutesSnoozed)
        {
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            dose.IsSnoozed = false;
            dose.SnoozedTime = null;
            dose.IsSkipped = false;
            await _sContext.SaveChangesAsync();
            return true;
        }

        [HttpGet]
        [Route("GetAllSupplments")]
        public async Task<List<NdSupplementList>> GetAllSupplments(long UserId)
        {
            var supp = await _sContext.NdsSupplementPlanWeekly.Where(t => t.FkCustomerId == UserId && t.IsActive == true)
                .Include(t => t.NdsSupplementPlanDaily)
                .ThenInclude(t => t.NdsSupplementPlanSupplement)
                .ThenInclude(t => t.FkSupplementReferenceNavigation)
                .ThenInclude(t => t.FkSupplementInstructionNavigation)


                .Include(t => t.NdsSupplementPlanDaily)
                .ThenInclude(t => t.NdsSupplementPlanSupplement)
                .ThenInclude(t => t.FkSupplementReferenceNavigation)
                .ThenInclude(t => t.FkUnitMetricNavigation)

                .Include(t => t.NdsSupplementPlanDaily)
                .ThenInclude(t => t.NdsSupplementPlanSupplement)
                .ThenInclude(t => t.NdsSupplementPlanDose)

                .AsNoTracking()
                .FirstOrDefaultAsync();

            List<NdSupplementList> lst = new List<NdSupplementList>();
            foreach (var sday in supp.NdsSupplementPlanDaily)
            {
                foreach (var ssupp in sday.NdsSupplementPlanSupplement)
                {
                    NdSupplementList su_1 = new NdSupplementList();
                    su_1.SupplementName = ssupp.FkSupplementReferenceNavigation.Name;
                    su_1.DayOfWeek = new List<ParentMiddleWare.Models.DayOfWeek>();
                    if (ssupp.FkSupplementReferenceNavigation.FkSupplementInstructionNavigation.TakeOnEmptyStomach)
                    {
                        su_1.TimeRemark = "Before Meal";
                    }
                    if (ssupp.FkSupplementReferenceNavigation.FkSupplementInstructionNavigation.TakeAfterMeal)
                    {
                        su_1.TimeRemark = "After Meal";
                    }
                    if (ssupp.FkSupplementReferenceNavigation.FkSupplementInstructionNavigation.RequiresSourceOfFat)
                    {
                        su_1.FoodRemark = "Take with fat";
                    }

                    su_1.Ammount = 0;
                    su_1.Frequency = "var per day";
                    su_1.Type = ssupp.FkSupplementReferenceNavigation.FkUnitMetricNavigation.Name;


                    foreach (var sdose in ssupp.NdsSupplementPlanDose)
                    {
                        su_1.DayOfWeek.Add((ParentMiddleWare.Models.DayOfWeek)(int)sday.DayOfWeek);
                        su_1.SupplmentDoseId = sdose.Id;
                        su_1.Ammount++;
                    }

                    if (su_1.Ammount > 0)
                    {
                        lst.Add(su_1);
                    }
                }
            }
            return lst;
        }



        //[HttpPost]
        //[Route("FavoriteDish")]
        //public async Task<bool> FavoriteDish(long recipeId)
        //{
        //    try
        //    {
        //        var dish = await _context.FnsNutritionActualDish.Where(t => t.Id == recipeId).FirstAsync();
        //        dish.IsFavorite = true;
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
}