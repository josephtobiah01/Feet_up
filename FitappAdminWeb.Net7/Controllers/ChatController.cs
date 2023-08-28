using FitappAdminWeb.Net7.Classes.Base;
using FitappAdminWeb.Net7.Classes.Repositories;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitappAdminWeb.Net7.Controllers
{
    public class ChatController : BaseController
    {
        MessageRepository _messagerepo;
        ILogger<ChatController> _logger;

        public ChatController(MessageRepository messagerepo,
                              ILogger<ChatController> logger)
            : base(messagerepo)
        {
            _messagerepo = messagerepo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Rooms(bool WithConcernsOnly = false)
        {
            RoomSelectViewModel vm = new RoomSelectViewModel();
            vm.CurrentUser = await _messagerepo.GetUserByLoggedInUserName(User);
            vm.Rooms = await _messagerepo.GetRooms(WithConcernsOnly, 1);

            return View(vm);
        } 

        [HttpGet]
        public async Task<IActionResult> RoomsWithConcerns()
        {
            //NOTE: This is Room Selection Mode (GetRoomWithConcerns)
            RoomSelectViewModel vm = new RoomSelectViewModel();
            vm.CurrentUser = await _messagerepo.GetUserByLoggedInUserName(User);
            vm.Rooms = await _messagerepo.GetRoomsWithConcerns() ?? new List<DAOLayer.Net7.Chat.MsgRoom>();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Room(long id)
        {
            SpecificUserChatViewModel vm = new SpecificUserChatViewModel();
            vm.CurrentRoom = await _messagerepo.GetRoomById(id); //await _messagerepo.GetRoomByRoomId(id);

            if (Request.Query.ContainsKey("error"))
            {
                ViewData["error"] = Request.Query["error"];
            }

            if (vm.CurrentRoom != null) 
            {
                vm.LoggedInUser = await _messagerepo.GetUserByLoggedInUserName(User);
                vm.RoomOwner = await _messagerepo.GetUserById(vm.CurrentRoom.FkUserId);
                vm.Messages = await _messagerepo.API_GetMessages(vm.CurrentRoom.Id);
                vm.Data.RoomId = vm.CurrentRoom.Id;
            }
            
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageChatModel Data)
        {
            try
            {
                var currentUser = await _messagerepo.GetUserByLoggedInUserName(User);

                if (ModelState.IsValid && currentUser != null)
                {
                    await _messagerepo.API_SendMessageToRoom(Data.RoomId, currentUser.FkFederatedUser, Data.Message, Data.MarkAsHandled, Data.ImageContentType, Data.ImageContent);
                    return RedirectToAction("Room", new { id = Data.RoomId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message");
            }

            return RedirectToAction("Room", new { id = Data.RoomId, error = "chatfailed"});
        }
    }
}
