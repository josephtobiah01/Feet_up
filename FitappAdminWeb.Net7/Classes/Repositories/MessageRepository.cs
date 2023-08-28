using DAOLayer.Net7.Chat;
using FitappAdminWeb.Net7.Classes.Constants;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Classes.Utilities;
using MessageApi.Net7.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Configuration;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    /// <summary>
    /// Handles all message-related operations
    /// </summary>
    public class MessageRepository
    {
        FitAppAPIUtil _apiutil;
        ILogger<MessageRepository> _logger;
        ChatContext _chatContext;
        IConfiguration _config;

        public static readonly string DOMAIN_APPSETTING_KEY = "MessageApi_Domain";
        public static readonly int DEFAULT_MESSAGEHISTORY_DAYS = 14; //2 weeks

        string _getMessages_Url = "/api/chat/getmessages?roomid={0}&fromDate={1}";
        string _getRoomsWithConcerns_Url = "/api/chat/getroomswithconcerns";
        string _sendMessage_Url = "/api/chat/sendmessage";
        string _removeUnhandledFlag_Url = "/api/chat/removeunhandledflag";

        public MessageRepository(ILogger<MessageRepository> logger, IConfiguration config, ChatContext chatContext, FitAppAPIUtil apiutil)
        {
            _logger = logger;
            _config = config;
            _chatContext = chatContext;
            _apiutil = apiutil;      
        }

        public async Task<List<ReceivedMessage>?> API_GetMessages(long roomId, DateTime? fromDate = null)
        {
            try
            {
                if (!fromDate.HasValue)
                {
                    fromDate = DateTime.Now.AddDays(DEFAULT_MESSAGEHISTORY_DAYS * -1);
                }

                var client = _apiutil.GetHttpClient();
                string url = string.Format(_getMessages_Url, roomId, fromDate.Value.ToString("s"));
                var request = _apiutil.BuildRequest(url, HttpMethod.Get);

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var message_result = JsonConvert.DeserializeObject<List<ReceivedMessage>>(await response.Content.ReadAsStringAsync());
                return message_result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve messages from FitApp API (RoomId {roomId}, fromDate {fromDate})", roomId, fromDate);
                return null;
            }
        }

        public async Task<List<MsgMessage>?> GetMessages(long roomId, DateTime fromDate, int numMessages = 10)
        {
            try
            {
                var query = _chatContext.MsgMessage.Where(r => r.FkRoomId == roomId && r.Timestamp >= fromDate)
                                .OrderByDescending(r => r.Timestamp)
                                .Take(numMessages)
                                .Include(r => r.FkUserSenderNavigation);
                var messages = await query.ToListAsync();

                return messages ?? new List<MsgMessage>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get messages from room Id {0} with fromDate {1}", roomId, fromDate);
                return null;
            }
        }

        public async Task<MsgRoom?> GetRoomById(long roomId)
        {
            try
            {
                return await _chatContext.MsgRoom.FirstOrDefaultAsync(r => r.Id == roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find room id {0}", roomId);
                return null;
            }
        }

        /// <summary>
        /// Retrieve a room via room id while also retrieving the latest messages
        /// </summary>
        public async Task<MsgRoom?> GetRoomByRoomId(long roomId, DateTime? fromDate = null)
        {
            try
            {
                if (!fromDate.HasValue)
                {
                    fromDate = DateTime.UtcNow.AddDays(-1 * DEFAULT_MESSAGEHISTORY_DAYS);
                }

                var query = _chatContext.MsgRoom.Where(r => r.Id == roomId)
                    .Include(r => r.MsgMessage.Where(r => r.Timestamp >= fromDate).OrderByDescending(r => r.Timestamp))
                        .ThenInclude(r => r.FkUserSenderNavigation);

                MsgRoom room = await query.FirstOrDefaultAsync();
                return room;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find room id {0}", roomId);
                return null;
            }
        }

        public async Task<MsgRoom?> EnsureChatRoomByUserId(long userId)
        {
            var user = await _chatContext.User.FirstOrDefaultAsync(r => r.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("User {id} does not exist. Cannot get room.", userId);
                return null;
            }

            var room = await _chatContext.MsgRoom.Where(r => r.FkUserId == userId).FirstOrDefaultAsync();

            if (room == null)
            {
                //add a new room for this user
                try
                {                   
                    room = new MsgRoom()
                    {
                        FkUserId = userId,
                        RoomName = $"{user.FirstName} : {user.Email}"
                    };

                    _chatContext.MsgRoom.Add(room);
                    await _chatContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create room for user {id}.", userId);
                    return null;
                }
            }

            return room;
        }

        public async Task<bool> SendMessageToRoom(long roomId, long senderId, string messageContent, bool markAsHandled)
        {
            try
            {
                var currentRoom = await _chatContext.MsgRoom.FirstOrDefaultAsync(r => r.Id == roomId);
                if (currentRoom == null)
                {
                    _logger.LogError("No room {roomId} exists.", roomId);
                    return false;
                }

                currentRoom.HasConcern = !markAsHandled;

                //add new message
                MsgMessage msg = new MsgMessage();
                msg.FkRoomId = roomId;
                msg.FkUserSender = senderId;
                msg.MessageContent = messageContent;
                msg.Timestamp = DateTime.UtcNow;

                _chatContext.MsgMessage.Add(msg);
                await _chatContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to room {roomId}.", roomId);
                return false;
            }
        }

        public async Task<bool> API_SendMessageToRoom(long roomId, string senderId, string? messageContent, bool markAsHandled, 
            string? imageFileContentType = null, string? imageFileContent = null)
        {
            using (_logger.BeginScope("SendMessageToRoomV2"))
            {
                try 
                {
                    //var currentRoom = await _chatContext.MsgRoom.FirstOrDefaultAsync(r => r.Id == roomId);
                    var currentRoom = await _chatContext.MsgRoom.Where(r => r.Id == roomId).Select(s => new
                    {
                        Room = s,
                        RoomOwner = _chatContext.User.FirstOrDefault(t => t.Id == s.FkUserId)
                    }).FirstOrDefaultAsync();

                    if (currentRoom == null || currentRoom.Room == null || currentRoom.RoomOwner == null)
                    {
                        _logger.LogError("No room {roomId} exists.", roomId);
                        return false;
                    }

                    string base64imagebytes = null;
                    //process image file strings if available
                    if (!string.IsNullOrEmpty(imageFileContent) && !string.IsNullOrEmpty(imageFileContentType))
                    {
                        if (!imageFileContentType.ToLower().StartsWith("image"))
                        {
                            _logger.LogError("Cannot send message. Invalid File Type {ctype}", imageFileContentType);
                        }

                        string pattern = @"^data:(\w+)\/(\w+);base64,";
                        base64imagebytes = Regex.Replace(imageFileContent, pattern, string.Empty);
                    }

                    //uses API util to build request message with prereq security headers
                    HttpClient client = _apiutil.GetHttpClient();
                    BackendMessage msg = new BackendMessage()
                    {
                        MessageContent = messageContent,
                        SenderFkFederatedUser = senderId,
                        ReceiverFkFederatedUser = currentRoom.RoomOwner.FkFederatedUser,
                        ImageFileContent = base64imagebytes,
                        ImageFileContentType = imageFileContentType
                    };
                    HttpRequestMessage request = _apiutil.BuildRequest(_sendMessage_Url, HttpMethod.Post, msg);

                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string response_string = await response.Content.ReadAsStringAsync();

                    //update hasConcern based on markAsHandled
                    if (markAsHandled)
                    {
                        currentRoom.Room.HasConcern = false;
                        await _chatContext.SaveChangesAsync();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send message to room {roomId}", roomId);
                    return false;
                }
            }
        }

        public async Task<List<MsgRoom>> GetRooms_Minimal(List<long> userIds)
        {
            return await _chatContext.MsgRoom.Where(r => userIds.Contains(r.FkUserId)).ToListAsync();
        }

        public async Task<List<MsgRoom>> GetRooms(bool hasConcernOnly = false, int numRecentMessages = 0)
        {
            List<MsgRoom> result;
            if (hasConcernOnly)
            {
                result = await _chatContext.MsgRoom.Where(r => r.HasConcern && _chatContext.User.Any(s => s.Id == r.FkUserId))
                   .Include(r => r.MsgMessage.OrderByDescending(r => r.Timestamp).Take(numRecentMessages))
                       .ThenInclude(r => r.FkUserSenderNavigation).ToListAsync();
            }
            else
            {
                result = await _chatContext.MsgRoom.Where(r => _chatContext.User.Any(s => s.Id == r.FkUserId))
                  .Include(r => r.MsgMessage.OrderByDescending(r => r.Timestamp).Take(numRecentMessages))
                      .ThenInclude(r => r.FkUserSenderNavigation).ToListAsync();
            }
           
            return result.OrderByDescending(r =>
            {
                return r.MsgMessage.OrderByDescending(s => s.Timestamp).Select(t => t.Timestamp).FirstOrDefault();
            }).ToList();
        }

        public async Task<List<MsgRoom>?> GetRoomsWithConcerns()
        {
            try
            {
                /*HttpClient client = _httpclientfactory.CreateClient();
                string url = $"{_apiDomain}{_getRoomsWithConcerns_Url}";

                string result = await client.GetStringAsync(url);
                var rooms = JsonConvert.DeserializeObject<List<MsgRoom>>(result);*/

                var query = _chatContext.MsgRoom.Where(r => r.HasConcern)
                    .Include(r => r.MsgMessage.OrderByDescending(r => r.Timestamp).Take(1))
                        .ThenInclude(r => r.FkUserSenderNavigation);
                var rooms = await query.ToListAsync();

                return rooms ?? new List<MsgRoom>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get rooms");
                return null;
            }
        }

        public async Task<int> GetRoomsWithConcernsCount()
        {
            try
            {
                var query = await _chatContext.MsgRoom.CountAsync(r => r.HasConcern && _chatContext.User.Any(s => s.Id == r.FkUserId));
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve Room With Concerns Count");
                return 0;
            }
        }

        public async Task<ReceivedMessage?> SendMessage(BackendMessage message)
        {
            try
            {
                //HttpClient client = _httpclientfactory.CreateClient();
                //string url = _apiDomain + _sendMessage_Url;

                //var response = await client.PostAsJsonAsync(url, message);
                HttpClient client = _apiutil.GetHttpClient();
                HttpRequestMessage request = _apiutil.BuildRequest(_sendMessage_Url, HttpMethod.Post, message);
                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                ReceivedMessage postedMessage = JsonConvert.DeserializeObject<ReceivedMessage>(responseString);

                return postedMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Message");
                return null;
            }
        }

        public async Task<bool> RemoveUnhandledFlag(long roomId)
        {
            try
            {
                //HttpClient client = _httpclientfactory.CreateClient();
                //string url = _apiDomain + _removeUnhandledFlag_Url;

                //var response = await client.PostAsJsonAsync(url, roomId);
                HttpClient client = _apiutil.GetHttpClient();
                var request = _apiutil.BuildRequest(_removeUnhandledFlag_Url, HttpMethod.Post, roomId);

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove unhandled flag in room {0}", roomId);
                return false;
            }
        }

        public async Task<User?> GetUserById(long userId)
        {
            try
            {
                var user = await _chatContext.User.FirstOrDefaultAsync(r => r.Id == userId);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user of id {0}", userId);
                return null;
            }
        }

        /// <summary>
        /// Retrieves/Creates a User object using the entered User ClaimsPrincipal (typically Controller.User)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByLoggedInUserName(ClaimsPrincipal user)
        {
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Cannot access LoggedIn User Identity. Defaulting to hardcoded User");
                //return hardcoded User "AIR Support" here.
                return null;
            }

            try
            {
                //retrieve Information from current login for building new User             
                string username = user.FindFirstValue(AADClaimTypes.AAD_PREFERRED_USERNAME);
                string objectidentifier = user.GetObjectId();
                string email = user.FindFirstValue(AADClaimTypes.AAD_EMAIL) ?? username;

                //NOTE: Implementation may change depending on how we handle nameidentifier, but for now, use email only.
                User? currUser = !string.IsNullOrEmpty(objectidentifier) ? await _chatContext.User.FirstOrDefaultAsync(r => r.FkFederatedUser.ToLower() == objectidentifier.ToLower()) : null;
                if (currUser != null)
                {
                    return currUser;
                }

                //Add a new User based on information from the claims principal
                string name = user.FindFirstValue(AADClaimTypes.AAD_NAME);
                string fname = String.Empty;
                string lname = String.Empty;
                if (!String.IsNullOrEmpty(name))
                {
                    var nameSplit = name.Trim().Split(' ');
                    if (nameSplit.Length > 0)
                    {
                        fname = String.Concat(nameSplit.SkipLast(1));
                        if (nameSplit.Length > 1)
                        {
                            lname = nameSplit.Last();
                        }
                    }
                }

                currUser = new User()
                {
                    UserLevel = UserConstants.ADMIN_USER_USERLEVEL_BASE + UserConstants.AUTOCREATED_USERLEVEL,
                    FirstName = fname,
                    LastName = lname,
                    Email = email,
                    FkFederatedUser = objectidentifier //FederatedUser is NOT nullable
                };

                //Save this user
                _chatContext.User.Add(currUser);
                await _chatContext.SaveChangesAsync();

                return currUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user by claims principal.");
                return null;
            }

        }
    }
}
