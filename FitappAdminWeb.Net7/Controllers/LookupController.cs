using AutoMapper;
using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Supplement;
using FitappAdminWeb.Net7.Classes.Base;
using FitappAdminWeb.Net7.Classes.Constants;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Classes.Repositories;
using FitappAdminWeb.Net7.Classes.Utilities;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitappAdminWeb.Net7.Controllers
{
    public class LookupController: BaseController
    {
        public const int NUM_PAGES = 5;

        LookupRepository _lookup;
        SupplementRepository _supprep;
        TrainingRepository _trrepo;
        MessageRepository _messagerepo;
        BlobStorageRepository _blobrepo;
        IMapper _mapper;
        ILogger<LookupController> _logger;

        public LookupController(LookupRepository lookup,
                                SupplementRepository supprep,
                                IMapper mapper,
                                ILogger<LookupController> logger,
                                MessageRepository messagerepo,
                                TrainingRepository trrepo,
                                BlobStorageRepository blobrepo)
            : base(messagerepo)
        {
            _lookup = lookup;
            _mapper = mapper;
            _logger = logger;
            _supprep = supprep;
            _messagerepo = messagerepo;
            _trrepo = trrepo;
            _blobrepo = blobrepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region Exercise Type
        [HttpGet]
        public async Task<IActionResult> ExerciseType(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var exerTypesList = await _lookup.GetExerciseTypesAllData();
            ExerciseTypeListViewModel vm = new ExerciseTypeListViewModel()
            {
                EdsExerciseTypes = exerTypesList
            };
            vm.PagingEdsExerciseTypes = new X.PagedList.PagedList<EdsExerciseType>(exerTypesList, pageNumber, pageSize);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditExerciseType(long? id = null)
        {
            ExerciseTypeEditViewModel vm = new ExerciseTypeEditViewModel();
            if (id.HasValue)
            {
                var currExerType = await _lookup.GetSingleExerciseTypeAllData(id.Value);
                if (currExerType != null)
                {
                    vm.Data = _mapper.Map<ExerciseType_DTO>(currExerType);
                }
            }
            else vm.Data = new ExerciseType_DTO();

            //load all lookups (consider loading these in parallel)
            vm.Equipment_List = (await _lookup.GetEdsEquipmentAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.Force_List = (await _lookup.GetEdsForceAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.ExerciseClass_List = (await _lookup.GetEdsExerciseClassAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.Level_List = (await _lookup.GetEdsLevelAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.MainMuscle_List = (await _lookup.GetEdsMainMuscleWorkedAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.OtherMuscle_List = (await _lookup.GetEdsOtherMuscleWorkedAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.Sport_List = (await _lookup.GetEdsSportAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.MechanicsType_List = (await _lookup.GetMechanicsTypeAll()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString() 
                                }).ToList();
            vm.SetMetricType_List = (await _lookup.GetSetMetricTypes()).Select(r => new SelectListItem() { 
                                    Text = r.Name, Value = r.Id.ToString()
                                }).ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditExerciseType(ExerciseType_DTO Data)
        {
            if (Data.Id == 0)
            {
                EdsExerciseType exType = await _lookup.AddExerciseType(Data);              
            }
            else
            {
                EdsExerciseType exType = await _lookup.EditExerciseType(Data);
            }
            return RedirectToAction("ExerciseType");
        }

        [HttpPost]
        public async Task<JsonResult> UploadImage( IFormFile image)
        {
            try
            {
                byte[]? imageBytes = ImageUtil.GetBytesFromImageFormFile(image, UserConstants.EXTYPE_UPLOADIMAGE_SIZELIMIT, true,
                    UserConstants.EXTYPE_UPLOADIMAGE_MAX_WIDTH, UserConstants.EXTYPE_UPLOADIMAGE_MAX_HEIGHT);
                string contentType = image.ContentType;

                if (imageBytes != null)
                {
                    return Json(await _blobrepo.UploadImageUrl(imageBytes, contentType));
                }
                _logger.LogWarning("Failed to upload image. Returning null.");
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed Upload Image");
                return Json(string.Empty);
            }
        }
        #endregion

        #region Templates
        [HttpGet]
        public async Task<IActionResult> TrainingSessionTemplates(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var templateList = await _trrepo.GetTemplateSessionsWithExercises();
            TrainingSessionTemplateListViewModel vm = new TrainingSessionTemplateListViewModel()
            {
                TemplateSessions = templateList
            };
            vm.PagingTemplateSessions = new X.PagedList.PagedList<EdsTrainingSession>(templateList, pageNumber, pageSize);
            return View(vm);
        }
        #endregion

        #region Supplement Reference

        [HttpGet]
        public async Task<IActionResult> SupplementReference(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            SupplementReferenceListViewModel vm = new SupplementReferenceListViewModel()
            {
                Page = page
            };

            List<NdsSupplementReference> supprefList = await _supprep.GetSupplementReferences();
            vm.NdsSupplementReferences = supprefList;
            vm.PagingNdsSupplementReferences = new X.PagedList.PagedList<NdsSupplementReference>(supprefList, pageNumber, pageSize);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditSupplementReference(long? id)
        {
            //NOTE: Supplement Reference here will be added without adding Legal Status. Consider adding it afterwards, maybe like a wizard after saving.
            //NdsSupplementInstruction will always be created/edited and should not be reused (unless specified)
            SupplementReferenceEditViewModel vm = new SupplementReferenceEditViewModel();

            if (id.HasValue)
            {
                //load Supplement Reference here
                var current_suppref = await _supprep.GetSupplementReferenceById(id.Value);
                if (current_suppref != null)
                {
                    vm.Data = _mapper.Map<SupplementReference_DTO>(current_suppref);
                }
            }

            var unitmetric_list = await _supprep.GetUnitMetricList();
            vm.UnitMetric_List = unitmetric_list.Select(r => new SelectListItem() {
                Text = r.Name,
                Value = r.Id.ToString()
            }).ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditSupplementReference(SupplementReference_DTO Data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NdsSupplementReference suppRef = null;
                    if (Data.Id == 0)
                    {
                        suppRef = await _supprep.AddSupplementReference(Data);
                        return RedirectToAction("SupplementReference");
                    }
                    else
                    {
                        suppRef = await _supprep.EditSupplementReference(Data);
                        return RedirectToAction("SupplementReference");
                    }   
                }
                return RedirectToAction("EditSupplementReference", ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed Add/Edit Supplement Reference");
                throw;
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> EditLegalStatus()
        {
            //NOTE: Legal Status will be added seperately from SupplementReference as it is too
            //complex to be condensed with SupplementReference in a single page. 
            return null;
        }
        #endregion
    }
}
