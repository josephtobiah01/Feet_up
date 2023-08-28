using AutoMapper;
using DAOLayer.Net7.Supplement;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Classes.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ParentMiddleWare.ApiModels;
using System.Net;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    public class SupplementRepository
    {
        SupplementContext _dbcontext;
        LookupRepository _lookup;
        ClientRepository _clientrepo;
        ILogger<SupplementRepository> _logger;
        FitAppAPIUtil _apiutil;
        IConfiguration _config;
        IMapper _mapper;

        readonly string API_SUPP_SCHEDULE_UPDATE_TRIGGER = "/api/Automation/Supplements_Trigger_Schedule_Update";

        readonly string APPSETTINGKEY_MAINAPI_DOMAIN = "MainApi_Domain";
        readonly string MAINAPI_DOMAIN_DEFAULT = "https://fitapp-mainapi-test.azurewebsites.net";

        public SupplementRepository(SupplementContext dbcontext,
            LookupRepository lookupRepository, ClientRepository clientrepo,
            IMapper mapper, ILogger<SupplementRepository> logger, IConfiguration config, FitAppAPIUtil apiutil)
        {
            _dbcontext = dbcontext;
            _lookup = lookupRepository;
            _clientrepo = clientrepo;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _apiutil = apiutil;
        }

        public async Task<User?> GetUserByUserId(long userId)
        {
            return await _dbcontext.User.FirstOrDefaultAsync(r => r.Id == userId);
        }

        public async Task<List<User>> GetUsersList()
        {
            return await _dbcontext.User.Where(r => r.UserLevel > 0 && r.UserLevel <= 1000).ToListAsync();
        }

        public async Task<List<NdsSupplementPlanWeekly>> GetWeeklyPlansByUserId(long userId)
        {
            var query = _dbcontext.NdsSupplementPlanWeekly.Where(r => r.FkCustomerId == userId);
            return await query
                .Include(r => r.NdsSupplementPlanDaily)
                    .ThenInclude(r => r.NdsSupplementPlanSupplement)
                        .ThenInclude(r => r.NdsSupplementPlanDose)
                .Include(r => r.NdsSupplementPlanDaily)
                    .ThenInclude(r => r.NdsSupplementPlanSupplement)
                .ToListAsync();
        }

        public async Task<NdsSupplementPlanWeekly?> GetSupplementPlanById(long planId)
        {
            var query = _dbcontext.NdsSupplementPlanWeekly.Where(r => r.Id == planId)
                .Include(r => r.NdsSupplementPlanDaily)
                    .ThenInclude(r => r.NdsSupplementPlanSupplement)
                        .ThenInclude(r => r.NdsSupplementPlanDose)
                .Include(r => r.NdsSupplementPlanDaily)
                    .ThenInclude(r => r.NdsSupplementPlanSupplement);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<NdsSupplementPlanWeekly>> GenerateDummySupplementPlans(long userId)
        {
            Random r = new Random();
            int planCount = r.Next(5);

            var supplement_reference = await GetSupplementReferences();
            List<NdsSupplementPlanWeekly> list_plan = new List<NdsSupplementPlanWeekly>();
            
            for (var i = 0; i < planCount; i++)
            {
                var currPlan = new NdsSupplementPlanWeekly();
                currPlan.FkCustomerId = userId;
                currPlan.Remark = "This is a generated dummy plan";
                currPlan.Id = r.Next();
                currPlan.IsActive = r.Next() % 2 == 0;  
                if (currPlan.IsActive)
                {
                    currPlan.ActiveSince = DateTime.Now.AddDays(-1 * r.Next(7));
                }
                else
                {
                    currPlan.InactiveSince = DateTime.Now.AddDays(-1 * r.Next(7));
                }

                int dailyCount = r.Next(1, 7);
                for (var j = 0; j < dailyCount; j++)
                {
                    var dailyPlan = new NdsSupplementPlanDaily();
                    dailyPlan.Id = r.Next();
                    dailyPlan.DayOfWeek = (short) j;

                    int supplementId = r.Next(0, supplement_reference.Count);
                    //var supplement = supplement_reference[supplementId];
                    int supplementCount = r.Next(5);

                    for (var k = 0; k < supplementCount; k++)
                    {
                        var supp = new NdsSupplementPlanSupplement();
                        supp.Id = r.Next();
                        supp.Remark = "Test Remark";
                        supp.FkSupplementReference = supplementId;
                        supp.IsCustomerCreatedEntry = r.Next() % 2 == 0;

                        int doseCount = r.Next(5);

                        for (var l = 0; l < doseCount; l++)
                        {
                            var dose = new NdsSupplementPlanDose();
                            dose.Id = r.Next();
                            dose.UnitCount = r.Next(10);
                            dose.DoseWarningLimit = dose.UnitCount * 2;
                            dose.DoseHardCeilingLimit = dose.UnitCount * 3;
                            dose.ScheduledTime = DateTime.Now.Date.AddHours(r.Next(24)).TimeOfDay;
                            dose.Remark = "Nothing of note";
                            supp.NdsSupplementPlanDose.Add(dose);
                        }

                        dailyPlan.NdsSupplementPlanSupplement.Add(supp);
                    }
                    currPlan.NdsSupplementPlanDaily.Add(dailyPlan);
                }
                list_plan.Add(currPlan);
            }

            return list_plan;
        }

        public async Task<NdsSupplementReference?> GetSupplementReferenceById(long supprefId)
        {
            var query = _dbcontext.NdsSupplementReference.Where(r => r.Id == supprefId);
            return await query
                .Include(r => r.FkUnitMetricNavigation)
                .Include(r => r.FkSupplementInstructionNavigation)
                .FirstOrDefaultAsync();
        }

        public async Task<List<NdsSupplementReference>> GetSupplementReferenceByIdList(List<long> supprefIdList)
        {
            var query = _dbcontext.NdsSupplementReference.Where(r => supprefIdList.Contains(r.Id));
            return await query
                .Include(r => r.FkUnitMetricNavigation)
                .Include(r => r.FkSupplementInstructionNavigation)
                .ToListAsync();
        }

        public async Task<List<NdsSupplementReference>> GetSupplementReferences()
        {
            var query = _dbcontext.NdsSupplementReference
                .Include(r => r.FkUnitMetricNavigation)
                .Include(r => r.FkSupplementInstructionNavigation);
            return await query.ToListAsync();
        }

        public async Task<List<NdsSupplementReference>> GetSupplementReferenceIds()
        {
            return await _dbcontext.NdsSupplementReference.ToListAsync();
        }

        public async Task<List<NdsSupplementLegalStatus>> GetSupplementLegalStatusBySuppRefId(long suppRefId)
        {
            var query = _dbcontext.NdsSupplementLegalStatus.Where(r => r.FkSupplementReference == suppRefId)
                .Include(r => r.FkCountry)
                .Include(r => r.FkSupplementLegalStatusTypesNavigation);
            return await query.ToListAsync();
        }

        public async Task<List<NdsSupplementReference>> SearchSupplementReferenceByName(string name)
        {
            var query = _dbcontext.NdsSupplementReference.Where(r => r.Name.Contains(name) || r.Name.StartsWith(name) || r.Name.EndsWith(name));
            return await query.ToListAsync();
        }

        public async Task<NdsSupplementReference> AddSupplementReference(SupplementReference_DTO suppRef)
        {
            using (_logger.BeginScope("AddSupplementReference"))
            {
                try
                {
                    NdsSupplementReference inputRef = _mapper.Map<NdsSupplementReference>(suppRef);

                    //we remove the mapped unit metric if it is an already existing metric
                    //this is to prevent adding multiple entries in nds_unitmetric
                    if (inputRef.FkUnitMetricNavigation.Id != 0)
                    {
                        inputRef.FkUnitMetric = inputRef.FkUnitMetricNavigation.Id;
                        inputRef.FkUnitMetricNavigation = null;
                    }

                    _dbcontext.NdsSupplementReference.Add(inputRef);
                    await _dbcontext.SaveChangesAsync();

                    return inputRef;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed AddSupplementReference");
                    throw new Exception();
                }
            }
        }

        public async Task<List<NdsUnitMetric>> GetUnitMetricList()
        {
            return await _dbcontext.NdsUnitMetric.ToListAsync();
        }

        public async Task<NdsSupplementReference?> EditSupplementReference(SupplementReference_DTO suppRef)
        {
            using (_logger.BeginScope("EditSupplementReference"))
            {
                try
                {
                    NdsSupplementReference input = _mapper.Map<NdsSupplementReference>(suppRef);

                    NdsSupplementReference refToEdit = await _dbcontext.NdsSupplementReference
                        .Include(r => r.FkUnitMetricNavigation)
                        .Include(r => r.FkSupplementInstructionNavigation)
                        .FirstOrDefaultAsync(r => r.Id == input.Id);

                    if (refToEdit == null)
                    {
                        _logger.LogError("Cannot find supplement reference to edit. Id {Id}", suppRef.Id);
                        return null;
                    }

                    //map the new values to the retrieved entity
                    refToEdit.Name = input.Name;
                    refToEdit.InstructionText = input.InstructionText;

                    long input_unitMetricId = input.FkUnitMetricNavigation.Id;
                    if (input_unitMetricId == 0)
                    {
                        //add a new unit metric and assign it
                        input.FkUnitMetricNavigation.Id = 0;
                        refToEdit.FkUnitMetric = 0;
                        refToEdit.FkUnitMetricNavigation = input.FkUnitMetricNavigation;
                        
                        _dbcontext.NdsUnitMetric.Add(input.FkUnitMetricNavigation);
                    }
                    else
                    {
                        //query the new unit metric and assign it
                        refToEdit.FkUnitMetric = input.FkUnitMetricNavigation.Id;
                        refToEdit.FkUnitMetricNavigation = _dbcontext.NdsUnitMetric.FirstOrDefault(r => r.Id == input_unitMetricId);
                    }

                    //Edit Supplement Instruction
                    refToEdit.FkSupplementInstructionNavigation.Description = input.FkSupplementInstructionNavigation.Description;
                    refToEdit.FkSupplementInstructionNavigation.TakeAfterMeal = input.FkSupplementInstructionNavigation.TakeAfterMeal;
                    refToEdit.FkSupplementInstructionNavigation.TakeBeforeSleep = input.FkSupplementInstructionNavigation.TakeBeforeSleep;
                    refToEdit.FkSupplementInstructionNavigation.TakeOnEmptyStomach = input.FkSupplementInstructionNavigation.TakeOnEmptyStomach;
                    refToEdit.FkSupplementInstructionNavigation.RequiresSourceOfFat = input.FkSupplementInstructionNavigation.RequiresSourceOfFat;

                    _dbcontext.Entry(refToEdit).State = EntityState.Modified;
                    await _dbcontext.SaveChangesAsync();

                    return refToEdit;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed EditSupplementReference");
                    throw new Exception();
                }
            }
        }

        public async Task<NdsSupplementPlanWeekly?> EditSupplementPlanWeekly(SupplementPlanWeekly_DTO suppplan)
        {
            using (_logger.BeginScope("EditSupplementPlanWeekly"))
            {
                try
                {
                    NdsSupplementPlanWeekly currPlan = await GetSupplementPlanById(suppplan.Id);
                    if (currPlan == null)
                    {
                        _logger.LogError("Cannot find supplement plan id {id}", suppplan.Id);
                        return null;
                    }

                    currPlan.Remark = suppplan.Remark;
                    currPlan.IsActive = suppplan.IsActive;

                    //since plans do not contain data, we do not have to retain daily plans and below.
                    //we can simplify the edit process by clearing the old daily plans and replacing it with the new ones
                    foreach (var daily in currPlan.NdsSupplementPlanDaily)
                    {
                        foreach (var supp in daily.NdsSupplementPlanSupplement)
                        {
                            foreach (var dose in supp.NdsSupplementPlanDose)
                            {
                                _dbcontext.NdsSupplementPlanDose.Remove(dose);
                            }
                            _dbcontext.NdsSupplementPlanSupplement.Remove(supp);
                        }
                        _dbcontext.NdsSupplementPlanDaily.Remove(daily);
                    }

                    NdsSupplementPlanWeekly input = _mapper.Map<NdsSupplementPlanWeekly>(suppplan);

                    List<NdsSupplementPlanDaily> forDeletion = new List<NdsSupplementPlanDaily>();
                    //remove all daily entries without supplements
                    foreach (var daily in input.NdsSupplementPlanDaily)
                    {
                        if (daily.NdsSupplementPlanSupplement.Count == 0)
                        {
                            forDeletion.Add(daily);
                        }
                    }
                    foreach (var del in forDeletion)
                    {
                        input.NdsSupplementPlanDaily.Remove(del);
                    }

                    //enforce only 1 isActive
                    if (currPlan.IsActive)
                    {
                        var allPlans = await GetAllSupplementPlansWeeklyByUserId(currPlan.FkCustomerId);
                        foreach (var plans in allPlans)
                        {
                            plans.IsActive = false;
                        }
                        currPlan.IsActive = true;
                        currPlan.ActiveSince = DateTime.UtcNow;
                        currPlan.InactiveSince = null;
                    }
                    else
                    {
                        currPlan.InactiveSince = DateTime.UtcNow;
                        currPlan.ActiveSince = null;
                    }

                    currPlan.NdsSupplementPlanDaily = input.NdsSupplementPlanDaily;
                    await _dbcontext.SaveChangesAsync();

                    //trigger plan schedule automation
                    if (currPlan.IsActive && currPlan.ActiveSince.HasValue)
                    {
                        bool forced = suppplan.ForceScheduleSync;
                        bool triggerResult = await TriggerSupplementPlanScheduleAutomation(currPlan.Id, currPlan.ActiveSince.Value, forced);
                        if (triggerResult == false)
                        {
                            _logger.LogWarning("SupplementPlanScheduleAutomation returned false. Schedules are NOT synced!");
                        }
                    }

                    return currPlan;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to edit supplement plan id {id}", suppplan.Id);
                    return null;
                }
            }
        }

        public async Task<NdsSupplementPlanWeekly?> AddSupplementPlanWeekly(SupplementPlanWeekly_DTO suppplan)
        {
            using (_logger.BeginScope("AddSupplementPlanWeekly"))
            {
                try
                {
                    NdsSupplementPlanWeekly planToBeAdded = _mapper.Map<NdsSupplementPlanWeekly>(suppplan);

                    List<NdsSupplementPlanDaily> forDeletion = new List<NdsSupplementPlanDaily>();
                    //remove all daily entries without supplements
                    foreach (var daily in planToBeAdded.NdsSupplementPlanDaily)
                    {
                        if (daily.NdsSupplementPlanSupplement.Count == 0)
                        {
                            forDeletion.Add(daily);
                        }
                    }

                    foreach(var del in forDeletion)
                    {
                        planToBeAdded.NdsSupplementPlanDaily.Remove(del);
                    }

                    //enforce only 1 isActive
                    if (planToBeAdded.IsActive)
                    {
                        var allPlans = await GetAllSupplementPlansWeeklyByUserId(planToBeAdded.FkCustomerId);
                        foreach (var plans in allPlans)
                        {
                            plans.IsActive = false;
                        }
                        planToBeAdded.IsActive = true;
                        planToBeAdded.ActiveSince = DateTime.UtcNow;
                        planToBeAdded.InactiveSince = null;
                    }
                    else
                    {
                        planToBeAdded.InactiveSince = DateTime.UtcNow;
                        planToBeAdded.ActiveSince = null;
                    }

                    _dbcontext.NdsSupplementPlanWeekly.Add(planToBeAdded);
                    await _dbcontext.SaveChangesAsync();

                    //trigger plan schedule automation
                    if (planToBeAdded.IsActive && planToBeAdded.ActiveSince.HasValue)
                    {
                        bool forced = suppplan.ForceScheduleSync;
                        bool triggerResult = await TriggerSupplementPlanScheduleAutomation(planToBeAdded.Id, planToBeAdded.ActiveSince.Value, forced);
                        if (triggerResult == false)
                        {
                            _logger.LogWarning("SupplementPlanScheduleAutomation returned false. Schedules are NOT synced!");
                        }
                    }
                    
                    return planToBeAdded;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add supplement plan");
                    return null;
                }
            }
        }

        public async Task<List<NdsSupplementPlanWeekly>> GetAllSupplementPlansWeeklyByUserId(long userId)
        {
            return await _dbcontext.NdsSupplementPlanWeekly.Where(r => r.FkCustomerId == userId).ToListAsync();
        }

        public async Task<bool> TriggerSupplementPlanScheduleAutomation(long planId, DateTime startDate, bool force = false)
        {
            try
            {
                AutomationApiModel data = new AutomationApiModel()
                {
                    supplement_plan_weekly_id = planId,
                    datetimeparam1 = startDate,
                    boolparam1 = force
                };
                string dateString = WebUtility.UrlEncode(startDate.ToString("s"));
                var httpClient = _apiutil.GetHttpClient();
                var httpRequestMessage = _apiutil.BuildRequest(API_SUPP_SCHEDULE_UPDATE_TRIGGER, HttpMethod.Post, data);

                var response = await httpClient.SendAsync(httpRequestMessage);

                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                
                return responseString.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to trigger supplement plan schedule update for id {planId}", planId);
                return false;
            }
        }

        public async Task<List<NdsSupplementPlanDaily>?> GetPlanScheduleByPlanId(long planId)
        {
            NdsSupplementPlanWeekly currPlan = await GetSupplementPlanById(planId);
            if (currPlan == null)
            {
                _logger.LogError("Cannot find plan schedule for plan id {id}", planId);
                return null;
            }

            return currPlan.NdsSupplementPlanDaily.ToList();
        }

        public async Task<List<NdsSupplementPlanWeekly>> GetPlansListByUser(long userId)
        {
            return await _dbcontext.NdsSupplementPlanWeekly.Where(r => r.FkCustomerId == userId).ToListAsync();
        }

        public async Task<bool> DeleteSupplementPlanWeekly(long planId)
        {
            using (_logger.BeginScope("DeleteSupplementPlanWeekly"))
            {
                try
                {
                    NdsSupplementPlanWeekly currPlan = await GetSupplementPlanById(planId);
                    if (currPlan == null)
                    {
                        _logger.LogError("Cannot find plan schedule for plan id {id}", planId);
                        return false;
                    }

                    foreach (var daily in currPlan.NdsSupplementPlanDaily)
                    {
                        foreach (var supp in daily.NdsSupplementPlanSupplement)
                        {
                            foreach (var dose in supp.NdsSupplementPlanDose)
                            {
                                _dbcontext.NdsSupplementPlanDose.Remove(dose);
                            }
                            _dbcontext.NdsSupplementPlanSupplement.Remove(supp);
                        }
                        _dbcontext.NdsSupplementPlanDaily.Remove(daily);
                    }
                    _dbcontext.NdsSupplementPlanWeekly.Remove(currPlan);
                    await _dbcontext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cannot delete supplement plan weekly {id}", planId);
                    return false;
                }
            }            
        }
    }
}
