using FitappAdminWeb.Net7.Classes.Repositories;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using FitappAdminWeb.Net7.Classes.Base;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using FitappAdminWeb.Net7.Classes.Utilities;
using System.Drawing;
using System.Drawing.Imaging;

namespace FitappAdminWeb.Net7.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private ClientRepository _clientrepo;
        private MessageRepository _messagerepo;
        private IMapper _mapper;
        private PromotionRepository _promorepo;

        private readonly int UPLOAD_BYTELIMIT = 4194304;
        private readonly int PROMOTION_IMG_WIDTH_LIMIT = 343;
        private readonly int PROMOTION_IMG_HEIGHT_LIMIT = 140;

        public HomeController(ILogger<HomeController> logger,
            ClientRepository clientrepo,
            MessageRepository messagerepo,
            PromotionRepository promorepo,
            IMapper mapper)
            : base(messagerepo)
        {
            _logger = logger;
            _clientrepo = clientrepo;
            _messagerepo = messagerepo;
            _mapper = mapper;
            _promorepo = promorepo;
        }

        public async Task<IActionResult> Index(bool IsTest)
        {
            var userList = await _clientrepo.GetAllClients(IsTest);
            var fedIdList = userList.Select(r => r.FkFederatedUser).ToList();
            var idUserList = await _clientrepo.GetIdentityUsersWithFederatedList(fedIdList);
            var roomList = await _messagerepo.GetRooms_Minimal(userList.Select(r => r.Id).ToList());

            UserListViewModel vm = new UserListViewModel()
            {
                Users = userList,
                Id_Users = idUserList,
                Rooms = roomList,
                IsTest = IsTest
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult UserInfo()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> MuhdoRegister(long? id)
        {
            //we load a user if an id is provided here
            MuhdoRegisterModel model;
            if (id.HasValue)
            {
                var currUser = await _clientrepo.GetClientById(id.Value);
                model = _mapper.Map<MuhdoRegisterModel>(currUser);
                model.Gender = currUser.FkGenderNavigation != null ? currUser.FkGenderNavigation.Name.ToLower() : currUser.Gender;
                model.Id = id.Value;
            }
            else
            {
                model = new MuhdoRegisterModel();
            }
            model.CountryList = new SelectList(ListUtil.CountryList(), "Key", "Value").OrderBy(r => r.Text).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MuhdoRegister(MuhdoRegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool success = await _clientrepo.CallMuhdoRegisterApiCall(model);
                if (success)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("apicall_failed", "Muhdo Registration failed. Please try again later.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call muhdo registration api");
                ModelState.AddModelError("apicall_failed", "Muhdo Registration failed. Please try again later.");
                return View(model); 
            }
        }

        [HttpGet]
        public IActionResult Promotion()
        {
            var success = Request.Query["success"].FirstOrDefault();
            ViewData["SubmitSuccess"] = success ?? "false";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Promotion(PromotionModel data)
        {
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            //retrieve file bytes and content type (optional)    
            if (data.UploadImageFile != null &&
                data.UploadImageFile.Length > 0 &&
                data.UploadImageFile.Length < UPLOAD_BYTELIMIT &&
                data.UploadImageFile.ContentType.StartsWith("image/"))
            {
                //byte[] imageBytes = new byte[data.UploadImageFile.Length];
                string contentType = data.UploadImageFile.ContentType;
                
                using (var fileStream = data.UploadImageFile.OpenReadStream())
                {
                    Image img = Image.FromStream(fileStream);
                    if (img.Width == PROMOTION_IMG_WIDTH_LIMIT && img.Height == PROMOTION_IMG_HEIGHT_LIMIT)
                    {
                        data.ImageBytes = GetBytesFromImage(img);
                    }
                    else
                    {
                        //resize the image if it does not meet the noted dimensions
                        Image resizeImg = ImageResize(img, PROMOTION_IMG_WIDTH_LIMIT, PROMOTION_IMG_HEIGHT_LIMIT);
                        data.ImageBytes = GetBytesFromImage(resizeImg);
                    }
                    
                    //fileStream.Read(imageBytes, 0, (int) data.UploadImageFile.Length);
                }

                //data.ImageBytes = imageBytes;
                data.ContentType = contentType;           
            }

            //save broadcast message
            bool result = await _promorepo.SendPromotionChatMessage(data);
            if (result)
            {
                return RedirectToAction("Promotion", new { success = true });
            }

            ModelState.AddModelError("error", "send_promotion_failed");
            return View(data);
        }

        private Image ImageResize(Image srcImage, int newWidth, int newHeight )
        {
            Bitmap bitmap = new Bitmap(newWidth, newHeight);

            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed ||
                bitmap.PixelFormat == PixelFormat.Format4bppIndexed ||
                bitmap.PixelFormat == PixelFormat.Undefined ||
                bitmap.PixelFormat == PixelFormat.DontCare || 
                bitmap.PixelFormat == PixelFormat.Format16bppArgb1555 ||
                bitmap.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                throw new NotSupportedException("Pixel Format not supported");
            }

            using (Graphics gImage = Graphics.FromImage(bitmap))
            {
                gImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gImage.DrawImage(srcImage, 0, 0, bitmap.Width, bitmap.Height);
            }
            return bitmap;
        }

        private static byte[] GetBytesFromImage(Image i)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[]) _imageConverter.ConvertTo(i, typeof(byte[]));
            return xByte;
        }
    }
}