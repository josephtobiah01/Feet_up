using AutoMapper;
using DAOLayer.Net7.Exercise;
using FitappAdminWeb.Net7.Classes.Base;
using FitappAdminWeb.Net7.Classes.Repositories;
using FitappAdminWeb.Net7.Classes.Utilities;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitappAdminWeb.Net7.Controllers
{
    public class UserController : BaseController
    {
        ClientRepository _clientrepo;
        MessageRepository _messagerepo;
        TrainingRepository _trrepo;
        ILogger<UserController> _logger;
        IMapper _mapper;

        public UserController(ClientRepository clientrepo,
                              ILogger<UserController> logger,
                              TrainingRepository trrepo,
                              MessageRepository messagerepo,
                              IMapper mapper)
            : base(messagerepo)
        {
            _clientrepo = clientrepo;
            _logger = logger;
            _trrepo = trrepo;
            _messagerepo = messagerepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //add filter
            bool includeInactive = false;
            if (Request.Query.ContainsKey("test"))
            {
                bool.TryParse(Request.Query["test"].ToString(), out includeInactive);
            }

            var userList = await _clientrepo.GetAllClients(includeInactive);
            var fedIdList = userList.Select(r => r.FkFederatedUser).ToList();
            var idUserList = await _clientrepo.GetIdentityUsersWithFederatedList(fedIdList);

            UserListViewModel vm = new UserListViewModel()
            {
                Users = userList, 
                Id_Users = idUserList
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var vm = await GenerateUserViewModel(id);
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddInternalNote(AddInternalNoteModel AddNote)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = await _clientrepo.GetUserByLoggedInUserName(User);
                    if (loginUser != null)
                    {
                        bool isNoteAdded = await _clientrepo.AddInternalNote(AddNote.ForUserId, loginUser.Id, AddNote.Note);
                        if (isNoteAdded)
                        {
                            return RedirectToAction("Details", new { id = AddNote.ForUserId, success = "addnote" });
                        }
                    }
                }              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add Internal Note");                
            }
            return RedirectToAction("Details", new { id = AddNote.ForUserId, error = "addnotefailed" });
        }

        private async Task<UserDetailViewModel?> GenerateUserViewModel(long userId)
        {
            UserDetailViewModel vm = new UserDetailViewModel();

            var user = await _clientrepo.GetClientById(userId);
            if (user == null)
            {
                return null;
            }
            vm.CurrentUser = user;
            vm.UserIdentity = await _clientrepo.GetIdentityUserById(user.FkFederatedUser);
            vm.AddNote.ForUserId = user.Id;

            vm.InternalNotes = await _clientrepo.GetInternalNotesForUserId(user.Id);
            vm.Room = await _messagerepo.EnsureChatRoomByUserId(user.Id);

            return vm;
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(long id)
        {
            var user = await _clientrepo.GetClientById(id);
            if (user == null)
            {
                _logger.LogError("Cannot find user to edit {id}", id);
                return NotFound();
            }

            var vm = _mapper.Map<UserEditModel>(user);
            vm.CountryList = ListUtil.CountryList().Select(r => new SelectListItem()
            {
                Text = r.Value.Trim(),
                Value = r.Key.Trim()
            }).OrderBy(r => r.Text).ToList();

            //vm.CountryList = new SelectList(ListUtil.CountryList(), "Key", "Value").OrderBy(r => r.Text).ToList();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _clientrepo.EditUser(model);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user id {id}", model.Id);
                return View(model);
            }
        }
    }
}
