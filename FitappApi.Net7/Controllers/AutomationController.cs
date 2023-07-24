using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Logs;
using DAOLayer.Net7.Nutrition;
using DAOLayer.Net7.Supplement;
using LogHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AutomationController : BaseController
    {

        private readonly ExerciseContext _context;
        private readonly NutritionContext _nContext;
        private readonly SupplementContext _sContext;
        private readonly LogsContext _lContext;

        public AutomationController(ExerciseContext context, NutritionContext nContext, SupplementContext sContext, LogsContext lContext)
        {
            _context = context;
            _nContext = nContext;
            _sContext = sContext;
            _lContext = lContext;
        }

        [HttpPost]
        [Route("Supplements_Trigger_Schedule_Update_All")]
        public async Task<bool> Supplements_Trigger_Schedule_Update_All(DateTime start_date, bool force = false)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                Logs llog =  sLogHelper.Log(LogHelper.Component.API,
                      this.GetType().Name,
                      System.Reflection.MethodBase.GetCurrentMethod().Name,
                      (int)LogHelper.Severity.Error,
                     "Called Supplements_Trigger_Schedule_Update_All"
                 );
                _lContext.Add(llog);
                await _lContext.SaveChangesAsync();

                var x = await _sContext.NdsSupplementPlanWeekly.Where(t => t.IsActive == true).ToListAsync();



                foreach (var y in x)
                {
                    try
                    {
                        await this.Supplements_Trigger_Schedule_Update(y.Id, start_date, force);
                    }
                    catch (Exception ex)
                    {
                        Logs llog2 = sLogHelper.Log(Component.API,
                       this.GetType().Name,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        (int)LogHelper.Severity.Error,
                        ex.Message + "ID: " + y.Id,
                        ex.StackTrace);
                        _lContext.Add(llog2);
                        await _lContext.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logs llog = sLogHelper.Log(LogHelper.Component.API,
                this.GetType().Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name,
                (int)LogHelper.Severity.Error,
                ex.Message,
                ex.StackTrace);
                _lContext.Add(llog);
                await _lContext.SaveChangesAsync();

                return false;
            }
        }

        [HttpPost]
        [Route("Supplements_Trigger_Schedule_Update")]
        public async Task<bool> Supplements_Trigger_Schedule_Update(long supplement_plan_weekly_id, DateTime start_date, bool force = false)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
             //   DbContextOptionsBuilder.EnableSensitiveDataLogging;

                var x = await _sContext.NdsSupplementPlanWeekly.Where(t => t.Id == supplement_plan_weekly_id)
                    .Include(t=>t.FkCustomer)
                    .Include(t => t.NdsSupplementPlanDaily)
                    .ThenInclude(t => t.NdsSupplementPlanSupplement)
                    .ThenInclude(t => t.NdsSupplementPlanDose)

                    .Include(t => t.NdsSupplementPlanDaily)
                    .ThenInclude(t => t.NdsSupplementPlanSupplement)
                    .ThenInclude(t => t.FkSupplementReferenceNavigation)

                   // .AsNoTracking()
                    .FirstAsync();

                foreach(var day in x.NdsSupplementPlanDaily)
                {
                    int DayOfWeek__start_date = netDayOfweek2DayOfweek(start_date.Date);
                    int DayOfWeek__Plan = day.DayOfWeek;



                    if (DayOfWeek__start_date == DayOfWeek__Plan && force == false)
                    {
                        continue;
                    }
                    if(DayOfWeek__start_date > DayOfWeek__Plan)
                    {
                        continue;
                    }

                        DateTime targetDate = start_date.Date.AddDays(DayOfWeek__Plan - DayOfWeek__start_date);

                    NdsSupplementSchedulePerDate s_date = await _sContext.NdsSupplementSchedulePerDate.Where(t => t.CustomerId == x.FkCustomerId && t.Date == targetDate)
                        .Include(t=>t.NdsSupplementSchedule)
                        .ThenInclude(t=>t.NdsSupplementScheduleDose)
                        .FirstOrDefaultAsync();
                    if (s_date == null)
                    {
                        s_date = new NdsSupplementSchedulePerDate()
                        {
                            Customer = x.FkCustomer,
                            Date = targetDate.Date,
                            NdsSupplementSchedule = new List<NdsSupplementSchedule>()
                        };
                        _sContext.NdsSupplementSchedulePerDate.Add(s_date);
                        await _sContext.SaveChangesAsync();
                    }
                    else
                    {
                        //// if (!force) continue;
                        // if(force)
                        // {
                        //   // do we need to delete old, or does linq do that already automatically?
                        //   // if double schedules show up, check
                        // }
                       
                        s_date.NdsSupplementSchedule.Clear();
                        await _sContext.SaveChangesAsync();
                    }
                        foreach(var plan_supplement in day.NdsSupplementPlanSupplement)
                        {
                            var s_schedule = new NdsSupplementSchedule()
                            {
                                FkFreeEntryUnitMetric = null,
                                FkSupplementReferenceNavigation = plan_supplement.FkSupplementReferenceNavigation,
                                FkSupplementSchedulePerDateNavigation = s_date,
                                FreeEntryInstructions = "",
                                IsCustomerCreatedEntry = false,
                                IsFreeEntry = false,
                                NdsSupplementScheduleDose = new List<NdsSupplementScheduleDose>()
                            };

                            foreach (var plan_dose in plan_supplement.NdsSupplementPlanDose)
                            {
                                var s_dose = new NdsSupplementScheduleDose()
                                {
                                    CompletionTime = null,
                                    CompletionTimeOffset = null,
                                    FkSupplementScheduleNavigation = s_schedule,
                                    IsComplete = false,
                                    IsSkipped = false,
                                    IsSnoozed = false,
                                    Remarks = null,
                                    ScheduledTime = targetDate.Add(plan_dose.ScheduledTime),
                                    SnoozedTime = null,
                                    SupplementSkipOtherReason = null,
                                    SupplementSkipReason = null,
                                    SupplementSkipReasonNavigation = null,
                                    UnitCountActual = null,
                                    UnitCountTarget = plan_dose.UnitCount
                                };
                                s_schedule.NdsSupplementScheduleDose.Add(s_dose);
                            }
                            s_date.NdsSupplementSchedule.Add(s_schedule);
                        }

                    //    _sContext.NdsSupplementSchedulePerDate.Add(s_date);
                        await _sContext.SaveChangesAsync();

                    //}
                    //else
                    //{
                    //    Logs llog = sLogHelper.Log(LogHelper.Component.API,
                    //    this.GetType().Name,
                    //    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    //    (int)LogHelper.Severity.Warning,
                    //    "NdsSupplementSchedulePerDate is not null",
                    //    "TargetDate" + targetDate.ToString());

                    //    _lContext.Add(llog);
                    //    await _lContext.SaveChangesAsync();
                    //}
                }
            }
            catch (Exception ex)
            {
                Logs llog = sLogHelper.Log(LogHelper.Component.API,
                    this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    (int)LogHelper.Severity.Error,
                    ex.Message,
                    ex.StackTrace);
                _lContext.Add(llog);
                await _lContext.SaveChangesAsync();
                return false;
            }

            return true;

        }

        public static short netDayOfweek2DayOfweek(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: return 0;
                case DayOfWeek.Tuesday: return 1;
                case DayOfWeek.Wednesday: return 2;
                case DayOfWeek.Thursday: return 3;
                case DayOfWeek.Friday: return 4;
                case DayOfWeek.Saturday: return 5;
                case DayOfWeek.Sunday: return 6;
                default: return 0;
            }
        }
    }
}
