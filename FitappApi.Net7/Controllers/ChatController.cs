using ChartApi.Net7;
using DAOLayer.Net7.Chat;
using DAOLayer.Net7.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParentMiddleWare.Models;
using User = DAOLayer.Net7.Chat.User;
using FitappApi.Net7.Util;
using System.Drawing;
using Microsoft.Extensions.Configuration.UserSecrets;
using MessageApi.Net7.Models;

namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ChatController : BaseController
    {
        public readonly string AzureFunctionURL;
        private readonly ChatContext _context;
        private readonly UserContext _uContext;
        private readonly BlobManager _blobManager;

        public ChatController(IConfiguration configuration, ChatContext context, UserContext uContext)
        {
            AzureFunctionURL = configuration["AzureFunctionUrl"];
            _context = context;
            _uContext = uContext;
            _blobManager = new BlobManager(configuration, "chatfiles");
        }

        [HttpGet]
        [Route("GetMessages")]
        public async Task<List<ReceivedMessage>> GetMessages(long RoomId, DateTime fromDate)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            return MapChatMessage(await _context.MsgMessage.Where(t => t.FkRoomId == RoomId && t.Timestamp >= fromDate)
                .Include(t=>t.FkUserSenderNavigation)
                .Include(t=>t.FkRoom)
                .Include(t => t.FkImage)
                .AsNoTracking()
                .ToListAsync());
        }

        [HttpGet]
        [Route("GetMessagesFrontend")]
        public async Task<List<ReceivedMessage>> GetMessagesFrontend(string FkFederatedUser, DateTime fromDate)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var user = _uContext.User.Where(t => t.FkFederatedUser == FkFederatedUser).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            long UserId = user.Id;
            var room = await CreateRoom(FkFederatedUser);
            return MapChatMessage(await _context.MsgMessage.Where(t => t.FkRoomId == room.Id && t.Timestamp >= fromDate)
                 .Include(t => t.FkUserSenderNavigation)
                 .Include(t => t.FkRoom)
                 .Include(t => t.FkImage)
                 .AsNoTracking()
                 .ToListAsync());
        }

        private byte[] CreateThumbnailImage(byte[] origImageData)
        {
            float scale = 1;
            using (MemoryStream memoryStream = new MemoryStream(origImageData))
            {
                memoryStream.Position = 0;
                var origImage = System.Drawing.Image.FromStream(memoryStream);
                int origWidth = origImage.Width;
                int origHeight = origImage.Height;
                if (origWidth > origHeight)
                {
                    scale = (300f / origWidth);
                }
                else
                {
                    scale = 300f / origHeight;
                }
                float resizedWidth = origWidth * scale;
                float resizedHeight = origHeight * scale;
                Bitmap thumbnailImage = new Bitmap(origImage, new Size((int)(resizedWidth), (int)(resizedHeight)));
                ImageConverter converter = new ImageConverter();
                return (byte[])converter.ConvertTo(thumbnailImage, typeof(byte[]));
            }
        }

        [HttpPost]
        [Route("SendMessage")]
        public async Task<ReceivedMessage> SendMessage([FromBody]BackendMessage message)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                MsgMessage msg = new MsgMessage();
                msg.FkRoom = await CreateRoom(message.ReceiverFkFederatedUser);
                msg.MessageContent = message.MessageContent;
                var Sender = _uContext.User.Where(u => u.FkFederatedUser == message.SenderFkFederatedUser).FirstOrDefault();
                if (Sender == null)
                {
                    return null;
                }
                msg.FkUserSender = Sender.Id;
                msg.Timestamp = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(message.ImageFileContent) && !string.IsNullOrEmpty(message.ImageFileContentType))
                {
                    byte[] data = Convert.FromBase64String(message.ImageFileContent);
                    byte[] thumbnailData = CreateThumbnailImage(data);

                    var uploadOrigImageTask = _blobManager.UploadBlob(message.ImageFileContentType, new BinaryData(data));
                    var uploadThumbnailImageTask = _blobManager.UploadBlob(message.ImageFileContentType, new BinaryData(thumbnailData));
                    await Task.WhenAll(uploadOrigImageTask, uploadThumbnailImageTask);
                    string imageUrl = uploadOrigImageTask.Result;
                    string thumbnailImageUrl = uploadThumbnailImageTask.Result;

                    msg.FkImage = new DAOLayer.Net7.Chat.Image();
                    msg.FkImage.RealImageUrl = imageUrl;
                    msg.FkImage.ThumbnailImageUrl = thumbnailImageUrl;
                    await _context.Image.AddAsync(msg.FkImage);
                }

                await _context.MsgMessage.AddAsync(msg);
                if (await _context.SaveChangesAsync() > 0)
                {
                    #region  APN
                    var Receiver = _uContext.User.Where(u => u.FkFederatedUser == message.ReceiverFkFederatedUser).FirstOrDefault();
                    if (Receiver == null)
                    {
                        return null;
                    }
                    HttpClient _httpClient = new HttpClient();
                    _httpClient.PostAsync(string.Format("{0}/api/SendChat?userId={1}", AzureFunctionURL, Receiver.Id), null);
                    #endregion
                }
                msg.FkUserSenderNavigation = _context.User.Where(t => t.Id == msg.FkUserSender).First();
                string sasToken = _blobManager.GetSasToken();
                return MapChatMessage(msg, sasToken);
            }          
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("SendMessageFrontend")]
        public async Task<ReceivedMessage> SendMessageFrontend(FrontendMessage message)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var user = _uContext.User.Where(t => t.FkFederatedUser == message.FkFederatedUser).FirstOrDefault();
                if (user == null)
                {
                    return null;
                }
                long UserId = user.Id;
                MsgMessage msg = new MsgMessage();
              
                msg.FkRoom = await CreateRoom(message.FkFederatedUser);
                msg.FkRoom.HasConcern = true;
                msg.MessageContent = message.MessageContent;
                msg.FkUserSender = UserId;
                msg.Timestamp = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(message.ImageFileContent) && !string.IsNullOrEmpty(message.ImageFileContentType))
                {
                    byte[] data = Convert.FromBase64String(message.ImageFileContent);
                    byte[] thumbnailData = CreateThumbnailImage(data);

                    var uploadOrigImageTask = _blobManager.UploadBlob(message.ImageFileContentType, new BinaryData(data));
                    var uploadThumbnailImageTask = _blobManager.UploadBlob(message.ImageFileContentType, new BinaryData(thumbnailData));
                    await Task.WhenAll(uploadOrigImageTask, uploadThumbnailImageTask);
                    string imageUrl = uploadOrigImageTask.Result;
                    string thumbnailImageUrl = uploadThumbnailImageTask.Result;

                    msg.FkImage = new DAOLayer.Net7.Chat.Image();
                    msg.FkImage.RealImageUrl = imageUrl;
                    msg.FkImage.ThumbnailImageUrl = thumbnailImageUrl;
                    await _context.Image.AddAsync(msg.FkImage);
                }

                await _context.MsgMessage.AddAsync(msg);
                if (await _context.SaveChangesAsync() > 0)
                {
                    msg.FkUserSenderNavigation = _context.User.Where(t => t.Id == msg.FkUserSender).First();
                    string sasToken = _blobManager.GetSasToken();
                    return MapChatMessage(msg, sasToken);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("CreateRoom")]
        public async Task<MsgRoom> CreateRoom(string FkFederatedUser)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var User = _uContext.User.Where(u=>u.FkFederatedUser==FkFederatedUser).FirstOrDefault();
                if (User == null)
                {
                    return null;
                }
                var userId = User.Id;
                var existingRoom = await _context.MsgRoom.Where(t => t.FkUserId == userId).FirstOrDefaultAsync();
                if (existingRoom == null)
                {
                    MsgRoom room = new MsgRoom();
                    room.FkUserId = userId;
                    User user = await _context.User.Where(t => t.Id == userId).FirstAsync();
                    room.RoomName = (user.FirstName + " : " + user.Email).Trim();
                    await _context.MsgRoom.AddAsync(room);
                    await _context.SaveChangesAsync();
                    return room;
                }
                return existingRoom;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("RemoveUnhandledFlag")]
        public async Task RemoveUnhandledFlag([FromBody]long RoomId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            _context.MsgRoom.Where(t => t.Id == RoomId).First().HasConcern = false;
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("AddUnhandledFlag")]
        public async Task AddUnhandledFlag([FromBody]string FkFederatedUser)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var user = _uContext.User.Where(t => t.FkFederatedUser == FkFederatedUser).FirstOrDefault();
            if (user == null)
            {
                return;
            }
            long UserId = user.Id;
            var room = await CreateRoom(FkFederatedUser);
            room.HasConcern = true;
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        [Route("GetRoomsWithConcerns")]
        public async Task<List<MsgRoom>> GetRoomsWithConcerns()
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            return await _context.MsgRoom.Where(t => t.HasConcern == true)
                .AsNoTracking()
                .ToListAsync();
        }


        private List<ReceivedMessage> MapChatMessage(List<MsgMessage> message)
        {
            string sasToken = _blobManager.GetSasToken();
            List<ReceivedMessage> msgList = new List<ReceivedMessage>();
            foreach (MsgMessage msg in message)
            {
                msgList.Add(MapChatMessage(msg, sasToken));
            }
            return msgList;
        }

        private ReceivedMessage MapChatMessage(MsgMessage message, string sasToken)
        {
            ReceivedMessage msg = new ReceivedMessage();
            msg.MessageContent = message.MessageContent;
            msg.TimeStamp = message.Timestamp;
            msg.IsUserMessage = (message.FkRoom.FkUserId == message.FkUserSender);
            msg.UserName = (message.FkUserSenderNavigation.FirstName + " " + message.FkUserSenderNavigation.Email).Trim();

            if(message.FkImage != null)
            {
                msg.ThumbnailImageUrl = message.FkImage.ThumbnailImageUrl + "?" + sasToken;
                msg.RealImageUrl = message.FkImage.RealImageUrl + "?" + sasToken;
            }

            return msg;
        }

        public static MsgMessage MapMsgMessage(ChatMessage ChatMessage)
        {
            MsgMessage msg = new MsgMessage();
            msg.MessageContent = ChatMessage.MessageContent;
            msg.FkRoomId = ChatMessage.FkRoomId;
            msg.FkUserSender = ChatMessage.FkUserSender;
            msg.Timestamp = ChatMessage.Timestamp;

            return msg;
        }


        public static ChatRoom MapChatRoom(MsgRoom room)
        {
            return null;
        }


        [HttpPost]
        [Route("SendPromotionChat")]
        public async Task<bool> SendPromotionChat([FromBody] PromotionChatmessage message)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                MsgBroadcast bmsg = new MsgBroadcast()
                {
                    FkUserId = null,
                    Icon = message.Icon,
                    Imageurl = message.ImageUrl,
                    IsDeleted = false,
                    Message = message.Message,
                    Timestamp = DateTime.UtcNow,
                    Title = message.Title,
                    Url = message.LinkUrl
                };
                _context.MsgBroadcast.Add(bmsg);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        [Route("GetPromotionChat")]
        public async Task<List<PromotionChatmessage>> GetPromotionChat()
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var rlist = new List<PromotionChatmessage>();
                var bmsgList = await _context.MsgBroadcast.Where(t => t.Timestamp >= DateTime.UtcNow.AddDays(-30))
                     .OrderByDescending(t => t.Timestamp)
                     .Take(20)
                     .AsNoTracking()
                     .ToListAsync();

                if (bmsgList.Count == 0)
                {
                    return new List<PromotionChatmessage>();
                }

                foreach (var m in bmsgList)
                {
                    try
                    {
                        PromotionChatmessage n = new PromotionChatmessage()
                        {
                            DateSentUTC = m.Timestamp,
                            Icon = m.Icon,
                            ImageUrl = m.Imageurl,
                            LinkUrl = m.Url,
                            Message = m.Message,
                            Title = m.Title
                        };
                        rlist.Add(n);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (rlist.Count == 0)
                {
                    PromotionChatmessage n = new PromotionChatmessage()
                    {
                        //DateSentUTC = DateTime.UtcNow.AddDays(-1),
                        //Icon = "Warning",
                        //ImageUrl = "https://www.google.com/imgres?imgurl=https%3A%2F%2Fimg.fruugo.com%2Fproduct%2F6%2F63%2F762022636_max.jpg&tbnid=d9BVUY2x9Y1bvM&vet=12ahUKEwi00_y3roj_AhWzyKACHS6hC5wQMygPegUIARCKAg..i&imgrefurl=https%3A%2F%2Fwww.fruugo.co.nz%2Fanime-game-garden-of-banban-monster-figure-toys-collectible-model-dolls-table-ornaments-decor-gift-for-kids-fans%2Fp-162802620-346140940&docid=SXoF3ljlfy5qbM&w=1000&h=1000&itg=1&q=banban&ved=2ahUKEwi00_y3roj_AhWzyKACHS6hC5wQMygPegUIARCKAg",
                        //LinkUrl = m.Url,
                        //Message = m.Message,
                        //Title = m.Title
                    };
                    rlist.Add(n);
                }

                return rlist;


            }
            catch (Exception ex)
            {
                return new List<PromotionChatmessage>();
            }
        }

    }
}
