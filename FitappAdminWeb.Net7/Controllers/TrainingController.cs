using AutoMapper;
using DAOLayer.Net7.Exercise;
using FitappAdminWeb.Net7.Classes.Base;
using FitappAdminWeb.Net7.Classes.Repositories;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace FitappAdminWeb.Net7.Controllers
{
    public class TrainingController : BaseController
    {
        private const string SKEY_CURRENTUSER = "skey_currentuser";

        TrainingRepository _trrepo;
        ClientRepository _clientrepo;
        LookupRepository _lookup;
        MessageRepository _messagerepo;
        IMapper _mapper;
        ILogger<TrainingController> _logger;

        public TrainingController(TrainingRepository trrepo,
                                  ClientRepository clientrepo,
                                  LookupRepository lookuprepo,
                                  ILogger<TrainingController> logger,
                                  IMapper mapper,
                                  MessageRepository messagerepo)
            : base(messagerepo)
        {
            _trrepo = trrepo;
            _clientrepo = clientrepo;
            _lookup = lookuprepo;
            _logger = logger;
            _mapper = mapper;
            _messagerepo = messagerepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long id)
        {
            if (id != 0)
            {
                HttpContext.Session.SetInt32(SKEY_CURRENTUSER, (int)id);
            }

            int? currentUserId = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);
            if (currentUserId.HasValue)
            {
                TrainingProgramViewModel vm = new TrainingProgramViewModel();
                vm.CurrentUser = await _trrepo.GetClientById(currentUserId.Value);
                if (vm.CurrentUser != null)
                {
                    vm.Programs = await _trrepo.GetAllProgramsForUser(vm.CurrentUser.Id);
                    vm.IdentityUser = await _clientrepo.GetIdentityUserById(vm.CurrentUser.FkFederatedUser);    
                }
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditProgram(long? id = null)
        {
            //Make sure a user is already selected
            int? currentUserId = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            TrainingProgramEditViewModel vm = new TrainingProgramEditViewModel()
            {
                Data = new TrainingProgramEditFormModel()
                {
                    UserId = currentUserId.Value
                },
                CurrentUser = await _trrepo.GetClientById(currentUserId.Value)
            };

            var templatelist = await _trrepo.GetTemplateList(true);
            vm.List_ProgramTemplates = templatelist.Select(r => 
                                        new SelectListItem()
                                        {
                                            Text = $"{r.Name} (ID: {r.Id})",
                                            Value = r.Id.ToString()
                                        }).ToList();

            if (id.HasValue) //EditMode
            {
                Eds12weekPlan currentPlan = await _trrepo.GetProgramById(id.Value);
                if (currentPlan != null)
                {
                    vm.Data = _mapper.Map<TrainingProgramEditFormModel>(currentPlan);
                }
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ProgramDetails(long id)
        {
            Eds12weekPlan currentPlan = await _trrepo.GetCompleteProgramById(id);
            if (currentPlan == null)
            {
                return View(new ProgramDetailViewModel());
            }

            //calculate completion rates
            float exercise_compliancerate = 0;
            float set_compliancerate = 0;
            string most_completed_ex = "No Data";
            string most_skipped_ex = "No Data";

            float completedSets = 0;
            float completedExers = 0;
            float pendingExers = 0;
            float pendingSets = 0;
            float totalExers = 0;
            float totalSets = 0;
            float skippedSets = 0;
            float skippedExers = 0;
            float skippedSessions = 0;

            //sort weekly by start date
            currentPlan.EdsWeeklyPlan.OrderBy(r => r.StartDate);

            foreach (var weekly in currentPlan.EdsWeeklyPlan)
            {
                foreach (var daily in weekly.EdsDailyPlan)
                {
                    foreach (var session in daily.EdsTrainingSession)
                    {
                        skippedExers += session.EdsExercise.Count(r => r.IsSkipped);
                        completedExers += session.EdsExercise.Count(r => r.IsComplete);
                        pendingExers += session.EdsExercise.Count(r => !r.IsSkipped && !r.IsComplete);
                        totalExers += session.EdsExercise.Count();
                        exercise_compliancerate = completedExers / (completedExers + skippedExers) * 100;
                        
                        long mostCompletedExerciseType = session.EdsExercise.Where(r => r.IsComplete)
                            .GroupBy(r => r.FkExerciseTypeId)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key).FirstOrDefault();
                        long mostSkippedExerciseType = session.EdsExercise.Where(r => r.IsSkipped)
                            .GroupBy(r => r.FkExerciseTypeId)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key).FirstOrDefault();

                        if (mostCompletedExerciseType != 0)
                        {
                            most_completed_ex = (await _lookup.GetExerciseTypeById(mostCompletedExerciseType)).Name;
                        }
                        if (mostSkippedExerciseType != 0)
                        {
                            most_skipped_ex = (await _lookup.GetExerciseTypeById(mostSkippedExerciseType)).Name;
                        }

                        foreach (var exercise in session.EdsExercise)
                        {
                            skippedSets += exercise.EdsSet.Count(r => r.IsSkipped);
                            completedSets += exercise.EdsSet.Count(r => r.IsComplete);
                            pendingSets += exercise.EdsSet.Count(r => !r.IsComplete && !r.IsSkipped);
                            totalSets += exercise.EdsSet.Count();
                            set_compliancerate = completedSets / (completedSets + skippedSets) * 100;
                        }
                    }
                }
            };


            ProgramDetailViewModel vm = new ProgramDetailViewModel()
            {
                CurrentPlan = currentPlan,
                SetComplianceRate = float.IsNaN(set_compliancerate) ? 100 : set_compliancerate, //a NaN generally means no data, so effectively still 100
                CompletedSets = !float.IsNaN(completedSets) ? completedSets : 0,
                SkippedSets = !float.IsNaN(skippedSets) ? skippedSets : 0,
                PendingSets = !float.IsNaN(pendingSets) ? pendingSets : 0,
                TotalSets = totalSets,

                ExerciseComplianceRate = float.IsNaN(exercise_compliancerate) ? 100 : exercise_compliancerate,
                CompletedExercises = !float.IsNaN(completedExers) ? completedExers : 0,
                SkippedExercises = !float.IsNaN(skippedExers) ? skippedExers : 0,
                PendingExercises = !float.IsNaN(pendingExers) ? pendingExers : 0,
                TotalExercises = totalExers,
                
                MostCompletedExercise = most_completed_ex,
                MostSkippedExercise = most_skipped_ex
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Sessions(long id)
        {
            if (id != 0)
            {
                HttpContext.Session.SetInt32(SKEY_CURRENTUSER, (int) id);
            }

            int? currentUserId = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);

            if (currentUserId != null)
            {
                TrainingSessionViewModel? vm;
                DateTime? selectedDate = null;
                long? programid = null;

                DateTime parseDt;
                if (Request.Query.ContainsKey("date") && DateTime.TryParse(Request.Query["date"], out parseDt))
                {
                    selectedDate = parseDt;
                }
                long parseId;
                if (Request.Query.ContainsKey("pid") && long.TryParse(Request.Query["pid"], out parseId))
                {
                    programid = parseId;
                }
                vm = await FillTrainingIndexViewModel((long) currentUserId, programid, selectedDate);
                
                //vm = await GetDummyTrainingData();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditSession(
            int? id = null, //training session id
            DateTime? date = null, //specific date filter
            long? pid = null, //program id
            bool copy = false, //true if this is a clone op
            bool tmpl = false)
        {
            EdsTrainingSession loadedSession = null;
            Eds12weekPlan loadedPlan = null;
            if (id.HasValue)
            {
                loadedSession = await _trrepo.GetTrainingSessionById(id.Value);
                if (loadedSession != null)
                {
                    loadedPlan = await _trrepo.GetProgramOfTrainingSession(loadedSession.Id);
                }
            }

            TrainingSessionEditViewModel vm = new TrainingSessionEditViewModel() { IsCopy = copy };
            if (tmpl)
            {
                vm.CurrentUser = new User();
            }
            else
            {
                int? sessionUser = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);
                vm.CurrentUser = await _trrepo.GetClientById(sessionUser ?? 0);
            }


            if (loadedSession != null && loadedPlan != null)
            {
                vm.Data = _mapper.Map<TrainingSessionEditFormModel>(loadedSession);
                vm.Data.Eds12WeekProgramId = loadedPlan.Id;   
            }

            vm.Data.IsTemplate = tmpl;

            if (date.HasValue)
            {
                vm.Data.StartDateTime = date.Value.Date;
                vm.Data.EndDateTime = date.Value.Date;
            }

            if (pid.HasValue)
            {
                vm.Data.Eds12WeekProgramId = pid.Value;
            }            

            var templates = await _trrepo.GetTemplateSessions();
            List<SelectListItem> list_templates = templates.Select(r => new SelectListItem()
            {
                Text = $"{r.Name} (ID: {r.Id})",
                Value = r.Id.ToString(),
                Selected = id.HasValue ? r.Id == id.Value : false
            }).ToList();
            vm.Select_Template = list_templates;

            var ExerciseTypes = await _lookup.GetExerciseTypes();
            List<SelectListItem> list_extypes = (from extype in ExerciseTypes
                                                 select new SelectListItem()
                                                 {
                                                     Text = extype.Name,
                                                     Value = extype.Id.ToString()
                                                 }).ToList();
            vm.Select_ExerciseTypes = list_extypes;

            var MetricTypes = await _lookup.GetSetMetricTypes();
            List<SelectListItem> list_metrictypes = (from mettype in MetricTypes
                                                     select new SelectListItem()
                                                     {
                                                         Text = mettype.Name,
                                                         Value = mettype.Id.ToString()
                                                     }).ToList();
            vm.Select_MetricTypes = list_metrictypes;

            var programs = await _trrepo.GetAllProgramsForUser(vm.CurrentUser.Id);
            List<SelectListItem> list_programs = (from program in programs
                                                  select new SelectListItem()
                                                  {
                                                      Text = program.Name + (program.IsCurrent ? "(Current)" : String.Empty),
                                                      Value = program.Id.ToString()
                                                  }).ToList();
            vm.Select_Program = list_programs;

            return View(vm);
        }

        private async Task<TrainingSessionViewModel> GetDummyTrainingData()
        {
            string skey_dummydata = "skey_dummydata";

            List<EdsExerciseType> list_exerciseTypes = await _lookup.GetExerciseTypesAllData();
            List<EdsSetMetricTypes> list_metricTypes = await _lookup.GetSetMetricTypes();

            TrainingSessionViewModel vm = new TrainingSessionViewModel();
            vm.CurrentUser = new User()
            {
                Id = 1,
                FirstName = "Cid",
                LastName = "Dummy",
                Email = "cidthedummy@test.com",
                Mobile = "0923 123 1231"
            };

            vm.CurrentProgram = new Eds12weekPlan()
            {
                FkCustomerId = 1,
                Name = "Ultra-Slimfit Program",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(7 * 12),
                DurationWeeks = 12,
                IsCurrent = true
            };

            vm.SelectedDate = DateTime.Now.Date;

            Random r = new Random();
            int maxExercises = 5;
            int maxSets = 5;
            int maxMetrics = 5;
            int numSessions = r.Next(10);
            
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
                    currExercise.FkExerciseType = list_exerciseTypes[r.Next(list_exerciseTypes.Count)];
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
                                ActualCustomMetric = r.Next(32),
                                FkMetricsType = list_metricTypes[r.Next(list_metricTypes.Count)]
                            };
                            currSet.EdsSetMetrics.Add(currMetric);
                        }
                        currExercise.EdsSet.Add(currSet);
                    }
                    currSession.EdsExercise.Add(currExercise);
                }
                vm.TrainingSessions.Add(currSession);
            }

            return vm;
        }

        private async Task<TrainingSessionViewModel?> FillTrainingIndexViewModel(long userId, long? pid = null, DateTime? date = null)
        {
            try
            {
                User? currentUser = await _trrepo.GetClientById(userId);
                if (currentUser == null)
                {
                    _logger.LogError($"Failed to retrieve user id {userId}");
                    return null;
                }

                TrainingSessionViewModel vm = new TrainingSessionViewModel();
                vm.CurrentUser = currentUser;
                vm.IdentityUser = await _clientrepo.GetIdentityUserById(currentUser.FkFederatedUser);

                if (pid.HasValue)
                {
                    Eds12weekPlan? currentProgram = await _trrepo.GetProgramById(pid.Value);
                    if (currentProgram == null)
                    {
                        _logger.LogError($"Failed to retrieve 12week plan from program id {pid}");
                        return null;
                    }
                    vm.CurrentProgram = currentProgram;
                    vm.SelectedProgramId = currentProgram.Id;

                    if (date.HasValue)
                    {
                        vm.SelectedDate = date;
                        vm.TrainingSessions.AddRange(await _trrepo.GetTrainingSessionsByDate(currentUser.Id, date.Value));
                    }
                    else
                    {
                        vm.TrainingSessions.AddRange(await _trrepo.GetAllTrainingSessionsInProgram(pid.Value));
                    }
                }
                
                vm.Programs = await _trrepo.GetAllProgramsForUser(currentUser.Id);
                vm.List_Programs = (from program in vm.Programs
                                    select new SelectListItem()
                                    {
                                        Text = program.Name + (program.IsCurrent ? "(Current)" : String.Empty),
                                        Value = program.Id.ToString(),
                                        Selected = vm.CurrentProgram != null ? program.Id == vm.CurrentProgram.Id : false,                                       
                                    }).ToList();

                return vm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FillTrainingIndexViewModel failed");
                Debug.Write(ex.ToString());
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> Chart(long userId, long? selectedProgram = null)
        {
            TrainingGraphViewModel vm = new TrainingGraphViewModel();

            var user = await _trrepo.GetClientById(userId);
            if (user == null)
            {
                _logger.LogWarning("Cannot find user {id}.", userId);
                return RedirectToAction("Index", "Home");
            }

            vm.CurrentUser = user;

            vm.Programs = await _trrepo.GetAllProgramsForUser(user.Id);
            if (vm.Programs == null || vm.Programs.Count == 0)
            {
                _logger.LogWarning("TrainingProgram: Cannot find in the user {id}.", userId);
                return View(vm);
            }

            vm.CurrentProgram = vm.Programs.Find(item => item.IsCurrent == true);

            selectedProgram ??= vm.Programs.Find(item => item.IsCurrent).Id;

            vm.List_Programs = (from program in vm.Programs
                                select new SelectListItem()
                                {
                                    Text = program.Name + (program.IsCurrent ? "(Current)" : String.Empty),
                                    Value = program.Id.ToString(),
                                    Selected = vm.CurrentProgram != null ? program.Id == selectedProgram : false,
                                }).ToList();

            foreach (var programs in vm.Programs)
            {
                if (programs.Id == selectedProgram)
                {
                    List<EdsTrainingSession> trainingSessions = new List<EdsTrainingSession>();
                    trainingSessions.AddRange(await _trrepo.GetAllTrainingSessionsInProgram(programs.Id));

                    int weekIndex = 1;
                    foreach (var week in programs.EdsWeeklyPlan)
                    {
                        var chartData = new TrainingChartDisplayData();
                        chartData.Name = "Week " + weekIndex++ + " ( " + week.StartDate.GetValueOrDefault().ToShortDateString() + " - " + week.EndDate.GetValueOrDefault().ToShortDateString() + " )";
                        chartData.Id = week.Id.ToString();

                        int dayIndex = 1;
                        List<EdsExercise> exercise_select = new List<EdsExercise>();
                        foreach (var daily in week.EdsDailyPlan)
                        {
                            var nameDay = "Day " + dayIndex++ + "(" + daily.StartDay.GetValueOrDefault().ToShortDateString() + ")" + daily.Id;

                            foreach (var item in daily.EdsTrainingSession)
                            {
                                var session = trainingSessions.Find(x => x.Id == item.Id);

                                foreach (var exercise in session.EdsExercise)
                                {
                                    exercise_select.Add(exercise);  
                                }
                            }
                        }

                        var queryData = exercise_select.GroupBy(x => x.FkExerciseType.Name).Select(y => y).ToList();
                        foreach (var item in queryData)
                        {
                            chartData.GraphDataTotal.Add(item.Select(y => y.EdsSet.Count()).Sum());
                            chartData.GraphDataComplete.Add(item.Select(y => y.EdsSet.Count(r => r.IsComplete)).Sum());
                            chartData.GraphDataSkipped.Add(item.Select(y => y.EdsSet.Count(r => r.IsSkipped)).Sum());
                            chartData.GraphDataInComplete.Add(item.Select(y => y.EdsSet.Count(r => !r.IsComplete && !r.IsSkipped)).Sum());
                            chartData.GraphLabel.Add(item.Key.ToString());
                        }
                        vm.TrainingListChartDisplayData.Add(chartData);
                    }
                }
            }

            return View(vm);
        }
    }
}
