using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Logs;
using DAOLayer.Net7.Nutrition;
using DAOLayer.Net7.Supplement;
using LogHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AutomationController : ControllerBase
    {

        private readonly ExerciseContext _context;
        private readonly NutritionContext _nContext;
        private readonly SupplementContext _sContext;
        private readonly LogsContext _lContext;

        public AutomationController(ExerciseContext context, NutritionContext nContext, SupplementContext sContext, LogsContext lContext)
        {
            _context = context;
            _nContext = nContext;
            _sContext = sContext;
            _lContext = lContext;
        }

        [HttpPost]
        [Route("Supplements_Trigger_Schedule_Update_All")]
        public async Task<bool> Supplements_Trigger_Schedule_Update_All(DateTime start_date, bool force = false)
        {
            try
            {
                Logs llog =  sLogHelper.Log(LogHelper.Component.API,
                      this.GetType().Name,
                      System.Reflection.MethodBase.GetCurrentMethod().Name,
                      (int)LogHelper.Severity.Error,
                     "Called Supplements_Trigger_Schedule_Update_All"
                 );
                _lContext.Add(llog);
                await _lContext.SaveChangesAsync();

                var x = await _sContext.NdsSupplementPlanWeekly.Where(t => t.IsActive == true).ToListAsync();



                foreach (var y in x)
                {
                    try
                    {
                        await this.Supplements_Trigger_Schedule_Update(y.Id, start_date, force);
                    }
                    catch (Exception ex)
                    {
                        Logs llog2 = sLogHelper.Log(Component.API,
                       this.GetType().Name,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        (int)LogHelper.Severity.Error,
                        ex.Message + "ID: " + y.Id,
                        ex.StackTrace);
                        _lContext.Add(llog2);
                        await _lContext.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logs llog = sLogHelper.Log(LogHelper.Component.API,
                this.GetType().Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name,
                (int)LogHelper.Severity.Error,
                ex.Message,
                ex.StackTrace);
                _lContext.Add(llog);
                await _lContext.SaveChangesAsync();

                return false;
            }
        }

        [HttpPost]
        [Route("Supplements_Trigger_Schedule_Update")]
        public async Task<bool> Supplements_Trigger_Schedule_Update(long supplement_plan_weekly_id, DateTime start_date, bool force = false)
        {
            try
            {
             //   DbContextOptionsBuilder.EnableSensitiveDataLogging;

                var x = await _sContext.NdsSupplementPlanWeekly.Where(t => t.Id == supplement_plan_weekly_id)
                    .Include(t=>t.FkCustomer)
                    .Include(t => t.NdsSupplementPlanDaily)
                    .ThenInclude(t => t.NdsSupplementPlanSupplement)
                    .ThenInclude(t => t.NdsSupplementPlanDose)

                    .Include(t => t.NdsSupplementPlanDaily)
                    .ThenInclude(t => t.NdsSupplementPlanSupplement)
                    .ThenInclude(t => t.FkSupplementReferenceNavigation)

                   // .AsNoTracking()
                    .FirstAsync();

                foreach(var day in x.NdsSupplementPlanDaily)
                {
                    int DayOfWeek__start_date = netDayOfweek2DayOfweek(start_date.Date);
                    int DayOfWeek__Plan = day.DayOfWeek;



                    if (DayOfWeek__start_date >= DayOfWeek__Plan && force == false)
                    {
                        continue;
                    }

                    DateTime targetDate = start_date.Date.AddDays(DayOfWeek__Plan - DayOfWeek__start_date);

                    NdsSupplementSchedulePerDate s_date = await _sContext.NdsSupplementSchedulePerDate.Where(t => t.CustomerId == x.FkCustomerId && t.Date == targetDate).FirstOrDefaultAsync();
                    if (s_date == null)
                    {
                        s_date = new NdsSupplementSchedulePerDate()
                        {
                            Customer = x.FkCustomer,
                            Date = targetDate.Date,
                            NdsSupplementSchedule = new List<NdsSupplementSchedule>()
                        };
                        _sContext.NdsSupplementSchedulePerDate.Add(s_date);
                        await _sContext.SaveChangesAsync();
                    }
                    else
                    {
                        if (!force) continue;
                        if(force)
                        {
                          // do we need to delete old, or does linq do that already automatically?
                          // if double schedules show up, check
                        }

                    }
                        foreach(var plan_supplement in day.NdsSupplementPlanSupplement)
                        {
                            var s_schedule = new NdsSupplementSchedule()
                            {
                                FkFreeEntryUnitMetric = null,
                                FkSupplementReferenceNavigation = plan_supplement.FkSupplementReferenceNavigation,
                                FkSupplementSchedulePerDateNavigation = s_date,
                                FreeEntryInstructions = "",
                                IsCustomerCreatedEntry = false,
                                IsFreeEntry = false,
                                NdsSupplementScheduleDose = new List<NdsSupplementScheduleDose>()
                            };

                            foreach (var plan_dose in plan_supplement.NdsSupplementPlanDose)
                            {
                                var s_dose = new NdsSupplementScheduleDose()
                                {
                                    CompletionTime = null,
                                    CompletionTimeOffset = null,
                                    FkSupplementScheduleNavigation = s_schedule,
                                    IsComplete = false,
                                    IsSkipped = false,
                                    IsSnoozed = false,
                                    Remarks = null,
                                    ScheduledTime = targetDate.Add(plan_dose.ScheduledTime),
                                    SnoozedTime = null,
                                    SupplementSkipOtherReason = null,
                                    SupplementSkipReason = null,
                                    SupplementSkipReasonNavigation = null,
                                    UnitCountActual = null,
                                    UnitCountTarget = plan_dose.UnitCount
                                };
                                s_schedule.NdsSupplementScheduleDose.Add(s_dose);
                            }
                            s_date.NdsSupplementSchedule.Add(s_schedule);
                        }

                    //    _sContext.NdsSupplementSchedulePerDate.Add(s_date);
                        await _sContext.SaveChangesAsync();

                    //}
                    //else
                    //{
                    //    Logs llog = sLogHelper.Log(LogHelper.Component.API,
                    //    this.GetType().Name,
                    //    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    //    (int)LogHelper.Severity.Warning,
                    //    "NdsSupplementSchedulePerDate is not null",
                    //    "TargetDate" + targetDate.ToString());

                    //    _lContext.Add(llog);
                    //    await _lContext.SaveChangesAsync();
                    //}
                }
            }
            catch (Exception ex)
            {
                Logs llog = sLogHelper.Log(LogHelper.Component.API,
                    this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    (int)LogHelper.Severity.Error,
                    ex.Message,
                    ex.StackTrace);
                _lContext.Add(llog);
                await _lContext.SaveChangesAsync();
                return false;
            }

            return true;

        }

        public static short netDayOfweek2DayOfweek(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: return 0;
                case DayOfWeek.Tuesday: return 1;
                case DayOfWeek.Wednesday: return 2;
                case DayOfWeek.Thursday: return 3;
                case DayOfWeek.Friday: return 4;
                case DayOfWeek.Saturday: return 5;
                case DayOfWeek.Sunday: return 6;
                default: return 0;
            }
        }

        //[HttpGet]
        //[Route("GetMessagesFrontend")]
        //public async Task<List<RecievedMessage>> GetMessagesFrontend(long UserId, DateTime fromDate)
        //{
        //    var room = await CreateRoom(UserId);
        //    return MapChatMessage(await _context.MsgMessage.Where(t => t.FkRoomId == room.Id && t.Timestamp >= fromDate)
        //         .Include(t => t.FkUserSenderNavigation)
        //         .Include(t => t.FkRoom)
        //         .AsNoTracking()
        //         .ToListAsync());
        //}


        //[HttpPost]
        //[Route("SendMessage")]
        //public async Task<RecievedMessage> SendMessage(BackendMessage message)
        //{
        //    try
        //    {
        //        MsgMessage msg = new MsgMessage();
        //        msg.FkRoom = await CreateRoom(message.Fk_Reciever_Id);
        //        msg.MessageContent = message.MessageContent;
        //        msg.FkUserSender = message.Fk_Sender_Id;
        //        msg.Timestamp = DateTime.UtcNow;

        //        await _context.MsgMessage.AddAsync(msg);
        //        if (await _context.SaveChangesAsync() > 0)
        //        {
        //            msg.FkUserSenderNavigation = _context.User.Where(t => t.Id == msg.FkUserSender).First();
        //            return MapChatMessage(msg);
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //[HttpPost]
        //[Route("SendMessageFrontend")]
        //public async Task<RecievedMessage> SendMessageFrontend(FrontendMessage message)
        //{
        //    try
        //    {
        //        MsgMessage msg = new MsgMessage();

        //        msg.FkRoom = await CreateRoom(message.Fk_Sender_Id);
        //        msg.FkRoom.HasConcern = true;
        //        msg.MessageContent = message.MessageContent;
        //        msg.FkUserSender = message.Fk_Sender_Id;
        //        msg.Timestamp = DateTime.UtcNow;


        //        await _context.MsgMessage.AddAsync(msg);
        //        if (await _context.SaveChangesAsync() > 0)
        //        {
        //            msg.FkUserSenderNavigation = _context.User.Where(t => t.Id == msg.FkUserSender).First();
        //            await SendMessage(new BackendMessage() { Fk_Reciever_Id = message.Fk_Sender_Id, Fk_Sender_Id = 1, MessageContent = "Beep Beep, Hello I am the Support Robot!" });
        //            return MapChatMessage(msg);
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //[HttpPost]
        //[Route("CreateRoom")]
        //public async Task<MsgRoom> CreateRoom(long userId)
        //{
        //    try
        //    {
        //        var existingRoom = await _context.MsgRoom.Where(t => t.FkUserId == userId).FirstOrDefaultAsync();
        //        if (existingRoom == null)
        //        {
        //            MsgRoom room = new MsgRoom();
        //            room.FkUserId = userId;
        //            User user = await _context.User.Where(t => t.Id == userId).FirstAsync();
        //            room.RoomName = (user.FirstName + " : " + user.Email).Trim();
        //            await _context.MsgRoom.AddAsync(room);
        //            await _context.SaveChangesAsync();
        //            return room;
        //        }
        //        return existingRoom;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //[HttpPost]
        //[Route("RemoveUnhandledFlag")]
        //public async Task RemoveUnhandledFlag(long RoomId)
        //{
        //    _context.MsgRoom.Where(t => t.Id == RoomId).First().HasConcern = false;
        //    await _context.SaveChangesAsync();
        //}

        //[HttpPost]
        //[Route("AddUnhandledFlag")]
        //public async Task AddUnhandledFlag(long UserId)
        //{
        //    var room = await CreateRoom(UserId);
        //    room.HasConcern = true;
        //    await _context.SaveChangesAsync();
        //}

        //[HttpGet]
        //[Route("GetRoomsWithConcerns")]
        //public async Task<List<MsgRoom>> GetRoomsWithConcerns()
        //{
        //    return await _context.MsgRoom.Where(t => t.HasConcern == true)
        //         .AsNoTracking()
        //         .ToListAsync();
        //}



        ////

        //public static List<RecievedMessage> MapChatMessage(List<MsgMessage> message)
        //{
        //    List<RecievedMessage> msgList = new List<RecievedMessage>();
        //    foreach (MsgMessage msg in message)
        //    {
        //        msgList.Add(MapChatMessage(msg));
        //    }
        //    return msgList;
        //}

        //public static RecievedMessage MapChatMessage(MsgMessage message)
        //{
        //    RecievedMessage msg = new RecievedMessage();
        //    msg.MessageContent = message.MessageContent;
        //    msg.TimeStamp = message.Timestamp;
        //    msg.IsUserMessage = (message.FkRoom.FkUserId == message.FkUserSender);
        //    msg.UserName = (message.FkUserSenderNavigation.FirstName + " " + message.FkUserSenderNavigation.Email).Trim();
        //    return msg;
        //}

        //public static MsgMessage MapMsgMessage(ChatMessage ChatMessage)
        //{
        //    MsgMessage msg = new MsgMessage();
        //    msg.MessageContent = ChatMessage.MessageContent;
        //    msg.FkRoomId = ChatMessage.FkRoomId;
        //    msg.FkUserSender = ChatMessage.FkUserSender;
        //    msg.Timestamp = ChatMessage.Timestamp;

        //    return msg;
        //}


        //public static ChatRoom MapChatRoom(MsgRoom room)
        //{
        //    return null;
        //}


    }
}
