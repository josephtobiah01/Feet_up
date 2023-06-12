using AutoMapper;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Models;
using DAOLayer.Net7.Exercise;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Security.Cryptography;
using System.Data.SqlTypes;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    /// <summary>
    /// Repository for all Training/Exercise data operations
    /// </summary>
    public class TrainingRepository
    {
        ExerciseContext _dbcontext;
        LookupRepository _lookup;
        ClientRepository _clientrepo;
        ILogger<TrainingRepository> _logger;
        IMapper _mapper;

        public static readonly long TEMPLATE_USER_ID = -10000; //User ID where we store templates in
        public static readonly string TEMPLATE_PLAN_NAME = "TemplatePlan";
        public static readonly long TEMPLATE_PLAN_ID = -10000;

        public TrainingRepository(ExerciseContext dbcontext, ILogger<TrainingRepository> logger, IMapper mapper, LookupRepository lookup, ClientRepository clientrepo)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _mapper = mapper;
            _lookup = lookup;
            _clientrepo = clientrepo;
        }

        public async Task<User?> GetClientById(long id)
        {
            User? user = await _dbcontext.User.Where(r => r.Id == id).FirstOrDefaultAsync();
            return user;
        }

        #region Get Operations
        public async Task<Eds12weekPlan?> GetProgramByUserId(long userid)
        {
            Eds12weekPlan? plan = await _dbcontext.Eds12weekPlan.Where(x => x.FkCustomerId == userid && x.IsCurrent).FirstOrDefaultAsync();
            return plan;
        }

        public async Task<Eds12weekPlan?> GetProgramById(long pid)
        {
            Eds12weekPlan? plan = await _dbcontext.Eds12weekPlan.Where(x => x.Id == pid).FirstOrDefaultAsync();
            return plan;
        }

        public async Task<List<Eds12weekPlan>> GetAllProgramsByUserId(long userId)
        {
            return await _dbcontext.Eds12weekPlan.Where(r => r.FkCustomerId == userId).ToListAsync();
        }

        public async Task<Eds12weekPlan?> GetCompleteProgramById(long pid)
        {
            var query = _dbcontext.Eds12weekPlan.Where(x => x.Id == pid)
                .Include(r => r.FkCustomer)
                .Include(r => r.EdsWeeklyPlan)
                    .ThenInclude(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                            .ThenInclude(r => r.EdsExercise)
                                .ThenInclude(r => r.EdsSet)
                                    .ThenInclude(r => r.EdsSetMetrics)
                                        .ThenInclude(r => r.FkMetricsType)
                .Include(r => r.EdsWeeklyPlan)
                    .ThenInclude(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                            .ThenInclude(r => r.EdsExercise)
                                .ThenInclude(r => r.FkExerciseType)
                .Include(r => r.EdsWeeklyPlan)
                    .ThenInclude(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                            .ThenInclude(r => r.ReasonForRescheduleNavigation)
                .Include(r => r.EdsWeeklyPlan)
                    .ThenInclude(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                            .ThenInclude(r => r.ReadonForSkippingNavigation);

            Eds12weekPlan result = await query.FirstOrDefaultAsync();
            return result;
        }

        public async Task<Eds12weekPlan?> GetCurrentPlanByUserId(long userId)
        {
            var query = _dbcontext.Eds12weekPlan.Where(x => x.FkCustomerId == userId && x.IsCurrent)
                .Include(r => r.FkCustomer)
                .Include(r => r.EdsWeeklyPlan)
                    .ThenInclude(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                            .ThenInclude(r => r.EdsExercise)
                                .ThenInclude(r => r.EdsSet)
                                    .ThenInclude(r => r.EdsSetMetrics)
                                        .ThenInclude(r => r.FkMetricsType);
            return await query.FirstOrDefaultAsync();
        }
             
        public async Task<List<Eds12weekPlan>> GetTemplateList(bool includeNonTemplates)
        {
            if (includeNonTemplates)
            {
                return await _dbcontext.Eds12weekPlan.OrderBy(r => r.Id).ToListAsync();
            }
            return await _dbcontext.Eds12weekPlan.Where(r => r.IsTemplate).ToListAsync();
        }

        public async Task<Eds12weekPlan?> GetTemplateProgramById(long pid, bool includeNonTemplate = false)
        {
            var query = from program in _dbcontext.Eds12weekPlan
                        where program.Id == pid &&
                              (includeNonTemplate ? true : program.IsTemplate)
                        select program;
            Eds12weekPlan template = await query
                .Include(r => r.EdsWeeklyPlan)
                    .ThenInclude(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                            .ThenInclude(r => r.EdsExercise)
                                .ThenInclude(r => r.EdsSet)
                                    .ThenInclude(r => r.EdsSetMetrics)
                .FirstOrDefaultAsync();
            return template;
        }

        public async Task<List<Eds12weekPlan>> GetAllProgramsForUser(long userId)
        {
            var plansquery = from plans in _dbcontext.Eds12weekPlan
                                where plans.FkCustomerId == userId && !plans.IsTemplate
                                orderby plans.IsCurrent
                                select plans;
            return await plansquery
                .Include(r => r.EdsWeeklyPlan)
                    .ThenInclude(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                .ToListAsync();
        }

        public async Task<List<EdsTrainingSession>> GetTemplateSessions()
        {
            var query = _dbcontext.EdsTrainingSession.Where(r => r.IsTemplate).OrderBy(r => r.Id);
            return await query.ToListAsync();
        }

        public async Task<List<EdsTrainingSession>> GetTemplateSessionsWithExercises()
        {
            var query = _dbcontext.EdsTrainingSession.Where(r => r.IsTemplate).OrderBy(r => r.Id)
                .Include(session => session.EdsExercise)
                    .ThenInclude(exercise => exercise.EdsSet)
                        .ThenInclude(set => set.EdsSetMetrics)
                            .ThenInclude(setmetric => setmetric.FkMetricsType)
                .Include(session => session.EdsExercise)
                    .ThenInclude(exercise => exercise.FkExerciseType)
                        .ThenInclude(exclass => exclass.FkExerciseClass);
            return await query.ToListAsync();
        }

        public async Task<List<EdsWeeklyPlan>> GetWeeklyPlansByProgramId(long programId)
        {
            var query = from program in _dbcontext.Eds12weekPlan
                        from week in _dbcontext.EdsWeeklyPlan
                        where program.Id == programId &&
                            week.FkEds12weekPlan == program.Id
                        select week;

            return await query.Include(r => r.EdsDailyPlan).ToListAsync();
        }

        public async Task<List<EdsWeeklyPlan>> GetWeeklyPlans(long userid)
        {
            var query = from program in _dbcontext.Eds12weekPlan
                        from week in _dbcontext.EdsWeeklyPlan
                        where program.FkCustomerId == userid &&
                            week.FkEds12weekPlan == program.Id
                        select week;
            return await query.ToListAsync();
        }

        public async Task<List<EdsDailyPlan>> GetDailyPlansByWeek(long weekid)
        {
            var query = from dailyplan in _dbcontext.EdsDailyPlan
                        where dailyplan.FkEdsWeeklyPlanId == weekid
                        select dailyplan;
            return await query.ToListAsync();
        }

        public async Task<EdsTrainingSession?> GetTrainingSessionById(long sessionId)
        {
            var query = from program in _dbcontext.EdsTrainingSession
                        where program.Id == sessionId
                        select program;
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Eds12weekPlan?> GetProgramOfTrainingSession(long sessionId)
        {
            var query = from programs in _dbcontext.Eds12weekPlan
                        from weekplans in _dbcontext.EdsWeeklyPlan
                        from dailyplans in _dbcontext.EdsDailyPlan
                        from sessions in _dbcontext.EdsTrainingSession
                        where sessions.Id == sessionId && sessions.FkEdsDailyPlan == dailyplans.Id &&
                            dailyplans.FkEdsWeeklyPlanId == weekplans.Id &&
                            weekplans.FkEds12weekPlan == programs.Id
                        select programs;
            return await query.FirstOrDefaultAsync();
        }

        public async Task<EdsTrainingSession?> GetTrainingSessionWithExercises(long sessionId)
        {
            var query = _dbcontext.EdsTrainingSession.Where(r => r.Id == sessionId);
            return await query
                .Include(r => r.EdsExercise)
                    .ThenInclude(r => r.EdsSet)
                        .ThenInclude(r => r.EdsSetMetrics)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EdsTrainingSession>> GetTrainingSessionsByDay(long dailyplanid)
        {
            var query = from session in _dbcontext.EdsTrainingSession
                        where session.FkEdsDailyPlan == dailyplanid
                        select session;

            var result = await query.
                Include(session => session.EdsExercise)
                .ThenInclude(exercise => exercise.EdsSet)
                .ToListAsync();

            return result;
        }

        public async Task<EdsDailyPlan?> GetDailyPlanByDate(long id, DateTime date)
        {
            var query = from program in _dbcontext.Eds12weekPlan
                            from weekplan in _dbcontext.EdsWeeklyPlan
                            from dailyplan in _dbcontext.EdsDailyPlan
                            where program.FkCustomerId == id && 
                                    weekplan.FkEds12weekPlan == program.Id && 
                                    dailyplan.FkEdsWeeklyPlanId == weekplan.Id && dailyplan.StartDay.HasValue && dailyplan.StartDay.GetValueOrDefault().Date == date.Date
                            select dailyplan;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<EdsTrainingSession>> GetTrainingSessionsByDate(long userId, DateTime date)
        {
            //get all training sessions by user id and target date
            var query = from programs in _dbcontext.Eds12weekPlan
                        from weekplans in _dbcontext.EdsWeeklyPlan
                        from dailyplans in _dbcontext.EdsDailyPlan
                        from sessions in _dbcontext.EdsTrainingSession
                        where programs.FkCustomerId == userId &&
                            weekplans.FkEds12weekPlan == programs.Id &&
                            dailyplans.FkEdsWeeklyPlanId == weekplans.Id && dailyplans.StartDay.HasValue && dailyplans.StartDay.Value.Date == date.Date &&
                            sessions.FkEdsDailyPlan == dailyplans.Id
                        select sessions;

            //include collections
            var result = await query
                .Include(session => session.EdsExercise)
                    .ThenInclude(exercise => exercise.EdsSet)
                        .ThenInclude(set => set.EdsSetMetrics)
                            .ThenInclude(setmetric => setmetric.FkMetricsType)
                .Include(session => session.EdsExercise)
                    .ThenInclude(exercise => exercise.FkExerciseType)
                        .ThenInclude(exclass => exclass.FkExerciseClass)
                .Include(session => session.ReasonForRescheduleNavigation)
                .Include(session => session.ReadonForSkippingNavigation)
                .ToListAsync();

            return result;
        }

        public async Task<List<EdsTrainingSession>> GetAllTrainingSessionsInProgram(long programId)
        {
            var query = from programs in _dbcontext.Eds12weekPlan
                        from weekplans in _dbcontext.EdsWeeklyPlan
                        from dailyplans in _dbcontext.EdsDailyPlan
                        from sessions in _dbcontext.EdsTrainingSession
                        where programs.Id == programId &&
                            weekplans.FkEds12weekPlan == programs.Id &&
                            dailyplans.FkEdsWeeklyPlanId == weekplans.Id &&
                            sessions.FkEdsDailyPlan == dailyplans.Id
                        select sessions;

            var result = await query
                .Include(session => session.EdsExercise)
                    .ThenInclude(exercise => exercise.EdsSet)
                        .ThenInclude(set => set.EdsSetMetrics)
                            .ThenInclude(setmetric => setmetric.FkMetricsType)
                .Include(session => session.EdsExercise)
                    .ThenInclude(exercise => exercise.FkExerciseType)
                        .ThenInclude(exclass => exclass.FkExerciseClass)
                .Include(session => session.ReadonForSkippingNavigation)
                .Include(session => session.ReasonForRescheduleNavigation)
                .ToListAsync();

            return result;
        }

        public async Task<List<EdsExercise>> GetExercisesByTrainingSessionId(long sessionId)
        {
            var query = _dbcontext.EdsExercise.Where(r => r.FkTrainingId == sessionId)
                .Include(session => session.EdsSet)
                    .ThenInclude(set => set.EdsSetMetrics)
                        .ThenInclude(setmetric => setmetric.FkMetricsType)
                .Include(session => session.FkExerciseType)
                    .ThenInclude(exclass => exclass.FkExerciseClass);
            return await query.ToListAsync();
        }
        #endregion

        #region Dummy Calls
        public async Task<List<EdsExercise>> GetDummyExercisesList()
        {
            List<EdsExerciseType> list_exerciseTypes = await _lookup.GetExerciseTypesAllData();
            List<EdsSetMetricTypes> list_metricTypes = await _lookup.GetSetMetricTypes();

            //vm.CurrentUser = new User()
            //{
            //    Id = 1,
            //    FirstName = "Cid",
            //    LastName = "Dummy",
            //    Email = "cidthedummy@test.com",
            //    Mobile = "0923 123 1231"
            //};

            //vm.CurrentProgram = new Eds12weekPlan()
            //{
            //    FkCustomerId = 1,
            //    Name = "Ultra-Slimfit Program",
            //    StartDate = DateTime.Now.Date,
            //    EndDate = DateTime.Now.Date.AddDays(7 * 12),
            //    DurationWeeks = 12,
            //    IsCurrent = true
            //};

            //vm.SelectedDate = DateTime.Now.Date;

            Random r = new Random();
            int maxExercises = 5;
            int maxSets = 5;
            int maxMetrics = 5;
            int numSessions = 1; //r.Next(10);

            List<EdsTrainingSession> sessionList = new List<EdsTrainingSession>();

            for (int i = 0; i < numSessions; i++)
            {
                EdsTrainingSession currSession = new EdsTrainingSession();
                currSession.Id = i;
                currSession.Name = $"Session #{i}";
                currSession.StartDateTime = DateTime.Now.AddHours(r.Next(3));
                currSession.EndDateTime = currSession.StartDateTime.Value.AddMinutes(r.Next(60));
                currSession.IsCustomerAddedTrainingSession = r.Next() % 2 == 0;
                currSession.IsSkipped = r.Next() % 2 == 0;

                int numExercises = r.Next(maxExercises);
                for (int j = 0; j < numExercises; j++)
                {
                    EdsExercise currExercise = new EdsExercise();
                    currExercise.Id = i * j;
                    //currExercise.FkExerciseType = list_exerciseTypes[r.Next(list_exerciseTypes.Count)];
                    currExercise.FkExerciseTypeId = list_exerciseTypes[r.Next(list_exerciseTypes.Count)].Id;
                    currExercise.IsComplete = r.Next() % 2 == 0;
                    currExercise.IsSkipped = !currExercise.IsComplete;
                    currExercise.IsCustomerAddedExercise = r.Next() % 2 == 0;

                    int numSets = r.Next(maxSets);
                    for (int k = 0; k < numSets; k++)
                    {
                        EdsSet currSet = new EdsSet()
                        {
                            Id = i * j * k,
                            SetSequenceNumber = (short)(k + 1),
                        };
                        currSet.IsComplete = r.Next() % 2 == 0;
                        currSet.IsSkipped = !currSet.IsComplete;
                        currSet.IsCustomerAddedSet = r.Next() % 2 == 0;

                        int numMetrics = r.Next(maxMetrics);
                        for (int x = 0; x < numMetrics; x++)
                        {
                            EdsSetMetrics currMetric = new EdsSetMetrics()
                            {
                                Id = k * x,
                                TargetCustomMetric = r.Next(32),
                                ActualCustomMetric = r.Next() % 2 == 0 ? r.Next(32) : null,
                                //FkMetricsType = list_metricTypes[r.Next(list_metricTypes.Count)],
                                FkMetricsTypeId = list_metricTypes[r.Next(list_metricTypes.Count)].Id
                            };
                            currSet.EdsSetMetrics.Add(currMetric);
                        }
                        currExercise.EdsSet.Add(currSet);
                    }
                    currSession.EdsExercise.Add(currExercise);
                }
                sessionList.Add(currSession);
            }

            return sessionList.First().EdsExercise.ToList();
        }
        #endregion

        #region Add/Edit Operations
        /// <summary>
        /// Ensure that a User where we can assign templates to exists. 
        /// This is needed for template functionality.
        /// </summary>
        /// <returns></returns>
        public async Task<User> EnsureTemplateUser()
        {
            using (_logger.BeginScope("EnsureTemplateUser"))
            {
                try
                {
                    var templateUser = await _dbcontext.User.Include(r => r.Eds12weekPlan).FirstOrDefaultAsync(r => r.UserLevel == TEMPLATE_USER_ID);
                    if (templateUser != null)
                    {
                        return templateUser;
                    }

                    //TemplateUser does not exist. Insert that user.
                    templateUser = new User()
                    {
                        FirstName = "n/a",
                        LastName = "n/a",
                        Email = "n/a",
                        UserLevel = (int)TEMPLATE_USER_ID,
                        FkFederatedUser = "n/a",
                        Mobile = "n/a"
                    };

                    var plan = await _dbcontext.Eds12weekPlan.FirstOrDefaultAsync(r => r.Name == TEMPLATE_PLAN_NAME);
                    if (plan == null)
                    {
                        //Create a eds plan under this user
                        plan = new Eds12weekPlan()
                        {
                            StartDate = ((DateTime)SqlDateTime.MinValue),
                            EndDate = ((DateTime)SqlDateTime.MaxValue),
                            Name = TEMPLATE_PLAN_NAME,
                            DurationWeeks = 0,
                            FkCustomer = templateUser,
                            IsCurrent = true
                        };
                        templateUser.Eds12weekPlan.Add(plan);
                        _dbcontext.Eds12weekPlan.Add(plan);
                    }

                    _dbcontext.User.Add(templateUser);

                    await _dbcontext.SaveChangesAsync();

                    return templateUser;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Critical: Cannot save a user to store templates. This will break all template functionality!");
                    throw;
                }
            }
        }

        public async Task<Eds12weekPlan> EnsureTemplate12WeekPlan()
        {
            using (_logger.BeginScope("EnsureTemplate12WeekPlan"))
            {
                var templateUser = await EnsureTemplateUser();

                var plan = await _dbcontext.Eds12weekPlan.FirstOrDefaultAsync(r => r.Name == TEMPLATE_PLAN_NAME);
                if (plan == null)
                {
                    //Create a eds plan under this user
                    plan = new Eds12weekPlan()
                    {
                        StartDate = ((DateTime)SqlDateTime.MinValue),
                        EndDate = ((DateTime)SqlDateTime.MaxValue),
                        Name = TEMPLATE_PLAN_NAME,
                        DurationWeeks = 0,
                        FkCustomer = templateUser,
                        IsCurrent = true
                    };
                    templateUser.Eds12weekPlan.Add(plan);
                    _dbcontext.Eds12weekPlan.Add(plan);
                    await _dbcontext.SaveChangesAsync();
                }

                return plan;
            }
        }

        public async Task<Eds12weekPlan?> AddProgram(ProgramDTO prog)
        {
            using (_logger.BeginScope("AddProgram"))
            {
                try
                {
                    Eds12weekPlan programToBeAdded = _mapper.Map<Eds12weekPlan>(prog);

                    //Add EndDay to all DailyPlans to be added
                    foreach (var week in programToBeAdded.EdsWeeklyPlan)
                    {
                        foreach (var day in week.EdsDailyPlan)
                        {
                            if (day.StartDay.HasValue)
                            {
                                day.EndDay = day.StartDay.Value.AddDays(1).AddMinutes(-1);
                            }
                        }
                    }

                    //calculate number of weeks based on startdate/enddate
                    var ts_datediff = programToBeAdded.EndDate - programToBeAdded.StartDate;
                    if (ts_datediff.HasValue) 
                    {
                        short numWeeks = (short) Math.Ceiling((decimal)ts_datediff.Value.TotalDays / 7);
                        programToBeAdded.DurationWeeks = numWeeks;
                    }

                    //set PLan StartDate and EndDate
                    programToBeAdded.StartDate = programToBeAdded.StartDate.GetValueOrDefault().Date; //remove time component
                    programToBeAdded.EndDate = programToBeAdded.StartDate.GetValueOrDefault().AddDays(12 * 7).AddSeconds(-1);

                    //if (programToBeAdded.IsTemplate) 
                    //{
                    //    //if plan is a template, set current to false and reassign this plan to the templateuser
                    //    programToBeAdded.IsCurrent = false;
                    //    var templateUser = await EnsureTemplateUser();
                    //    programToBeAdded.FkCustomer = templateUser;
                    //    programToBeAdded.FkCustomerId = templateUser.Id;
                    //}
                    //else 
                    //{
                        //assign user
                        User user = await GetClientById(prog.UserId);
                        if (user != null)
                        {
                            programToBeAdded.FkCustomer = user;
                            programToBeAdded.FkCustomerId = user.Id;
                        }

                        //set all other plans to not current if isCurrent
                        if (programToBeAdded.IsCurrent)
                        {
                            var allPlans = await GetAllProgramsByUserId(user.Id);
                            foreach (var plans in allPlans)
                            {
                                plans.IsCurrent = false;
                            }
                            programToBeAdded.IsCurrent = true;
                        }
                    //}

                    //TODO: if loaded from a template, place code here to load exercises onto the new plan items

                    _dbcontext.Add(programToBeAdded);

                    await _dbcontext.SaveChangesAsync();
                    return programToBeAdded;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to Add 12 week plan");
                    return null;
                }
            }
        }

        public async Task<bool> DeleteProgram(long id)
        {
            using (_logger.BeginScope("DeleteProgram"))
            {
                try
                {
                    var planQuery = _dbcontext.Eds12weekPlan.Where(r => r.Id == id)
                                        .Include(r => r.EdsWeeklyPlan)
                                            .ThenInclude(r => r.EdsDailyPlan)
                                                .ThenInclude(r => r.EdsTrainingSession)
                                                    .ThenInclude(r => r.EdsExercise)
                                                        .ThenInclude(r => r.EdsSet)
                                                            .ThenInclude(r => r.EdsSetMetrics);

                    Eds12weekPlan planToDelete = await planQuery.FirstOrDefaultAsync();
                    if (planToDelete == null)
                    {
                        _logger.LogError("Cannot find plan to delete with id {id}", id);
                        return false;
                    }

                    //cascade delete every child (NOTE: Consider asking to configure the cascade delete SQL-side instead of this)
                    //This will delete everything from daily plans to set metrics!!
                    foreach (var week in planToDelete.EdsWeeklyPlan)
                    {
                        foreach (var day in week.EdsDailyPlan)
                        {
                            foreach (var session in day.EdsTrainingSession)
                            {
                                foreach (var exercise in session.EdsExercise)
                                {
                                    foreach (var set in exercise.EdsSet)
                                    {
                                        foreach (var metric in set.EdsSetMetrics)
                                        {
                                            _dbcontext.Remove(metric);
                                        }
                                        _dbcontext.Remove(set);
                                    }
                                    _dbcontext.Remove(exercise);
                                }
                                _dbcontext.Remove(session);
                            }
                            _dbcontext.Remove(day);
                        }
                        _dbcontext.Remove(week);
                    }
                    _dbcontext.Remove(planToDelete);

                    int affectedRows = await _dbcontext.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cannot delete plan id {id}", id);
                    return false;
                }
            }
        }

        public async Task<Eds12weekPlan?> EditProgram(ProgramDTO prog)
        {
            using (_logger.BeginScope("EditProgram"))
            {
                try
                {
                    Eds12weekPlan programToBeEdited = await GetProgramById(prog.Id);
                    if (programToBeEdited == null)
                    {
                        _logger.LogError("Cannot find 12 week plan ID {id} to edit.", prog.Id);
                        return null;
                    }

                    programToBeEdited.StartDate = prog.StartDate;
                    programToBeEdited.EndDate = prog.EndDate;
                    programToBeEdited.Name = prog.Name;
                    programToBeEdited.IsCurrent = prog.IsCurrent;
                    programToBeEdited.IsTemplate = prog.IsTemplate;

                    //set all other plans to not current if isCurrent
                    if (programToBeEdited.IsCurrent)
                    {
                        var allPlans = await GetAllProgramsByUserId(programToBeEdited.FkCustomerId.Value);
                        foreach (var plans in allPlans)
                        {
                            plans.IsCurrent = false;
                        }
                        programToBeEdited.IsCurrent = true;
                    }                  

                    await _dbcontext.SaveChangesAsync();

                    return programToBeEdited;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to edit program id {id}", prog.Id);
                    return null;
                }
            }
        }

        public async Task<List<EdsWeeklyPlan>> LoadTemplateSchedule(long planId, bool includeNonTemplate = false)
        {
            using (_logger.BeginScope("LoadTemplateSchedule"))
            {
                var planquery = from program in _dbcontext.Eds12weekPlan
                                where program.Id == planId &&
                                      (includeNonTemplate ? true : program.IsTemplate)
                                select program;
                Eds12weekPlan? template = await planquery
                    .Include(r => r.EdsWeeklyPlan)
                        .ThenInclude(r => r.EdsDailyPlan)
                    .FirstOrDefaultAsync();

                return template != null ? template.EdsWeeklyPlan.ToList(): new List<EdsWeeklyPlan>();
            }
        }

        public async Task<Eds12weekPlan?> CreatePlanFromTemplate(long templatePlanId, ProgramDTO program, bool includeNonTemplate = false)
        {
            using (_logger.BeginScope("CreatePlanFromTemplate"))
            {
                try
                {
                    Eds12weekPlan programToBeAdded = _mapper.Map<Eds12weekPlan>(program);
                    //discard all weekly/daily plans in input if it exists. We take those from template
                    programToBeAdded.DurationWeeks = 12;
                    programToBeAdded.EdsWeeklyPlan.Clear();

                    //load template plan id
                    Eds12weekPlan? template = await GetTemplateProgramById(templatePlanId, includeNonTemplate);
                    if (template == null)
                    {
                        _logger.LogError("Cannot find template id {id}", templatePlanId);
                        return null;
                    }

                    //copy and map template entities to new 12 week plan
                    foreach (var weekly in template.EdsWeeklyPlan)
                    {
                        var week_startDateDiff = (weekly.StartDate.GetValueOrDefault() - template.StartDate.GetValueOrDefault());
                        EdsWeeklyPlan weeklyplan = new EdsWeeklyPlan();
                        weeklyplan.StartDate = programToBeAdded.StartDate.GetValueOrDefault().AddDays(Math.Abs(week_startDateDiff.Days)).Date;
                        weeklyplan.EndDate = weeklyplan.StartDate.GetValueOrDefault().AddDays(7).Date.AddMinutes(-1);

                        //add daily plans from template
                        foreach (var daily in weekly.EdsDailyPlan)
                        {
                            var day_startDateDiff = (daily.StartDay.GetValueOrDefault() - weekly.StartDate.GetValueOrDefault());
                            EdsDailyPlan dailyplan = new EdsDailyPlan();
                            dailyplan.StartDay = weeklyplan.StartDate.GetValueOrDefault().Date.AddDays(Math.Abs(day_startDateDiff.Days)).Date;
                            dailyplan.EndDay = dailyplan.StartDay.GetValueOrDefault().AddDays(1).AddMinutes(-1);

                            //add Training Sessions from template
                            foreach (var session in daily.EdsTrainingSession)
                            {
                                EdsTrainingSession trainingSession = _mapper.Map<EdsTrainingSession>(session);
                                var starttime_ts = session.StartDateTime.GetValueOrDefault().TimeOfDay;
                                var endtime_ts = session.EndDateTime.GetValueOrDefault().TimeOfDay;

                                //clean up mapped entities for adding
                                trainingSession.Id = 0;
                                trainingSession.StartDateTime = dailyplan.StartDay.GetValueOrDefault().Add(starttime_ts);
                                trainingSession.EndDateTime = dailyplan.StartDay.GetValueOrDefault().Add(endtime_ts);
                                trainingSession.IsMoved = trainingSession.IsSkipped = trainingSession.IsCustomerAddedTrainingSession = false;
                                trainingSession.StartTimestamp = trainingSession.EndTimeStamp = null;
                                trainingSession.TimeOffset = null;

                                foreach (var ex in trainingSession.EdsExercise)
                                {
                                    //EdsExercise exercise = new EdsExercise(); //_mapper.Map<EdsExercise>(ex);
                                    ex.Id = 0;
                                    ex.IsComplete = ex.IsCustomerAddedExercise = ex.IsSkipped = false;
                                    ex.EndTimeStamp = null;
                                    ex.TimeOffset = null;

                                    foreach (var set in ex.EdsSet)
                                    {
                                        set.Id = 0;
                                        set.TimeOffset = null;
                                        set.IsComplete = set.IsCustomerAddedSet = set.IsSkipped = false;
                                        set.EndTimeStamp = null;

                                        foreach (var metric in set.EdsSetMetrics)
                                        {
                                            metric.Id = 0;
                                            metric.ActualCustomMetric = null;
                                        }
                                    }
                                }

                                dailyplan.EdsTrainingSession.Add(trainingSession);
                            }
                            weeklyplan.EdsDailyPlan.Add(dailyplan);
                        }
                        programToBeAdded.EdsWeeklyPlan.Add(weeklyplan);
                    }

                    //if (programToBeAdded.IsTemplate)
                    //{
                    //    //if plan is a template, set current to false and reassign this plan to the templateuser
                    //    programToBeAdded.IsCurrent = false;
                    //    var templateUser = await EnsureTemplateUser();
                    //    programToBeAdded.FkCustomer = templateUser;
                    //    programToBeAdded.FkCustomerId = templateUser.Id;
                    //}
                    //else
                    //{
                        //assign user
                        User user = await GetClientById(program.UserId);
                        if (user != null)
                        {
                            programToBeAdded.FkCustomer = user;
                            programToBeAdded.FkCustomerId = user.Id;
                        }

                        //set all other plans to not current
                        if (programToBeAdded.IsCurrent)
                        {
                            var allPlans = await GetAllProgramsByUserId(user.Id);
                            foreach (var plans in allPlans)
                            {
                                plans.IsCurrent = false;
                            }
                        }
                    //}

                    _dbcontext.Add(programToBeAdded);

                    int rowsAffected = await _dbcontext.SaveChangesAsync();

                    return programToBeAdded;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create 12 week plan from template id {id}", templatePlanId);
                    return null;
                }
            }
        }

        public async Task<List<Eds12weekPlan>> GetTemplateTrainingPlans()
        {
            return await _dbcontext.Eds12weekPlan.Where(r => r.IsTemplate == true).ToListAsync();
        }

        public async Task<EdsTrainingSession?> AddTrainingSession(long userId, long program_id, TrainingSessionDTO tsession)
        {
            using (_logger.BeginScope("AddTrainingSession")) 
            {
                try
                {
                    EdsTrainingSession sessionToBeAdded = _mapper.Map<EdsTrainingSession>(tsession);

                    //modify parameters when adding a session with isTemplate true
                    if (sessionToBeAdded.IsTemplate)
                    {
                        var templateUser = await EnsureTemplateUser();
                        var plan = templateUser.Eds12weekPlan.FirstOrDefault(r => r.Name == TEMPLATE_PLAN_NAME);

                        if (templateUser == null || plan == null)
                        {
                            throw new InvalidOperationException("Failed to ensure Template User!");
                        }

                        program_id = plan.Id;
                        userId = templateUser.Id;
                        sessionToBeAdded.StartDateTime = DateTime.UtcNow;
                        sessionToBeAdded.EndDateTime = DateTime.UtcNow.AddMinutes(1);
                    }

                    EdsDailyPlan? currDailyPlan = await EnsureDailyPlanForDay(userId, program_id, sessionToBeAdded.StartDateTime.Value);
                    if (currDailyPlan == null)
                    {
                        _logger.LogError("Cannot find daily plan");
                        return null;
                    }
                    currDailyPlan.EdsTrainingSession.Add(sessionToBeAdded);

                    await _dbcontext.SaveChangesAsync();

                    return sessionToBeAdded;
                }
                catch (DataException dex)
                {
                    _logger.LogError(dex, "Data Exception in AddTrainingSession");
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to Add Training Session.");
                    return null;
                }
            }
        }

        /// <summary>
        /// This method will try to ensure that a daily plan will be available for a certain date for a user
        /// as long as the day is within a 12-week program.
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private async Task<EdsDailyPlan?> EnsureDailyPlanForDay(long userId, long programId, DateTime day)
        {
            try
            {
                //check if a daily plan is already there. If yes, return that plan immediately
                var dailyplanquery = await (from daily in _dbcontext.EdsDailyPlan
                                        from program in _dbcontext.Eds12weekPlan
                                        from weekly in _dbcontext.EdsWeeklyPlan
                                        where program.Id == programId && program.FkCustomerId == userId && !program.IsTemplate &&
                                        weekly.FkEds12weekPlan == programId &&
                                        daily.FkEdsWeeklyPlanId == weekly.Id && !daily.IsComplete && 
                                        daily.StartDay != null && daily.StartDay.Value == day.Date
                                        select daily).ToListAsync();
                if (dailyplanquery.Count > 0)
                {
                    //CHECK: how do we prioritize multiple daily plans from different programs?
                    return dailyplanquery.FirstOrDefault();
                }

                //if no daily plan is found, check if a 12-week program and weekly plan exists for the day and
                //insert a daily plan there
                var weeklyplanquery = _dbcontext.EdsWeeklyPlan.Where(r => 
                    r.FkEds12weekPlan == programId && !r.FkEds12weekPlanNavigation.IsTemplate &&
                    r.StartDate <= day.Date && day.Date <= r.EndDate);

                EdsWeeklyPlan? selectedPlan = await weeklyplanquery.FirstOrDefaultAsync();
                if (selectedPlan == null)
                {
                    //Adds a new weekly + daily plan for the exercise
                    _logger.LogWarning("No weekly plan is available for user {userId} at day {day}. Adding to accomodate..", userId, day);

                    //Get the last monday (start of the week) of the week encompassing the target day
                    DateTime startOfWeek = day.Date.AddDays(-(int)day.Date.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                    EdsWeeklyPlan weeklyplan = new EdsWeeklyPlan()
                    {
                        StartDate = startOfWeek,
                        EndDate = endOfWeek
                    };

                    Eds12weekPlan program = await _dbcontext.Eds12weekPlan.FirstOrDefaultAsync(r => r.Id == programId);
                    if (program == null)
                    {
                        _logger.LogError("Cannot find 12-week program of ID {id}", programId);
                        return null;
                    }

                    //add new daily plan
                    EdsDailyPlan newDailyPlan = new EdsDailyPlan()
                    {
                        StartDay = day.Date,
                        EndDay = day.Date.AddDays(1).AddSeconds(-1),
                        IsComplete = false
                    };
                    weeklyplan.EdsDailyPlan.Add(newDailyPlan);
                    
                    //add weekly plan to program
                    program.EdsWeeklyPlan.Add(weeklyplan);

                    return newDailyPlan;
                }
                else
                {
                    //insert new daily plan in seleced weekly plan
                    EdsDailyPlan newDailyPlan = new EdsDailyPlan()
                    {
                        StartDay = day.Date,
                        EndDay = day.Date.AddDays(1).AddSeconds(-1),
                        FkEdsWeeklyPlanId = selectedPlan.Id,
                        IsComplete = false
                    };
                    selectedPlan.EdsDailyPlan.Add(newDailyPlan);

                    return newDailyPlan;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create/retrieve daily plan for {day}", day);
                return null;
            }
        }

        public async Task<EdsTrainingSession?> EditTrainingSession(long userId, long program_id, TrainingSessionDTO trainingSessionDTO)
        {
            //trainingSessionDTO should contain everything (all exercises, all sets)
            //method will, for now, overwrite everything with the same id

            using (_logger.BeginScope("EditTrainingSession"))
            {
                try
                {
                    //retrieve from context
                    EdsTrainingSession currSession = await GetTrainingSessionWithExercises(trainingSessionDTO.TrainingSessionID);
                    if (currSession == null)
                    {
                        _logger.LogError("Cannot find TrainingSession ID {id} to edit", trainingSessionDTO.TrainingSessionID);
                        return null;
                    }

                    List<EdsSetMetricTypes> setmetrictypes = await _lookup.GetSetMetricTypes();
                    List<EdsExerciseType> exercisetypes = await _lookup.GetExerciseTypes();

                    EdsTrainingSession input = _mapper.Map<EdsTrainingSession>(trainingSessionDTO);
                    currSession.Name = input.Name;
                    currSession.Description = input.Description;
                    currSession.StartDateTime = input.StartDateTime;
                    currSession.EndDateTime = input.EndDateTime;
                    foreach (var in_exercise in input.EdsExercise)
                    {
                        if (in_exercise.Id == 0)
                        {
                            //exercise does not exist in db. Adding...
                            currSession.EdsExercise.Add(in_exercise);
                            continue;
                        }

                        EdsExercise currExercise = currSession.EdsExercise.FirstOrDefault(r => r.Id == in_exercise.Id);
                        if (currExercise != null)
                        {
                            //Edit exercise fields, check sets
                            currExercise.FkExerciseType = exercisetypes.First(r => r.Id == in_exercise.FkExerciseTypeId);
                            currExercise.FkExerciseTypeId = in_exercise.FkExerciseTypeId;
                            foreach (var in_set in in_exercise.EdsSet)
                            {
                                if (in_set.Id == 0)
                                {
                                    //set does not exist in db. Adding...
                                    currExercise.EdsSet.Add(in_set);
                                    continue;
                                }

                                EdsSet currSet = currExercise.EdsSet.FirstOrDefault(r => r.Id == in_set.Id);
                                if (currSet != null)
                                {
                                    //Edit set fields, check metrics
                                    currSet.SetSequenceNumber = in_set.SetSequenceNumber;
                                    foreach (var in_metric in in_set.EdsSetMetrics)
                                    {
                                        if (in_metric.Id == 0)
                                        {
                                            //set metric does not exist in db. Adding...
                                            currSet.EdsSetMetrics.Add(in_metric);
                                            continue;
                                        }

                                        EdsSetMetrics currMetric = currSet.EdsSetMetrics.FirstOrDefault(r => r.Id == in_metric.Id);
                                        if (currMetric != null)
                                        {
                                            //Edit metric fields
                                            currMetric.TargetCustomMetric = in_metric.TargetCustomMetric;
                                            currMetric.FkMetricsType = setmetrictypes.First(r => r.Id == in_metric.FkMetricsTypeId);
                                            currMetric.FkMetricsTypeId = in_metric.FkMetricsTypeId;
                                        }
                                    }
                                    //remove non-existing metrics
                                    List<long> metricsIdToRemove = currSet.EdsSetMetrics.Select(r => r.Id).Except(in_set.EdsSetMetrics.Select(s => s.Id)).ToList();
                                    List<EdsSetMetrics> metricsToRemove = currSet.EdsSetMetrics.Where(r => metricsIdToRemove.Contains(r.Id)).ToList();
                                    foreach (var m in metricsToRemove)
                                    {
                                        currSet.EdsSetMetrics.Remove(m);
                                    }
                                }
                            }
                            //remove non-existing sets
                            List<long> setsIdToRemove = currExercise.EdsSet.Select(r => r.Id).Except(in_exercise.EdsSet.Select(s => s.Id)).ToList();
                            List<EdsSet> setsToRemove = currExercise.EdsSet.Where(r => setsIdToRemove.Contains(r.Id)).ToList();
                            foreach (var m in setsToRemove)
                            {
                                m.EdsSetMetrics.Clear();
                                currExercise.EdsSet.Remove(m);
                            }
                        }                    
                    }
                    //remove non-existing exercises
                    List<long> exerciseIdToRemove = currSession.EdsExercise.Select(r => r.Id).Except(input.EdsExercise.Select(s => s.Id)).ToList();
                    List<EdsExercise> exercisesToRemove = currSession.EdsExercise.Where(r => exerciseIdToRemove.Contains(r.Id)).ToList();
                    foreach (var m in exercisesToRemove)
                    {
                        foreach (var set in m.EdsSet)
                        {
                            set.EdsSetMetrics.Clear();
                        }
                        m.EdsSet.Clear();
                        
                        currSession.EdsExercise.Remove(m);
                        _dbcontext.Remove(m);
                    }

                    await _dbcontext.SaveChangesAsync();

                    return currSession;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to Edit Training Session ID {id}", trainingSessionDTO.TrainingSessionID);
                    return null;
                }
            }
        }

        public async Task<bool> DeleteTrainingSession(long trainingsessionID)
        {
            using (_logger.BeginScope("Delete Training Session"))
            {
                //TODO: Enforce certain restrictions to training session deletion
                EdsTrainingSession? targetSession = await GetTrainingSessionWithExercises(trainingsessionID);
                if (targetSession == null)
                {
                    _logger.LogError("Cannot delete. Cannot find training session with id {id}", trainingsessionID);
                    return false;
                }

                //cascade delete children
                foreach (var exercise in targetSession.EdsExercise)
                {
                    foreach (var set in exercise.EdsSet)
                    {
                        foreach (var metric in set.EdsSetMetrics)
                        {
                            _dbcontext.Remove(metric);
                        }
                        _dbcontext.Remove(set);
                    }
                    _dbcontext.Remove(exercise);
                }
                _dbcontext.Remove(targetSession);

                int affectedRows = await _dbcontext.SaveChangesAsync();
                return affectedRows > 0;
            }
        }

        public async Task<EdsWeeklyPlan?> CopyWeek(long weekid, DateTime startdate)
        {
            using (_logger.BeginScope("CopyWeek"))
            {
                EdsWeeklyPlan currWeek = await _dbcontext.EdsWeeklyPlan.Where(r => r.Id == weekid)
                    .Include(r => r.EdsDailyPlan)
                        .ThenInclude(r => r.EdsTrainingSession)
                            .ThenInclude(r => r.EdsExercise)
                                .ThenInclude(r => r.EdsSet)
                                    .ThenInclude(r => r.EdsSetMetrics)
                    .FirstOrDefaultAsync();

                //check if copied week exists
                if (currWeek == null)
                {
                    _logger.LogError("Cannot find week to copy from. week {weekid}", weekid);
                    throw new ArgumentException($"Cannot find week to be copied. week {weekid}");
                }

                //check if start date has collision with an existing week
                int collisionCount = await _dbcontext.EdsWeeklyPlan.CountAsync(r => r.FkEds12weekPlan == currWeek.FkEds12weekPlan && r.StartDate == startdate.Date);
                if (collisionCount > 0)
                {
                    _logger.LogError("New Start Date already has week. Start Date {startdate}", startdate);
                    throw new ArgumentException($"New Start date already has a weekly plan in it. StartDate {startdate}", "startdate");
                }

                //proceed with creating deep copy (except specific properties, as these are new items)
                EdsWeeklyPlan newWeek = new EdsWeeklyPlan()
                {
                    StartDate = startdate.Date,
                    EndDate = startdate.Date.AddDays(7).AddSeconds(-1),
                    FkEds12weekPlan = currWeek.FkEds12weekPlan
                };

                foreach (var daily in currWeek.EdsDailyPlan)
                {
                    var daily_timediff = daily.StartDay - currWeek.StartDate;
                    var newStartDate = newWeek.StartDate.GetValueOrDefault().Add(daily_timediff.GetValueOrDefault());
                    EdsDailyPlan newDaily = new EdsDailyPlan()
                    {
                        StartDay = newStartDate.Date,
                        EndDay = newStartDate.Date.AddDays(1).AddSeconds(-1)
                    };

                    foreach (var session in daily.EdsTrainingSession)
                    {
                        EdsTrainingSession newSession = new EdsTrainingSession()
                        {
                            Name = session.Name,
                            Description = session.Description,
                            StartDateTime = newDaily.StartDay.GetValueOrDefault().Date.Add(session.StartDateTime.GetValueOrDefault().TimeOfDay),
                            EndDateTime = newDaily.StartDay.GetValueOrDefault().Date.Add(session.EndDateTime.GetValueOrDefault().TimeOfDay)
                        };

                        foreach (var exercise in session.EdsExercise)
                        {
                            EdsExercise newExer = new EdsExercise()
                            {
                                FkExerciseTypeId = exercise.FkExerciseTypeId,
                            };

                            foreach (var set in exercise.EdsSet)
                            {
                                EdsSet newSet = new EdsSet()
                                {
                                    SetSequenceNumber = set.SetSequenceNumber,
                                };

                                foreach (var metric in set.EdsSetMetrics)
                                {
                                    EdsSetMetrics newMetric = new EdsSetMetrics()
                                    {
                                        FkMetricsTypeId = metric.FkMetricsTypeId,
                                        TargetCustomMetric = metric.TargetCustomMetric,
                                    };
                                    newSet.EdsSetMetrics.Add(newMetric);
                                }

                                newExer.EdsSet.Add(newSet);
                            }

                            newSession.EdsExercise.Add(newExer);
                        }

                        newDaily.EdsTrainingSession.Add(newSession);
                    }

                    newWeek.EdsDailyPlan.Add(newDaily);
                }

                _dbcontext.EdsWeeklyPlan.Add(newWeek);
                int rowsAffected = await _dbcontext.SaveChangesAsync();

                if (rowsAffected == 0)
                    return null;
                return newWeek;
            }
        }
        #endregion
    }
}
