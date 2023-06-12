using ChartApi.Net7;
using DAOLayer.Net7.Chat;
using DAOLayer.Net7.Chat.ApiModels;
using DAOLayer.Net7.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.NotificationHubs;
using Microsoft.EntityFrameworkCore;
using ParentMiddleWare.Models;
using User = DAOLayer.Net7.Chat.User;

namespace FitappApi.Net7.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ChatContext _context;
        private readonly UserContext _uContext;
        public ChatController(ChatContext context, UserContext uContext)
        {
            _context = context;
            _uContext = uContext;
        }


        [HttpGet]
        [Route("GetMessages")]
        public async Task<List<RecievedMessage>> GetMessages(long RoomId, DateTime fromDate)
        {
            return MapChatMessage(await _context.MsgMessage.Where(t => t.FkRoomId == RoomId && t.Timestamp >= fromDate)
                .Include(t=>t.FkUserSenderNavigation)
                .Include(t=>t.FkRoom)
                .AsNoTracking()
                .ToListAsync());
        }

        [HttpGet]
        [Route("GetMessagesFrontend")]
        public async Task<List<RecievedMessage>> GetMessagesFrontend(long UserId, DateTime fromDate)
        {
            var room = await CreateRoom(UserId);
            return MapChatMessage(await _context.MsgMessage.Where(t => t.FkRoomId == room.Id && t.Timestamp >= fromDate)
                 .Include(t => t.FkUserSenderNavigation)
                 .Include(t => t.FkRoom)
                 .AsNoTracking()
                 .ToListAsync());
        }


        [HttpPost]
        [Route("SendMessage")]
        public async Task<RecievedMessage> SendMessage(BackendMessage message)
        {
            try
            {
                MsgMessage msg = new MsgMessage();
                msg.FkRoom = await CreateRoom(message.Fk_Reciever_Id);
                msg.MessageContent = message.MessageContent;
                msg.FkUserSender = message.Fk_Sender_Id;
                msg.Timestamp = DateTime.UtcNow;

                await _context.MsgMessage.AddAsync(msg);
                if (await _context.SaveChangesAsync() > 0)
                {

                    #region  APN
                    //try
                    //{
                    //      INotificationHubClient hub = new NotificationHubClient(
                    //      "Endpoint=sb://airns1.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=NSSFc3/djIcdHKt7JXDitQ2g6b3RyMTwIFLR5OrziQM=",
                    //      "airnotificationhub"
                    //      );

                    //    var DeviceList = await _uContext.Apn.Where(t => t.FkUserId == message.Fk_Reciever_Id && t.IsActive == true).ToListAsync();
                    //    if (DeviceList == null)
                    //    {

                    //    }
                    //    foreach (var device in DeviceList)
                    //    {
                    //        var jsonPayload = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
                    //        var n = new AppleNotification(jsonPayload);
                    //        NotificationOutcome outcome = await hub.SendNotificationAsync(n, "RegistrationId:" + device.DeviceId + "/");
                    //    }

                    //}
                    //catch (Exception ex)
                    //{

                    //}
                    #endregion
                }





                msg.FkUserSenderNavigation = _context.User.Where(t => t.Id == msg.FkUserSender).First();
                return MapChatMessage(msg);
            }
            
           
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("SendMessageFrontend")]
        public async Task<RecievedMessage> SendMessageFrontend(FrontendMessage message)
        {
            try
            {
                MsgMessage msg = new MsgMessage();
              
                msg.FkRoom = await CreateRoom(message.Fk_Sender_Id);
                msg.FkRoom.HasConcern = true;
                msg.MessageContent = message.MessageContent;
                msg.FkUserSender = message.Fk_Sender_Id;
                msg.Timestamp = DateTime.UtcNow;
               

                await _context.MsgMessage.AddAsync(msg);
                if (await _context.SaveChangesAsync() > 0)
                {
                    msg.FkUserSenderNavigation = _context.User.Where(t => t.Id == msg.FkUserSender).First();
                    //await SendMessage(new BackendMessage() { Fk_Reciever_Id = message.Fk_Sender_Id,  Fk_Sender_Id = 1, MessageContent = "Beep Beep, Hello I am the Support Robot!" });
                    return MapChatMessage(msg);
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
        public async Task<MsgRoom> CreateRoom(long userId)
        {
            try
            {
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
        public async Task RemoveUnhandledFlag(long RoomId)
        {
            _context.MsgRoom.Where(t => t.Id == RoomId).First().HasConcern = false;
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("AddUnhandledFlag")]
        public async Task AddUnhandledFlag(long UserId)
        {
            var room = await CreateRoom(UserId);
            room.HasConcern = true;
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        [Route("GetRoomsWithConcerns")]
        public async Task<List<MsgRoom>> GetRoomsWithConcerns()
        {
           return await _context.MsgRoom.Where(t => t.HasConcern == true)
                .AsNoTracking()
                .ToListAsync();
        }



        //

        public static List<RecievedMessage> MapChatMessage(List<MsgMessage> message)
        {
            List<RecievedMessage> msgList = new List<RecievedMessage>();
            foreach (MsgMessage msg in message)
            {
                msgList.Add(MapChatMessage(msg));
            }
            return msgList;
        }

        public static RecievedMessage MapChatMessage(MsgMessage message)
        {
            RecievedMessage msg = new RecievedMessage();
            msg.MessageContent = message.MessageContent;
            msg.TimeStamp = message.Timestamp;
            msg.IsUserMessage = (message.FkRoom.FkUserId == message.FkUserSender);
            msg.UserName = (message.FkUserSenderNavigation.FirstName + " " + message.FkUserSenderNavigation.Email).Trim();
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
