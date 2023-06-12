using AutoMapper;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Classes.Repositories;
using DAOLayer.Net7.Exercise;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FitappAdminWeb.Net7.Models;
using DAOLayer.Net7.Supplement;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using DAOLayer.Net7.Nutrition;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FitappAdminWeb.Net7.Controllers.API
{
    /// <summary>
    /// API controller for JSON data calls.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        TrainingRepository _trrepo;
        ClientRepository _clientrepo;
        LookupRepository _lookup;
        SupplementRepository _supprepo;
        NutritionRepository _nutrepo;
        ILogger<DataController> _logger;
        IMapper _mapper;

        public DataController(TrainingRepository trrepo, 
            ClientRepository clientrepo, 
            IMapper mapper, 
            ILogger<DataController> logger, 
            LookupRepository lookup, 
            SupplementRepository supprepo,
            NutritionRepository nutrepo)
        {
            _trrepo = trrepo;
            _clientrepo = clientrepo;
            _mapper = mapper;
            _logger = logger;
            _lookup = lookup;
            _supprepo = supprepo;
            _nutrepo = nutrepo;
        }

        #region Training
        [HttpGet("GetExercisesbyTrainingSession")]
        public async Task<ActionResult<IEnumerable<ExerciseDTO>>> GetExercisesbyTrainingSession([FromQuery] long sessionid, [FromQuery] bool copy)
        {
            List<EdsExercise> result = await _trrepo.GetExercisesByTrainingSessionId(sessionid);

            //map to DTOs
            List<ExerciseDTO> resultDTO = new List<ExerciseDTO>();
            foreach (var exercise in result)
            {
                if (copy)
                {
                    //set all ids to 0 so it is effectively a "new" entity
                    //set all flags to false
                    exercise.Id = 0;
                    exercise.FkTraining = null;
                    exercise.FkTrainingId = 0;
                    exercise.IsComplete = exercise.IsSkipped = exercise.IsCustomerAddedExercise = false;

                    foreach (var set in exercise.EdsSet)
                    {
                        set.Id = 0;
                        set.FkExercise = null;
                        set.FkExerciseId = 0;
                        set.IsComplete = set.IsSkipped = set.IsCustomerAddedSet = false;

                        foreach (var metric in set.EdsSetMetrics)
                        {
                            //set actual metric to 0
                            metric.Id = 0;
                            metric.FkSet = null;
                            metric.FkSetId = 0;
                            metric.ActualCustomMetric = null;
                        }
                    }
                }

                var mappedObj = _mapper.Map<ExerciseDTO>(exercise);
                resultDTO.Add(mappedObj);             
            }
            return resultDTO;
        }

        [HttpGet("GetDummyExercisesByTrainingSession")]
        public async Task<ActionResult<IEnumerable<EdsExercise>>> GetDummyExercisesByTrainingSession([FromQuery] long userId, [FromQuery] long sessionid)
        {
            return await _trrepo.GetDummyExercisesList();
        }

        [HttpPost("addtrainingsession")]
        public async Task<ActionResult<EdsTrainingSession>?> AddTrainingSession([FromBody] TrainingSessionDTO data)
        {
            try
            {
                EdsTrainingSession? result = await _trrepo.AddTrainingSession(data.UserId, data.Eds12WeekPlanId, data);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add training session");
                return null;
            }
        }

        [HttpPost("edittrainingsession")]
        public async Task<ActionResult<EdsTrainingSession>?> EditTrainingSession([FromBody] TrainingSessionDTO data)
        {
            try
            {
                EdsTrainingSession? result = await _trrepo.EditTrainingSession(data.UserId, data.Eds12WeekPlanId, data);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to edit training session");
                return null;
            }
        }

        [HttpPost("deletetrainingsession")]
        public async Task<ActionResult<bool>> DeleteTrainingSession([FromBody] long id)
        {
            try
            {
                return await _trrepo.DeleteTrainingSession(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete session with id {id}", id);
                return false;
            }
        }

        [HttpPost("deleteplan")]
        public async Task<ActionResult<bool>> DeletePlan([FromBody] long id)
        {
            try
            {
                return await _trrepo.DeleteProgram(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete plan with id {id}", id);
                return false;
            }
        }

        [HttpPost("deleteexercisetype")]
        public async Task<ActionResult<bool>> DeleteExerciseType([FromBody] long id)
        {
            try
            {
                return await _lookup.DeleteExerciseType(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete plan with id {id}", id);
                return false;
            }
        }


        [HttpGet("getplanschedulebyid")]
        public async Task<ActionResult<IEnumerable<WeeklyPlanDTO>>> GetPlanScheduleById([FromQuery] long planid)
        {
            try
            {
                List<EdsWeeklyPlan> weeklyplans = await _trrepo.GetWeeklyPlansByProgramId(planid);
                //map to DTOs
                List<WeeklyPlanDTO> weeklyplanDTO = new List<WeeklyPlanDTO>();
                foreach (var exercise in weeklyplans)
                {
                    weeklyplanDTO.Add(_mapper.Map<WeeklyPlanDTO>(exercise));
                }
                return weeklyplanDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot retrieve plan schedule from program id {planid}", planid);
                return new List<WeeklyPlanDTO>();
            }
        }

        [HttpPost("addplan")]
        public async Task<ActionResult<Eds12weekPlan>?> AddPlan([FromBody] ProgramDTO plan)
        {
            try
            {
                Eds12weekPlan? result = null;
                if (plan.TemplateId > 0)
                {
                    result = await _trrepo.CreatePlanFromTemplate(plan.TemplateId, plan, true);
                }
                else
                {
                    result = await _trrepo.AddProgram(plan);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot add plan");
                return null;
            }
        }

        [HttpPost("editplan")]
        public async Task<ActionResult<Eds12weekPlan?>> EditPlan([FromBody] ProgramDTO plan)
        {
            try
            {
                return await _trrepo.EditProgram(plan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot edit plan");
                return null;
            }
        }

        [HttpGet("getsetdefaults")]
        public async Task<ActionResult<List<EdsSetDefaults>>> GetSetDefaults([FromQuery] long extypeid)
        {
            try
            {
                return await _lookup.GetSetDefaultsForExerciseType(extypeid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed GetSetDefault");
                return new List<EdsSetDefaults>();
            }
        }

        [HttpGet("getexercisetypechoices")]
        public async Task<ActionResult<List<AutoCompleteChoiceDTO>>> GetExerciseTypeChoices()
        {
            var exertype_list = await _lookup.GetExerciseTypes();

            List<AutoCompleteChoiceDTO> result = exertype_list.Select(r => new AutoCompleteChoiceDTO()
            {
                label = r.Name,
                value = r.Id.ToString(),
                disabled = false,
                selected = false
            }).ToList();

            return result;
        }

        [HttpPost("addexercisetype")]
        public async Task<ActionResult<EdsExerciseType?>> AddExerciseType([FromBody] ExerciseType_DTO exertype)
        {
            try
            {
                return await _lookup.AddExerciseType(exertype);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add exercise Type");
                return null;
            }
        }

        [HttpPost("editexercisetype")]
        public async Task<ActionResult<EdsExerciseType?>> EditExerciseType([FromBody] ExerciseType_DTO exertype)
        {
            try
            {
                return await _lookup.EditExerciseType(exertype);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add exercise Type");
                return null;
            }
        }

        [HttpGet("getallsetdefaults")]
        public async Task<ActionResult<List<SetDefault_DTO>>> GetAllSetDefaults() 
        {
            try
            {
                List<SetDefault_DTO> result_list = new List<SetDefault_DTO>();
                var result = await _lookup.GetAllSetDefaults();
                foreach (var item in result)
                {
                    result_list.Add(_mapper.Map<SetDefault_DTO>(item));
                }
                return result_list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get set defaults");
                return new List<SetDefault_DTO>();

            }
        }

        [HttpPost("copyweek")]
        public async Task<ActionResult<CopyWeekResult_DTO>> CopyWeek([FromBody] CopyWeekFormModel copyweek)
        {
            try
            {
                var newWeek = await _trrepo.CopyWeek(copyweek.WeekId.GetValueOrDefault(), copyweek.StartDay.GetValueOrDefault());
                if (newWeek == null)
                {
                    return new CopyWeekResult_DTO() { Success = false };
                }

                return new CopyWeekResult_DTO() { Success = true, WeekId = newWeek.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to copy week");
                return new CopyWeekResult_DTO() { Success = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Supplement
        [HttpPost("addsupplementplan")]
        public async Task<ActionResult<NdsSupplementPlanWeekly?>> AddSupplementPlan([FromBody] SupplementPlanWeekly_DTO data)
        {
            try
            {
                return await _supprepo.AddSupplementPlanWeekly(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add supplement plan");
                return null;
            }
        }

        [HttpPost("editsupplementplan")]
        public async Task<ActionResult<NdsSupplementPlanWeekly?>> EditSupplementPlan([FromBody] SupplementPlanWeekly_DTO data)
        {
            try
            {
                return await _supprepo.EditSupplementPlanWeekly(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to edit supplement plan");
                return null;
            }
        }

        [HttpPost("deletesupplementplan")]
        public async Task<ActionResult<bool>> DeleteSupplementPlan([FromBody] long planId)
        {
            try
            {
                return await _supprepo.DeleteSupplementPlanWeekly(planId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete supplement plan {id}", planId);
                return false;
            }
        }

        [HttpGet("getsupplementplanschedulebyplanid")]
        public async Task<ActionResult<List<SupplementPlanDaily_DTO>?>> GetSupplementPlanScheduleByPlanId(long planId)
        {
            try
            {
                List<NdsSupplementPlanDaily> dailyPlanList = await _supprepo.GetPlanScheduleByPlanId(planId) ?? new List<NdsSupplementPlanDaily>();
                List<SupplementPlanDaily_DTO> dto = new List<SupplementPlanDaily_DTO>();

                foreach (var plan in dailyPlanList)
                {
                    dto.Add(_mapper.Map<SupplementPlanDaily_DTO>(plan));
                }
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot retrieve supplement daily plans by weekly plan id {id}", planId);
                return null;
            }
        }

        [HttpGet("getplanslistbyuser")]
        public async Task<ActionResult<List<SelectListItem>>> GetPlansListByUser(long userId)
        {
            return (await _supprepo.GetPlansListByUser(userId)).Select(r => new SelectListItem()
            {
                Text = $"Plan {r.Id}",
                Value = r.Id.ToString()
            }).ToList();
        }
        #endregion

        #region Nutrition
        [HttpGet("getmealsinday")]
        public async Task<ActionResult<List<ActualMeal_DTO>>> GetMealsInDay(long dayId)
        {
            var mealList = await _nutrepo.GetMealsInDay(dayId);
            List<ActualMeal_DTO> result = new List<ActualMeal_DTO>();
            
            foreach (var meal in mealList)
            {
                result.Add(_mapper.Map<ActualMeal_DTO>(meal));
            }
            return result;
        }

        [HttpPost("adddailyfoodplan")]
        public async Task<ActionResult<bool>> AddDailyFoodPlan([FromBody] ActualDay_DTO input)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState Error. Errors: {count}", ModelState.ErrorCount);
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var success = await _nutrepo.AddNutritionalDailyPlans(input);
            return success;
        }

        [HttpPost("editdailyfoodplan")]
        public async Task<ActionResult<bool>> EditDailyFoodPlan([FromBody] ActualDay_DTO input)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState Error. Errors: {count}", ModelState.ErrorCount);
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var success = await _nutrepo.EditNutritionalDailyPlans(input);
            return success;
        }

        [HttpGet("getdishdetails")]
        public async Task<ActionResult<FnsNutritionActualDish?>> GetDishDetails(long id)
        {
            return await _nutrepo.GetActualDishById_MinimalInclude(id);
        }

        [HttpGet("getdishlist")]
        public async Task<ActionResult<List<SelectListItem>>> GetDishList(long userid)
        {
            return await _nutrepo.GetDishListSelectItems(userid);
        }
        #endregion
    }
}
