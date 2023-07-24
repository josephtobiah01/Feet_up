using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Logs;
using DAOLayer.Net7.Nutrition;
using DAOLayer.Net7.Supplement;
using DAOLayer.Net7.User;
using FeedApi.Net7.Models;
using LogHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using System.Diagnostics;


namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ExerciseController : BaseController
    {

        private readonly ExerciseContext _context;
        private readonly NutritionContext _nContext;
        private readonly SupplementContext _sContext;
        private readonly LogsContext _lContext;

        public ExerciseController(ExerciseContext context, NutritionContext nContext, SupplementContext sContext, LogsContext lContext)
        {
            _context = context;
            _nContext = nContext;
            _sContext = sContext;
            _lContext = lContext;
        }


        [HttpPost]
        [Route("SetFeedBack")]
        public async Task<bool> SetFeedBack(long traningsessionId, float sliderFeedback, string Textfeedback)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var session = await _context.EdsTrainingSession.Where(t => t.Id == traningsessionId).FirstOrDefaultAsync();
                if (session == null) return false;
                session.CustomerFedback = Textfeedback;
                session.FloatFeedback = sliderFeedback;
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

            [HttpPost]
        [Route("CreateCustomExercise")]
        public async Task<EmTrainingSession> CreateCustomExercise(long UserID, string startDatetime, string EndDatetime)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                DateTime StartDate = DateTime.Parse(startDatetime, System.Globalization.CultureInfo.InvariantCulture);
                DateTime EndDatet = DateTime.Parse(EndDatetime, System.Globalization.CultureInfo.InvariantCulture);

                var user = _context.User.Where(t => t.Id == UserID)
                    .Include(t => t.Eds12weekPlan)
                    .ThenInclude(t => t.EdsWeeklyPlan)
                    .ThenInclude(t => t.EdsDailyPlan).FirstOrDefault();

                var _Eds12weekPlan = user.Eds12weekPlan.Where(t => t.StartDate <= StartDate && t.EndDate >= StartDate && t.IsCurrent == true).FirstOrDefault();
                if (_Eds12weekPlan == null)
                {
                    DateTime dt = StartDate.StartOfWeek(System.DayOfWeek.Monday);
                    DateTime dt2 = dt.AddDays(83);
                    user.Eds12weekPlan.Add(new Eds12weekPlan()
                    {
                        DurationWeeks = 12,
                        StartDate = dt.Date,
                        EndDate = dt2.Date.AddDays(63).AddHours(23).AddMinutes(59).AddSeconds(59),
                        IsCurrent = true,
                        IsTemplate = false,
                        Name = "User Generated Plan " + new Random().Next(0, 100000)
                    });
                    _context.SaveChanges();
                    _Eds12weekPlan = user.Eds12weekPlan.Where(t => t.StartDate <= StartDate && t.EndDate >= StartDate).FirstOrDefault();
                }


                var _EdsWeeklyPlan = _Eds12weekPlan.EdsWeeklyPlan.Where(t => t.StartDate <= StartDate && t.EndDate >= StartDate).FirstOrDefault();
                if (_EdsWeeklyPlan == null)
                {
                    DateTime dt = StartDate.StartOfWeek(System.DayOfWeek.Monday);
                    DateTime dt2 = dt.AddDays(6);

                    _Eds12weekPlan.EdsWeeklyPlan.Add(new EdsWeeklyPlan()
                    {
                        StartDate = dt.Date,
                        EndDate = dt2.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                        FkEds12weekPlan = _Eds12weekPlan.Id
                    });
                    _context.SaveChanges();
                    _EdsWeeklyPlan = _Eds12weekPlan.EdsWeeklyPlan.Where(t => t.StartDate <= StartDate && t.EndDate >= StartDate).FirstOrDefault();
                }
               


                var _eds_daily_plan = _EdsWeeklyPlan.EdsDailyPlan.Where(t => t.StartDay <= StartDate && t.EndDay >= StartDate).FirstOrDefault();
                if (_eds_daily_plan == null)
                {
                    _EdsWeeklyPlan.EdsDailyPlan.Add(new EdsDailyPlan()
                    {
                        StartDay = StartDate.Date,
                        EndDay = StartDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                        FkEdsWeeklyPlanId = _EdsWeeklyPlan.Id
                    });
                    _context.SaveChanges();
                    _eds_daily_plan = _EdsWeeklyPlan.EdsDailyPlan.Where(t => t.StartDay <= StartDate && t.EndDay >= StartDate).FirstOrDefault();
                }
             


                EdsTrainingSession _EdsTrainingSession = new EdsTrainingSession()
                {
                    StartDateTime = StartDate,
                    EndDateTime = EndDatet,
                    Name = "Custom TrainingSession",
                    Description = "Custom TrainingSession"
                };

                _eds_daily_plan.EdsTrainingSession.Add(_EdsTrainingSession);
                _EdsTrainingSession.FkEdsDailyPlan = _eds_daily_plan.Id;
                _EdsTrainingSession.StartTimestamp = StartDate;

                _context.EdsTrainingSession.Add(_EdsTrainingSession);
                _context.SaveChanges();
                return MapTrainingSession(_EdsTrainingSession);
            }
            catch(Exception e)
            {
                return null;
            }
        }



        [HttpGet]
        [Route("GetUserByID")]
        public DAOLayer.Net7.Exercise.User GetUserByID(long ID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var user2 = _context.User.Where(t => t.Id == t.Id).First();
            return user2;
        }

        public static EmExerciseClass MapExerciseClass(EdsExerciseClass _obj)
        {
            EmExerciseClass obj = new EmExerciseClass();

            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();

            return obj;
        }

        public static EmEquipment MapEquipment(EdsEquipment _obj)
        {
            EmEquipment obj = new EmEquipment();
            if (_obj == null) return obj;
            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();
            obj.ImageUrl = _obj.ImageUrl;
            return obj;
        }

        public static EmMechanicsType MapMechanicsType(EdsMechanicsType _obj)
        {
            EmMechanicsType obj = new EmMechanicsType();
            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();
            return obj;
        }

        public static EmLevel MapLevel(EdsLevel _obj)
        {
            EmLevel obj = new EmLevel();
            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();
            return obj;
        }

        public static EmMainMuscleWorked MapMainMuscleWorked(EdsMainMuscleWorked _obj)
        {
            EmMainMuscleWorked obj = new EmMainMuscleWorked();
            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();
            return obj;
        }

        public static EmForce MapForce(EdsForce _obj)
        {
            EmForce obj = new EmForce();
            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();
            return obj;
        }

        public static EmSport MapSport(EdsSport _obj)
        {
            EmSport obj = new EmSport();
            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();
            return obj;
        }

        public static EmOtherMuscleWorked MapOtherMuscleWorked(EdsOtherMuscleWorked _obj)
        {
            EmOtherMuscleWorked obj = new EmOtherMuscleWorked();
            obj.Id = _obj.Id;
            obj.IsDeleted = _obj.IsDeleted;
            obj.Name = _obj.Name.Trim();
            return obj;
        }

        public static EmSetMetricTypes MapSetMetricTypes(EdsSetMetricTypes _obj)
        {
            EmSetMetricTypes obj = new EmSetMetricTypes();
            obj.Id = _obj.Id;
            obj.Name = _obj.Name.Trim();
            obj.isTime = _obj.IsTime;
            obj.isWeight = _obj.IsWeight;
            obj.isDistance = _obj.IsDistance;
            obj.isRepetition = _obj.IsRepetition;
            obj.isResistance = _obj.IsResistance;
            return obj;
        }

        public static EmSetMetrics MapSetMetrics(EdsSetMetrics _obj)
        {
            EmSetMetrics obj = new EmSetMetrics();
            obj.Id = _obj.Id;
            obj.TargetCustomMetric = _obj.TargetCustomMetric;
            obj.ActualCustomMetric = _obj.ActualCustomMetric;
            obj.EmSetMetricTypes = ExerciseController.MapSetMetricTypes(_obj.FkMetricsType);
            return obj;
        }

        public static List<EmDailyPlan> MapDailyPlan(List<EdsDailyPlan> _obj)
        {
            List<EmDailyPlan> obj = new List<EmDailyPlan>();
            foreach (var x in _obj)
            {
                EmDailyPlan mobj = new EmDailyPlan();
                mobj.Id = x.Id;
                mobj.StartDay = x.StartDay;
                mobj.EndDay = x.EndDay;
                obj.Add(mobj);
            }
            return obj;
        }

        public static EmSet MapSet(EdsSet _obj)
        {
            EmSet obj = new EmSet();
            obj.Id = _obj.Id;
            if (_obj.FkExerciseId.HasValue)
            {
                obj.ExerciseId = _obj.FkExerciseId.Value;
            }
            obj.SetSequenceNumber = _obj.SetSequenceNumber;
            obj.IsComplete = _obj.IsComplete;
            obj.IsSkipped = _obj.IsSkipped;
            obj.IsCustomerAddedSet = _obj.IsCustomerAddedSet;
            obj.EndTimeStamp = _obj.EndTimeStamp;
            obj.TimeOffset = _obj.TimeOffset;

            if (_obj.EdsSetMetrics != null) obj.EmSetMetrics = new List<EmSetMetrics>();
            if (_obj.EdsSetMetrics == null) _obj.EdsSetMetrics = new List<EdsSetMetrics>();
            foreach (var x in _obj.EdsSetMetrics)
            {
                obj.EmSetMetrics.Add(ExerciseController.MapSetMetrics(x));
            }
            return obj;
        }

        public static EmExerciseType MapExerciseType(EdsExerciseType _obj)
        {
            EmExerciseType obj = new EmExerciseType();

            obj.Id = _obj.Id;
            obj.EmExerciseClass = MapExerciseClass(_obj.FkExerciseClass);
            obj.EmEquipment = MapEquipment(_obj.FkEquipment);
            obj.EmMechanicsType = MapMechanicsType(_obj.FkMechanicsType);
            obj.EmLevel = MapLevel(_obj.FkLevel);
            obj.EmMainMuscleWorked = MapMainMuscleWorked(_obj.FkMainMuscleWorked);
            obj.EmForce = MapForce(_obj.FkForce);
            obj.EmSport = MapSport(_obj.FkSport);
            obj.EmOtherMuscleWorked = MapOtherMuscleWorked(_obj.FkOtherMuscleWorked);
            obj.Name = _obj.Name;
            obj.ExplainerVideoFr = _obj.ExplainerVideoFr;
            obj.ExplainerTextFr = _obj.ExplainerTextFr;
            obj.HasSetDefaultTemplate = _obj.HasSetDefaultTemplate;
            obj.IsSetCollapsed = _obj.IsSetCollapsed;
            obj.IsDeleted = _obj.IsDeleted;
            obj.ImageUrl = _obj.ExerciseImage;
            return obj;
        }

        public static EmExercise MapExercise(EdsExercise eds_exercise)
        {
            EmExercise exercise = new EmExercise();
            exercise.Id = eds_exercise.Id;
            exercise.FkExerciseTypeId = eds_exercise.FkExerciseTypeId;
            exercise.EmExerciseType = ExerciseController.MapExerciseType(eds_exercise.FkExerciseType);
            exercise.IsCustomerAddedExercise = eds_exercise.IsCustomerAddedExercise;
            exercise.TimeOffset = eds_exercise.TimeOffset;

            if (eds_exercise.EdsSet != null) exercise.EmSet = new List<EmSet>();
            if (eds_exercise.EdsSet == null) eds_exercise.EdsSet = new List<EdsSet>();
            foreach (var x in eds_exercise.EdsSet)
            {
                exercise.EmSet.Add(ExerciseController.MapSet(x));
            }
            exercise.EndTimeStamp = eds_exercise.EndTimeStamp;
            exercise.FkExerciseTypeId = eds_exercise.FkExerciseTypeId;
            exercise.IsComplete = eds_exercise.IsComplete;
            exercise.FkTrainingId = eds_exercise.FkTrainingId;
            exercise.IsSkipped = eds_exercise.IsSkipped;
            return exercise;
        }

        public static EmTrainingSession MapTrainingSession(EdsTrainingSession eds_session)
        {
            EmTrainingSession session = new EmTrainingSession();

            session.Id = eds_session.Id;
            session.Name = eds_session.Name.Trim();
            if (eds_session.Description != null)
            {
                session.Description = eds_session.Description.Trim();
            }
            session.FkEdsDailyPlan = eds_session.FkEdsDailyPlan;
            session.StartDateTime = eds_session.StartDateTime;
            session.EndDateTime = eds_session.EndDateTime;
            session.IsMoved = eds_session.IsMoved;
            session.IsSkipped = eds_session.IsSkipped;
            session.EndTimeStamp = eds_session.EndTimeStamp;
            session.IsCustomerAddedTrainingSession = eds_session.IsCustomerAddedTrainingSession;
            session.IsCustomerAddedTrainingSession = eds_session.IsCustomerAddedTrainingSession;
            session.ReasonForReschedule = eds_session.ReasonForReschedule;
            session.ReadonForSkipping = eds_session.ReadonForSkipping;
            session.ExerciseDuration = eds_session.ExerciseDuration;
            session.StartTimestamp = eds_session.StartTimestamp;
            session.TimeOffset = eds_session.TimeOffset;

            if (eds_session.EdsExercise != null) session.emExercises = new List<EmExercise>();
            if (eds_session.EdsExercise == null) eds_session.EdsExercise = new List<EdsExercise>();
            foreach (var x in eds_session.EdsExercise)
            {
                session.emExercises.Add(ExerciseController.MapExercise(x));
            }
            return session;
        }

        public static List<FeedItem> ParseFeed(List<EdsTrainingSession> sessions)
        {
            if (sessions == null) return new List<FeedItem>();

            List<FeedItem> FeedList = new List<FeedItem>();

            foreach (var t in sessions)
            {
                FeedItem item = new FeedItem();
                item.ID = "TR" + t.Id;
                item.ItemType = FeedItemType.TrainingSessionFeedItem;
                item.TrainingSessionFeedItem = new TrainingSessionFeedItem();
                item.TrainingSessionFeedItem.TraningSession = ExerciseController.MapTrainingSession(t);
                item.Title = t.Name;
                if (t.StartDateTime.HasValue)
                {
                    item.Date = t.StartDateTime.Value;
                }
                item.Status = FeedItemStatus.Scheduled;
                if (t.StartTimestamp.HasValue)
                {
                    item.Status = FeedItemStatus.Ongoing;
                    if (t.EndTimeStamp.HasValue)
                    {
                        item.Status = FeedItemStatus.Completed;
                    }
                }

                if (t.IsMoved)
                    item.Status = FeedItemStatus.Snoozed;
                if (t.IsSkipped)
                    item.Status = FeedItemStatus.Skipped;


                item.Text = new List<TextPair>();
                item.Text.Add(new TextPair(TextCategory.Description, t.Description));

                if (t.ExerciseDuration.HasValue)
                    item.Text.Add(new TextPair(TextCategory.Estimated_Time, t.ExerciseDuration.Value.ToString()));

                if (t.EdsExercise.Count > 0)
                {
                    string muscleWorked = "";
                    foreach (var r in t.EdsExercise)
                    {
                        if (r.FkExerciseType.FkMainMuscleWorked != null)
                        {
                            if (r.FkExerciseType.FkMainMuscleWorked.Name.Trim().ToLower() != "none")
                            {
                                muscleWorked += r.FkExerciseType.FkMainMuscleWorked.Name + ", ";
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(muscleWorked))
                    {
                        muscleWorked = muscleWorked.Trim();
                        muscleWorked = muscleWorked.Trim(',');
                        item.Text.Add(new TextPair(TextCategory.Target_Muscles, muscleWorked));
                    }
                }
                FeedList.Add(item);
            }
            return FeedList;
        }

        [HttpGet]
        [Route("GetTrainingSession")]
        public async Task<EmTrainingSession> GetTrainingSession(long TraningSessionId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                DateTime Timestamp1 = DateTime.Now;
                var sessions = await _context.EdsTrainingSession.Where(t => t.Id == TraningSessionId)
                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.EdsSet)
                    .ThenInclude(t => t.EdsSetMetrics)
                    .ThenInclude(t => t.FkMetricsType)
                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkExerciseClass)

                     .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkLevel)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkForce)


                     .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkEquipment)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkMechanicsType)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkMainMuscleWorked)


                     .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkOtherMuscleWorked)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkSport).AsNoTracking().AsSingleQuery()
                    .FirstAsync();

                var traningsession = ExerciseController.MapTrainingSession(sessions);
                return traningsession;

            }
            catch (Exception e)
            {
                return new EmTrainingSession();
            }
        }


        [HttpGet]
        [Route("GetDailyFeed")]
        public async Task<List<FeedItem>> GetDailyFeed(long PlanId, string Dateft, long UserId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var sessions = await _context.EdsTrainingSession.Where(t => t.FkEdsDailyPlan == PlanId)
                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.EdsSet)
                    .ThenInclude(t => t.EdsSetMetrics)
                    .ThenInclude(t => t.FkMetricsType)
                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkExerciseClass)

                     .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkLevel)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkForce)


                     .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkEquipment)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkMechanicsType)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkMainMuscleWorked)


                     .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkOtherMuscleWorked)


                    .Include(t => t.EdsExercise)
                    .ThenInclude(t => t.FkExerciseType)
                    .ThenInclude(t => t.FkSport).AsNoTracking().AsSingleQuery()
                    .ToListAsync();


                List<FeedItem> mm = new List<FeedItem>();
                if (sessions != null && sessions.Count() > 0)
                {
                    mm = ExerciseController.ParseFeed(sessions.OrderBy(t => t.StartDateTime).ToList());
                }

                //
                //  NUTRIENTS
                //


                DateTime Date = DateTime.Parse(Dateft, System.Globalization.CultureInfo.InvariantCulture).Date;
                //  var nday = await _nContext.FnsNutritionActualDay.Where(t => t.FkUserId == UserId && t.Date >= Date && t.Date < Date.AddDays(1))
                   var nday = await _nContext.FnsNutritionActualDay.Where(t => t.FkUserId == UserId && t.Date == Date)
              //  var nday = await _nContext.FnsNutritionActualDay.Where(t => t.FkUserId == UserId)
                       .Include(t => t.FnsNutritionActualMeal)
                    .ThenInclude(t => t.MealType)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (nday != null)
                {
                    foreach (var k in nday.FnsNutritionActualMeal)
                    {
                        NutrientMeal meal = new NutrientMeal();
                        meal.DayId = nday.Id;
                        meal.MealId = k.Id;
                        meal.TargetKiloCalories = k.MealCalorieTarget;
                        meal.MealType = k.MealTypeId;

                        FeedItem nutrition = new FeedItem();
                        nutrition.ID = "ME" + k.Id;
                        nutrition.ItemType = FeedItemType.NutrientsFeedItem;
                        nutrition.NutrientsFeedItem = new NutrientsFeedItem();
                        nutrition.NutrientsFeedItem.Meal = meal;
                        nutrition.Title = k.MealType.Name;
                        nutrition.Date = k.SnoozedTime.HasValue ? k.SnoozedTime.Value : GetDateFromMealName(k, nday);
                        nutrition.Text = new List<TextPair>();
                        nutrition.Text.Add(new TextPair(TextCategory.Description, "Log your " + k.MealType.Name + " to keep log of your nutrients"));
                        nutrition.Text.Add(new TextPair(TextCategory.Nutrient_Calories, meal.TargetKiloCalories.ToString()));

                        nutrition.Status = FeedItemStatus.Scheduled;
                        if (k.IsSkipped)
                        {
                            nutrition.Text = new List<TextPair>();
                            nutrition.Status = FeedItemStatus.Skipped;
                        }
                        if (k.IsSnoozed) nutrition.Status = FeedItemStatus.Snoozed;
                        if (k.IsOngoing)
                        {
                            nutrition.Status = FeedItemStatus.Ongoing;
                            nutrition.Text = new List<TextPair>();
                            nutrition.Text.Add(new TextPair(TextCategory.Description, "Hold tight! We are checking and logging your nutrients. We will notify once we are done!"));
                        }
                        if (k.IsComplete)
                        {
                            nutrition.Status = FeedItemStatus.Completed;
                        }
                        if (k.MealTypeId == 4)
                        {
                            if (k.Timestamp.HasValue)
                            {
                                nutrition.Date = k.Timestamp.Value;
                            }
                        }
                        mm.Add(nutrition);
                    }
                }

                //
                // Supplements
                //
                // 


                var s_schedule = await _sContext.NdsSupplementSchedulePerDate.Where(t => t.CustomerId == UserId && t.Date == Date.Date)
                    .Include(t => t.NdsSupplementSchedule)
                    .ThenInclude(t => t.NdsSupplementScheduleDose)

                    .Include(t => t.NdsSupplementSchedule)
                    .ThenInclude(t => t.FkSupplementReferenceNavigation)
                    .ThenInclude(t => t.FkUnitMetricNavigation)

                    .Include(t => t.NdsSupplementSchedule)
                    .ThenInclude(t => t.FkSupplementReferenceNavigation)
                    .ThenInclude(t => t.FkSupplementInstructionNavigation)

                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (s_schedule != null)
                {
                    Dictionary<string, List<NdsSupplementScheduleDose>> dose_buckets = new Dictionary<String, List<NdsSupplementScheduleDose>>();
                    foreach (var s_supp in s_schedule.NdsSupplementSchedule)
                    {
                        foreach (var s_dose in s_supp.NdsSupplementScheduleDose)
                        {
                            DateTime s_time;
                            try
                            {
                                s_time = !s_dose.SnoozedTime.HasValue ? s_dose.ScheduledTime.Value : s_dose.SnoozedTime.Value;
                            }
                            catch
                            {
                                s_time = DateTime.MinValue;
                            }
                            if (!dose_buckets.ContainsKey(s_time.Hour.ToString()))
                            {
                                dose_buckets.Add(s_time.Hour.ToString(), new List<NdsSupplementScheduleDose>());
                            }
                            dose_buckets[s_time.Hour.ToString()].Add(s_dose);
                        }
                    }

                    if (dose_buckets.Keys.Count > 0)
                    {
                        foreach (var bucket in dose_buckets.Keys)
                        {

                            var s_bucket = dose_buckets[bucket];

                            FeedItem supp = new FeedItem();
                            bool IsComplete = true;
                            bool IsPartialComplete = false;
                            bool isSkipped = false;
                            bool isSnoozed = false;

                            supp.ItemType = FeedItemType.SupplementItem;
                            supp.SupplementFeedItem = new SupplementFeedItem();
                            supp.SupplementFeedItem.SupplementEntries = new List<SupplementEntry>();

                            DateTime s_date = DateTime.MinValue;
                            int s_supp_count = 0;
                            int s_supp_beforeMeal = 0;
                            int s_supp_afterMeal = 0;


                            if (s_bucket != null && s_bucket.Count > 0)
                            {
                                foreach (var s_dose in s_bucket)
                                {
                                    SupplementEntry supplementEntry = new SupplementEntry();
                                    supplementEntry.Supplementname = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.Name;
                                    supplementEntry.SupplementId = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.Id;
                                    supplementEntry.DoseId = s_dose.Id;
                                    supplementEntry.UnitCount = (float)s_dose.UnitCountTarget;
                                    supplementEntry.UnitName = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkUnitMetricNavigation.Name;
                                    supplementEntry.is_Weight = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkUnitMetricNavigation.IsWeight;
                                    supplementEntry.is_Volume = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkUnitMetricNavigation.IsVolume;
                                    supplementEntry.is_Count = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkUnitMetricNavigation.IsCount;
                                    supplementEntry.Instructions = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.InstructionText;
                                    supplementEntry.Requires_source_of_fat = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkSupplementInstructionNavigation.RequiresSourceOfFat;
                                    supplementEntry.Take_after_meal = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkSupplementInstructionNavigation.TakeAfterMeal;
                                    supplementEntry.Take_before_sleep = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkSupplementInstructionNavigation.TakeBeforeSleep;
                                    supplementEntry.Take_on_empty_stomach = s_dose.FkSupplementScheduleNavigation.FkSupplementReferenceNavigation.FkSupplementInstructionNavigation.TakeOnEmptyStomach;
                                    supplementEntry.ScheduledTime = !s_dose.SnoozedTime.HasValue ? s_dose.ScheduledTime.Value : s_dose.SnoozedTime.Value;
                                    supplementEntry.isComplete = s_dose.IsComplete;
                                    supplementEntry.isSnoozed = s_dose.IsSnoozed.HasValue ? s_dose.IsSnoozed.Value : false;
                                    supplementEntry.SnoozedTimeMinutes = s_dose.SnoozedTime.HasValue ? (int)(s_dose.SnoozedTime.Value - s_dose.ScheduledTime.Value).TotalMinutes : 0;
                                    s_date = supplementEntry.ScheduledTime;

                                    if (!s_dose.IsComplete) IsComplete = false;
                                    if (s_dose.IsComplete) IsPartialComplete = true;
                                    else if (s_dose.IsSkipped) isSkipped = true;
                                    else if (s_dose.IsSnoozed.HasValue && s_dose.IsSnoozed.Value == true) isSnoozed = true;

                                    supp.SupplementFeedItem.SupplementEntries.Add(supplementEntry);
                                    s_supp_count++;
                                    if (supplementEntry.Take_on_empty_stomach)
                                    {
                                        s_supp_beforeMeal++;
                                    }
                                    if (supplementEntry.Take_after_meal)
                                    {
                                        s_supp_afterMeal++;
                                    }
                                }
                            }

                            supp.ID = "SU" + int.Parse(bucket);
                            supp.Status = FeedItemStatus.Scheduled;
                            supp.Text = new List<TextPair>();
                            supp.Text.Add(new TextPair(TextCategory.Supplement_Count, "Total Supplements: " + s_supp_count));
                            supp.Text.Add(new TextPair(TextCategory.Supplement_Instructions, s_supp_beforeMeal + "  Before meal, " + s_supp_afterMeal + " After meal"));
                            supp.Title = "Take Supplements";
                            supp.Date = s_date.Date.AddHours(int.Parse(bucket));

                            if (IsComplete) supp.Status = FeedItemStatus.Completed;
                            else if (IsPartialComplete && isSkipped) supp.Status = FeedItemStatus.Partly_Skipped;
                            else if (isSkipped) supp.Status = FeedItemStatus.Skipped;
                            else if (isSnoozed) supp.Status = FeedItemStatus.Snoozed;
                            mm.Add(supp);
                        }
                    }
                }

                //
                // End Supplements
                //

                mm.Sort((ps1, ps2) => DateTime.Compare(ps1.Date, ps2.Date));
                return mm;

            }
            catch (Exception e)
            {
                try
                {
                    Logs llog = sLogHelper.Log(LogHelper.Component.API,
                    this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    (int)LogHelper.Severity.Warning,
                    e.Message,
                    e.StackTrace);

                    _lContext.Add(llog);
                    await _lContext.SaveChangesAsync();
                    return new List<FeedItem>();
                }
                catch { return new List<FeedItem>(); }
            }
        }

        public static DateTime GetDateFromMealName(FnsNutritionActualMeal meal, FnsNutritionActualDay day)
        {
            if (meal.ScheduledTime.HasValue)
            {
                return meal.ScheduledTime.Value;
            }
            if (meal.MealType.Defaulttime.HasValue)
            {
                return day.Date.Date.Add(meal.MealType.Defaulttime.Value);
            }
            return DateTime.MaxValue;
        }

        [HttpGet]
        [Route("GetConfig")]
        public async Task<List<int>> GetConfig()
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            List<int> list = new List<int>();
            list.Add(60);
            list.Add(120);
            list.Add(15);
            list.Add(1);
            return list;
        }

        [HttpGet]
        [Route("GetDailyPlanId")]
        public async Task<List<EmDailyPlan>> GetDailyPlanId(long UserId, string Dateft)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                DateTime Date = DateTime.Parse(Dateft, System.Globalization.CultureInfo.InvariantCulture);
                var user2 = await _context.Eds12weekPlan.Where(t => t.FkCustomerId == UserId && t.IsCurrent == true)
                    .Include(t => t.EdsWeeklyPlan)
                    .ThenInclude(t => t.EdsDailyPlan)
                    .FirstAsync();

                if (user2 == null) return null;

                List<EmDailyPlan> planList = new List<EmDailyPlan>();
                foreach (var x in user2.EdsWeeklyPlan)
                {
                    planList.AddRange(MapDailyPlan(x.EdsDailyPlan.ToList()));
                }
                return planList;
            }
            catch (Exception e)
            {
                return null;
            }
        }



        #region TraningSession, Exercise, Set  START, SKIP, COMPLETE
        // Start Methods
        [HttpPost]
        [Route("StartTrainingSession")]
        public async Task<bool> StartTrainingSession(long TrainingSessionID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsTrainingSession.Where(t => t.Id == TrainingSessionID).FirstAsync();
            if (x == null) return false;
            x.StartTimestamp = DateTime.UtcNow;
            x.IsSkipped = false;

            // x.o
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("PauseTrainingSession")]
        public async Task<bool> PauseTrainingSession(long TrainingSessionID, int Duration)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsTrainingSession.Where(t => t.Id == TrainingSessionID).FirstAsync();
            if (x == null) return false;
            x.ExerciseDuration = Duration;
            // x.o
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("StartExercise")]
        public async Task<bool> StartExercise(long ExerciseID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsExercise.Where(t => t.Id == ExerciseID).FirstAsync();
            if (x == null) return false;
            x.IsSkipped = false;
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("StartSet")]
        public async Task<bool> StartSet(long SetID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsSet.Where(t => t.Id == SetID).FirstAsync();
            if (x == null) return false;
            x.IsSkipped = false;
            await _context.SaveChangesAsync();
            return true;
        }


        // Skip Methods
        [HttpPost]
        [Route("SkipTrainingSession")]
        public async Task<bool> SkipTrainingSession(long TrainingSessionID, string Reason4Skipping = "")
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsTrainingSession.Where(t => t.Id == TrainingSessionID).FirstAsync();
            if (x == null) return false;
            x.IsSkipped = true;
            if (!String.IsNullOrEmpty(Reason4Skipping))
            {
                //  x.ReadonForSkipping = Reason4Skipping;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("SkipExercise")]
        public async Task<bool> SkipExercise(long ExerciseID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsExercise.Where(t => t.Id == ExerciseID).FirstAsync();
            if (x == null) return false;
            x.IsSkipped = true;
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("RescheduleTrainingSession")]
        public async Task<bool> RescheduleTrainingSession(long TrainingSessionID, int MinutesSnooze, string Reason4Rescheduling)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsTrainingSession.Where(t => t.Id == TrainingSessionID).FirstAsync();
            if (x == null) return false;
            // x.ReasonForReschedule = DateTime.UtcNow;
            x.IsMoved = true;

            if (x.StartDateTime.HasValue)
            {
                x.StartDateTime = x.StartDateTime.Value.AddMinutes(MinutesSnooze);
            }
            if (x.EndDateTime.HasValue)
            {
                x.EndDateTime = x.EndDateTime.Value.AddMinutes(MinutesSnooze);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("SkipSet")]
        public async Task<bool> SkipSet(long SetID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsSet.Where(t => t.Id == SetID).FirstAsync();
            if (x == null) return false;
            x.IsSkipped = true;
            await _context.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("ChangeSetMetrics")]
        public async Task<bool> ChangeSetMetrics(long SetID, long SetMetricsId, double newalue)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsSet.Where(t => t.Id == SetID).Include(t => t.EdsSetMetrics).FirstAsync();
            if (x == null) return false;
            var metric = x.EdsSetMetrics.Where(t => t.Id == SetMetricsId).First();
            if (metric == null) return false;
            metric.ActualCustomMetric = newalue;
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("EndTrainingSession")]
        public async Task<bool> EndTrainingSession(long TrainingSessionID, int Duration)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsTrainingSession.Where(t => t.Id == TrainingSessionID).FirstAsync();
            if (x == null) return false;
            x.ExerciseDuration = Duration;
            x.EndTimeStamp = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("EndExercise")]
        public async Task<bool> EndExercise(long ExerciseID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsExercise.Where(t => t.Id == ExerciseID).FirstAsync();
            if (x == null) return false;
            x.EndTimeStamp = DateTime.UtcNow;
            x.IsComplete = true;
            x.IsSkipped = false;
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("EndSet")]
        public async Task<bool> EndSet(long SetID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsSet.Where(t => t.Id == SetID).FirstAsync();
            if (x == null) return false;
            x.EndTimeStamp = DateTime.UtcNow;
            // TODO OFFSET
            x.IsComplete = true;
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpGet]
        [Route("GetSetById")]
        public async Task<EmSet> GetSetById(long SetID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            // var x = _context.EdsSet.Where(t => t.Id == SetID).First();

            var x = await _context.EdsSet.Where(t => t.Id == SetID)
            .Include(t => t.EdsSetMetrics)
            .ThenInclude(t => t.FkMetricsType)
            .AsNoTracking().AsSplitQuery()
            .FirstAsync();


            if (x == null) return null;
            return MapSet(x);
        }

        [HttpPost]
        [Route("UndoEndSet")]
        public async Task<bool> UndoEndSet(long SetID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsSet.Where(t => t.Id == SetID).FirstAsync();
            if (x == null) return false;
            x.IsComplete = false;
            x.EndTimeStamp = null;
            await _context.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("UpdateSet")]
        public async Task<bool> UpdateSet(long setMetricsId, double newValue)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsSetMetrics.Where(t => t.Id == setMetricsId).FirstAsync();
            if (x == null) return false;
            x.ActualCustomMetric = newValue;
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("UndoSkipTrainingSeseesion")]
        public async Task<bool> UndoSkipTrainingSeseesion(long SessionId, DateTime now)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsTrainingSession.Where(t => t.Id == SessionId).FirstAsync();
            if (x == null) return false;
            x.IsMoved = true;
            x.IsSkipped = false;
            if (x.EndDateTime.HasValue && x.StartDateTime.HasValue)
            {
                var lenght = x.EndDateTime.Value.Subtract(x.StartDateTime.Value);
                x.EndDateTime = now.Add(lenght);
                x.StartDateTime = now;
            }
            await _context.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("AddNewExercise")]
        public async Task<EmExercise> AddNewExercise(long TrainingSessionId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsTrainingSession.Where(t => t.Id == TrainingSessionId)
             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.EdsSet)
             .ThenInclude(t => t.EdsSetMetrics)
             .ThenInclude(t => t.FkMetricsType)
             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkExerciseClass)

             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkLevel)

             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkForce)

             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkEquipment)

             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkMechanicsType)

             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkMainMuscleWorked)

             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkOtherMuscleWorked)

             .Include(t => t.EdsExercise)
             .ThenInclude(t => t.FkExerciseType)
             .ThenInclude(t => t.FkSport).AsSingleQuery()
             .FirstAsync();



            if (x == null) return null;

            var y = x.EdsExercise.Last();

            EdsExercise obj = new EdsExercise();

            obj.IsCustomerAddedExercise = true;
            obj.FkExerciseTypeId = y.FkExerciseTypeId;
            obj.FkExerciseType = y.FkExerciseType;
            obj.EndTimeStamp = null;
            obj.TimeOffset = y.TimeOffset;
            obj.IsSkipped = false;
            obj.IsComplete = false;


            foreach (var k in y.EdsSet)
            {
                EdsSet _obj = new EdsSet();
                _obj.SetSequenceNumber = k.SetSequenceNumber;
                _obj.IsComplete = k.IsComplete;
                _obj.IsSkipped = false;
                _obj.IsCustomerAddedSet = true;
                _obj.EndTimeStamp = null;
                _obj.TimeOffset = k.TimeOffset;

                foreach (var l in k.EdsSetMetrics)
                {
                    var m = new EdsSetMetrics();
                    m.ActualCustomMetric = l.ActualCustomMetric;
                    m.TargetCustomMetric = l.TargetCustomMetric;
                    m.FkMetricsType = l.FkMetricsType;
                    _obj.EdsSetMetrics.Add(m);
                }
                obj.EdsSet.Add(_obj);

            }
            x.EdsExercise.Add(obj);
            await _context.SaveChangesAsync();
            await _context.Entry(obj).ReloadAsync();

            return MapExercise(y);
        }

        [HttpPost]
        [Route("AddNewSet")]
        // ExerciseId -- the Id of the EmExercise where the set is added to
        // EmSetId  -- the ID of the set to use as a template:  this is always the ID of the last set of that exercise
        public async Task<EmSet> AddNewSet(long ExerciseId, long EmSetId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsExercise.Where(t => t.Id == ExerciseId)
                .Include(t => t.EdsSet)
                .ThenInclude(t => t.EdsSetMetrics)
                .ThenInclude(t => t.FkMetricsType)
                .FirstAsync();

            if (x == null) return null;

            var y = x.EdsSet.Last();

            EdsSet set = new EdsSet();
            set.SetSequenceNumber = (short)(y.SetSequenceNumber + 1);
            List<EdsSetMetrics> edsSetMetrics = new List<EdsSetMetrics>();

            foreach (var k in y.EdsSetMetrics)
            {
                var m = new EdsSetMetrics();
                m.ActualCustomMetric = k.ActualCustomMetric;
                m.TargetCustomMetric = k.TargetCustomMetric;
                m.FkMetricsType = k.FkMetricsType;
                set.EdsSetMetrics.Add(m);
            }
            set.FkExercise = y.FkExercise;
            set.FkExerciseId = y.FkExerciseId;
            set.IsComplete = false;
            set.IsSkipped = false;
            set.IsCustomerAddedSet = true;

            x.EdsSet.Add(set);

            await _context.SaveChangesAsync();
            await _context.Entry(set).ReloadAsync();

            return MapSet(set);
        }
        #endregion


        [HttpGet]
        [Route("GetExerciseTypes")]
        public async Task<List<EmExerciseType>> GetExerciseTypes()
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsExerciseType
                .Include(t => t.FkExerciseClass)
                 .Include(t => t.FkEquipment)
                  .Include(t => t.FkMechanicsType)
                   .Include(t => t.FkLevel)
                    .Include(t => t.FkMainMuscleWorked)
                     .Include(t => t.FkForce)
                      .Include(t => t.FkSport)
                       .Include(t => t.FkOtherMuscleWorked)
                .AsNoTracking().ToListAsync();



            if (x == null) return null;

            List<EmExerciseType> y = new List<EmExerciseType>();

            foreach (var k in x)
            {
                if (!k.IsDeleted)
                {
                    y.Add(MapExerciseType(k));
                }
            }
            return y;
        }

        [HttpGet]
        [Route("GetEquipments")]
        public async Task<List<EmEquipment>> GetEquipments()
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsEquipment
                .AsNoTracking().ToListAsync();

            if (x == null) return null;

            List<EmEquipment> y = new List<EmEquipment>();

            foreach (var k in x)
            {
                y.Add(MapEquipment(k));
            }
            return y;
        }

        [HttpGet]
        [Route("GetMainMuscleWorked")]
        public async Task<List<EmMainMuscleWorked>> GetMainMuscleWorked()
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var x = await _context.EdsMainMuscleWorked
                .AsNoTracking().ToListAsync();

            if (x == null) return null;

            List<EmMainMuscleWorked> y = new List<EmMainMuscleWorked>();

            foreach (var k in x)
            {
                y.Add(MapMainMuscleWorked(k));
            }
            return y;
        }


        [HttpGet]
        [Route("GetDefaultExerciseFromExerciseType")]
        public async Task<EmExercise> GetDefaultExerciseFromExerciseType(long ExerciseTypeId, long TraningsessionID)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var x = await _context.EdsExerciseType
                  .Include(t => t.FkExerciseClass)
                   .Include(t => t.FkEquipment)
                    .Include(t => t.FkMechanicsType)
                     .Include(t => t.FkLevel)
                      .Include(t => t.FkMainMuscleWorked)
                       .Include(t => t.FkForce)
                        .Include(t => t.FkSport)
                         .Include(t => t.FkOtherMuscleWorked)
                  .Include(t => t.EdsSetDefaults)
                  .ThenInclude(t => t.EdsSetMetricsDefault)
                  .ThenInclude(t => t.FkSetMetricType)
                  //   .ThenInclude(t => t.EdsSetMetrics)
                  .Where(t => t.Id == ExerciseTypeId)
           .FirstAsync();

                if (x == null) return null;
                //   if (!x.HasSetDefaultTemplate) return new EmExercise();
                if (x.EdsSetDefaults == null || x.EdsSetDefaults.Count < 1) return new EmExercise();

                EdsExercise exercise = new EdsExercise();
                exercise.FkTrainingId = TraningsessionID;
                exercise.FkExerciseType = x;
                exercise.IsCustomerAddedExercise = true;

                foreach (var setDefaults in x.EdsSetDefaults)
                {
                    EdsSet set = new EdsSet();
                    set.FkExercise = exercise;
                    set.IsCustomerAddedSet = true;

                    if (setDefaults.EdsSetMetricsDefault != null && setDefaults.EdsSetMetricsDefault.Count > 0)
                    {
                        foreach (var setmetricsDefault in setDefaults.EdsSetMetricsDefault)
                        {
                            set.SetSequenceNumber = setDefaults.SetSequenceNumber;
                            EdsSetMetrics metrics = new EdsSetMetrics();
                            metrics.FkSet = set;

                            metrics.FkMetricsType = setmetricsDefault.FkSetMetricType;
                            metrics.TargetCustomMetric = setmetricsDefault.DefaultCustomMetric;
                            set.EdsSetMetrics.Add(metrics);
                        }
                        exercise.EdsSet.Add(set);
                    }
                }

                _context.EdsExercise.Add(exercise);
                await _context.SaveChangesAsync();
                return MapExercise(exercise);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("InsertSetHistory")]
        public async Task<bool> InsertSetHistory(UserSetHistory model)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                EdsExerciseTypeUserHistory hhistory = await _context.EdsExerciseTypeUserHistory.Where(t => t.UserId == model.UserId && t.FkExerciseTypeId == model.ExerciseTypeId)
                    .Include(t=>t.EdsExerciseTypeUserHistorySetHistory)
                    .FirstOrDefaultAsync();
                if (hhistory == null)
                {
                    hhistory = new EdsExerciseTypeUserHistory();
                    hhistory.UserId = model.UserId;
                    hhistory.FkExerciseTypeId = model.ExerciseTypeId;
                    hhistory.EdsExerciseTypeUserHistorySetHistory = new List<EdsExerciseTypeUserHistorySetHistory>();
                    _context.EdsExerciseTypeUserHistory.Add(hhistory);
                }
                if(hhistory.EdsExerciseTypeUserHistorySetHistory == null)
                {
                    hhistory.EdsExerciseTypeUserHistorySetHistory = new List<EdsExerciseTypeUserHistorySetHistory>();
                }

                var mset = hhistory.EdsExerciseTypeUserHistorySetHistory.Where(t => t.SetNumber == model.SetNumber).FirstOrDefault();
                
                if (mset != null)
                {
                    mset.HistoryString = model.SetString;
                }
                else
                {
                    hhistory.EdsExerciseTypeUserHistorySetHistory.Add(new EdsExerciseTypeUserHistorySetHistory()
                    {
                        FkSetId = -1,
                        SetNumber = model.SetNumber,
                        HistoryString = model.SetString
                    });
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpGet]
        [Route("GetSetHistory")]
        public async Task<List<UserSetHistory>> GetSetHistory(long UserId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                List<UserSetHistory> userHistory = new List<UserSetHistory>();

                var hhistory = await _context.EdsExerciseTypeUserHistory.Where(t => t.UserId == UserId)
                     .Include(t => t.EdsExerciseTypeUserHistorySetHistory)
                     .AsNoTracking()
                     .ToListAsync();
                if (hhistory == null) return null;
           
                foreach(var history in hhistory)
                {
                    foreach (var sethistory in history.EdsExerciseTypeUserHistorySetHistory)
                    {
                        userHistory.Add(new UserSetHistory()
                        {
                            UserId = UserId,
                            ExerciseTypeId = history.FkExerciseTypeId,
                            SetNumber = sethistory.SetNumber,
                            SetString = sethistory.HistoryString
                        });
                    }
                }
                return userHistory;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
