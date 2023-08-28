using DAOLayer.Net7.Logs;
using DAOLayer.Net7.Supplement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParentMiddleWare.ApiModels;
using ParentMiddleWare.Models;


namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SupplementController : BaseController
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
        //public async Task<bool> TakeDose(long UserId,  long DoseId, float UnitCountActual, bool isCustomerAdded = false, bool isFreeEntry = false, string FreeEntryName = "")
        public async Task<bool> TakeDose([FromBody] GeneralApiModel model)
        {
            string FkFederatedUser = model.FkFederatedUser;
            long DoseId = model.longparam1;
            float UnitCountActual = model.floatparam1;
            bool isCustomerAdded = false;
            bool isFreeEntry = false;
            string FreeEntryName = "";
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            if (dose == null) return false;
            dose.UnitCountActual = UnitCountActual;
            dose.IsComplete = true;
            dose.CompletionTime = DateTime.Now;
            await _sContext.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("TakeDoseUndo")]
        public async Task<bool> TakeDoseUndo([FromBody]long DoseId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            if (dose == null) return false;
            dose.UnitCountActual = 0;
            dose.IsComplete = false;
            dose.CompletionTime = null;
            await _sContext.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("SnoozeDose")]
        public async Task<bool> SnoozeDose([FromBody] GeneralApiModel model)
        {
            long DoseId = model.longparam1;
            int MinutesSnoozed = model.intparam1;
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            if (dose == null) return false;
            dose.IsSnoozed = true;
            if (dose.SnoozedTime.HasValue)
            {
                dose.SnoozedTime = dose.SnoozedTime.Value.AddMinutes(MinutesSnoozed);
            }
            else if(dose.ScheduledTime.HasValue)
            {
                dose.SnoozedTime = dose.ScheduledTime.Value.AddMinutes(MinutesSnoozed);
            }
            await _sContext.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("UndoSnooze")]
        public async Task<bool> UndoSnooze([FromBody]long DoseId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            if (dose == null) return false;
            dose.IsSnoozed = false;
            dose.SnoozedTime = null;
            await _sContext.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("SkipSupplement")]
        public async Task<bool> SkipSupplement([FromBody] long DoseId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            if (dose == null) return false;
            dose.IsSnoozed = false;
            dose.SnoozedTime = null;
            dose.IsSkipped = true;
            await _sContext.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("UndoSkipSupplement")]
        //public async Task<bool> UndoSkipSupplement(long DoseId, int MinutesSnoozed)
        public async Task<bool> UndoSkipSupplement([FromBody] long DoseId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var dose = await _sContext.NdsSupplementScheduleDose.Where(t => t.Id == DoseId).FirstOrDefaultAsync();
            if (dose == null) return false;
            dose.IsSnoozed = false;
            dose.SnoozedTime = null;
            dose.IsSkipped = false;
            await _sContext.SaveChangesAsync();
            return true;
        }

        [HttpGet]
        [Route("GetAllSupplments")]
        public async Task<List<NdSupplementList>> GetAllSupplments(string FkFederatedUser)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var supp = await _sContext.NdsSupplementPlanWeekly.Where(t => t.FkCustomer.FkFederatedUser == FkFederatedUser && t.IsActive == true)
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
    }
}