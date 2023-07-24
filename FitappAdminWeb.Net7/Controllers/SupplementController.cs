using AutoMapper;
using DAOLayer.Net7.Supplement;
using FitappAdminWeb.Net7.Classes.Base;
using FitappAdminWeb.Net7.Classes.Repositories;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParentMiddleWare.Models;
using System.Collections.Immutable;
using System.Linq;

namespace FitappAdminWeb.Net7.Controllers
{
    public class SupplementController : BaseController
    {
        private const string SKEY_CURRENTUSER = "skey_currentuser";

        SupplementRepository _supprepo;
        MessageRepository _messagerepo;
        ClientRepository _clientrepo;
        LookupRepository _lookup;
        IMapper _mapper;
        ILogger<SupplementController> _logger;

        public SupplementController(SupplementRepository supprepo,
                                    ClientRepository clientrepo,
                                    LookupRepository lookup,
                                    IMapper mapper,
                                    ILogger<SupplementController> logger,
                                    MessageRepository messagerepo)
            : base(messagerepo)
        {
            _supprepo = supprepo;
            _clientrepo = clientrepo;
            _lookup = lookup;
            _mapper = mapper;
            _logger = logger;
            _messagerepo = messagerepo;
        }

        [HttpGet]
        public async Task<IActionResult> SupplementsWeekly(long id)
        {
            if (id != 0)
            {
                HttpContext.Session.SetInt32(SKEY_CURRENTUSER, (int)id);
            }

            int? currentUserId = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);
            if (currentUserId.HasValue)
            {
                SupplementWeeklyPlanListViewModel vm = new SupplementWeeklyPlanListViewModel();
                vm.CurrentUser = await _supprepo.GetUserByUserId(currentUserId.Value);

                vm.UserList = (await _supprepo.GetUsersList()).Select(r => new SelectListItem()
                {
                    Text = $"{r.FirstName} {r.LastName} (ID: {r.Id})",
                    Value = r.Id.ToString()
                }).ToList();

                if (vm.CurrentUser != null)
                {
                    vm.UserIdentity = await _clientrepo.GetIdentityUserById(vm.CurrentUser.FkFederatedUser);
                    vm.SupplementWeeklyPlanList = await _supprepo.GetWeeklyPlansByUserId(currentUserId.Value);

                    var supprefIdList = from suppPlan in vm.SupplementWeeklyPlanList
                                           from dailyPlan in suppPlan.NdsSupplementPlanDaily
                                           from supplement in dailyPlan.NdsSupplementPlanSupplement
                                           select supplement.FkSupplementReference;

                    vm.SupplementReference = await _supprepo.GetSupplementReferenceByIdList(supprefIdList.ToList());
                }
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditPlan(long? id, bool copy = false)
        {
            SupplementWeeklyPlanEditViewModel vm = new SupplementWeeklyPlanEditViewModel();

            NdsSupplementPlanWeekly loadedPlan = null;
            if (id.HasValue)
            {
                loadedPlan = await _supprepo.GetSupplementPlanById(id.Value);
            }

            if (loadedPlan != null)
            {
                vm.Data = _mapper.Map<SupplementWeeklyPlanEditFormModel>(loadedPlan);
                if (copy)
                {
                    int? sessionUser = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);
                    vm.CurrentUser = await _supprepo.GetUserByUserId(sessionUser ?? 0);
                }
                else
                {
                    vm.CurrentUser = await _supprepo.GetUserByUserId(loadedPlan.FkCustomerId);
                }
            }
            else
            {
                int? sessionUser = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);
                vm.CurrentUser = await _supprepo.GetUserByUserId(sessionUser ?? 0);
            }

            vm.IsCopy = copy;

            //select lists
            var suppTypes = await _supprepo.GetSupplementReferenceIds();
            vm.SupplementList = suppTypes.Select(r => new SelectListItem() { Text = r.Name, Value = r.Id.ToString() }).ToList();

            var metricList = await _supprepo.GetUnitMetricList();
            vm.UnitMetricList = metricList.Select(r => new SelectListItem() { Text = r.Name, Value = r.Id.ToString() }).ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> SupplementPlanChart(long userId, long planId)
        {
            if (userId != 0)
            {
                HttpContext.Session.SetInt32(SKEY_CURRENTUSER, (int)userId);
            }

            int? currentUserId = HttpContext.Session.GetInt32(SKEY_CURRENTUSER);
            if (currentUserId.HasValue)
            {
                SupplementPlanChartModel vm = new SupplementPlanChartModel();
                vm.CurrentUser = await _supprepo.GetUserByUserId(currentUserId.Value);

                vm.UserList = (await _supprepo.GetUsersList()).Select(r => new SelectListItem()
                {
                    Text = $"{r.FirstName} {r.LastName} (ID: {r.Id})",
                    Value = r.Id.ToString()
                }).ToList();

                if (vm.CurrentUser != null)
                {
                    vm.SupplementWeeklyPlanList = await _supprepo.GetWeeklyPlansByUserId(currentUserId.Value);

                    var supprefIdList = from suppPlan in vm.SupplementWeeklyPlanList
                                        from dailyPlan in suppPlan.NdsSupplementPlanDaily
                                        from supplement in dailyPlan.NdsSupplementPlanSupplement
                                        select supplement.FkSupplementReference;

                    vm.SupplementReference = await _supprepo.GetSupplementReferenceByIdList(supprefIdList.ToList());


                    foreach (var value in vm.SupplementReference)
                    {
                        var values = new supplementValues();
                        values.suppId = value.Id;
                        values.Name = value.Name;
                        vm.supplementValues.Add(values);
                    }



                    var suppTypes = await _supprepo.GetSupplementReferenceIds();
                    vm.SupplementList = suppTypes.Select(r => new SelectListItem() { Text = r.Name, Value = r.Id.ToString() }).ToList();

                    var metricList = await _supprepo.GetUnitMetricList();
                    vm.UnitMetricList = metricList.Select(r => new SelectListItem() { Text = r.Name, Value = r.Id.ToString() }).ToList();

                }
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
