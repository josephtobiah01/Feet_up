using AutoMapper;
using DAOLayer.Net7.User;
using FitappAdminWeb.Net7.Classes.Constants;
using FitappAdminWeb.Net7.Classes.Utilities;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MuhdoApi.Net7;
using MuhdoApi.Net7.Model;
using Newtonsoft.Json;
using System.Security.Claims;
using UserApi.Net7.Models;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    /// <summary>
    /// Repository for handling Client data and information
    /// </summary>
    public class ClientRepository
    {
        readonly string DEFAULT_CUSTOMER_PASSWORD = "Ch@ngeMe123!";
        readonly string URL_CREATEUSER_API = "/api/User/CreateUser";

        UserContext _dbcontext;
        IdentityDbContext _iddb;
        FitAppAPIUtil _apiutil;
        IMapper _mapper;
        ILogger<ClientRepository> _logger;

        public ClientRepository(
            UserContext dbcontext, 
            IdentityDbContext iddb, 
            ILogger<ClientRepository> logger, 
            IMapper mapper,
            FitAppAPIUtil apiutil)
        {
            _dbcontext = dbcontext;
            _iddb = iddb;
            _logger = logger;
            _mapper = mapper;
            _apiutil = apiutil;
        }

        public async Task<string> GetQtoolLink(long userId)
        {
            return null;
        }

        public async Task<List<User>> GetAllClients(bool includeInactive)
        {
            //NOTE: This must filter only Users that are clients, not staff based on UserLevel
            return await _dbcontext.User.Where(r => 
                r.UserLevel > 0 && r.UserLevel <= 1000 && 
                (includeInactive || r.IsActive == true))
            .ToListAsync();
        }

        public async Task<List<IdentityUser>> GetIdentityUsersWithFederatedList(List<string> ids)
        {
            return await _iddb.Users.Where(r => ids.Contains(r.Id)).ToListAsync();
        }

        public async Task<IdentityUser?> GetIdentityUserById(string id)
        {
            return await _iddb.Users.Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetClientById(long id)
        {
            User? user = await _dbcontext.User.Where(r => r.Id == id)
                .Include(r => r.FkGenderNavigation)
                .Include(r => r.FkShippingAddressNavigation).FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<Gender>> GetGenders()
        {
            return await _dbcontext.Gender.Where(r => r.IsActive == true).ToListAsync();
        }

        public async Task<User?> AddCustomer(RegisterCustomerModel data)
        {
            using (_logger.BeginScope("AddCustomer"))
            {
                try
                {
                    //create a user via API first
                    CreateUserModel usermodel = new CreateUserModel()
                    {
                        Username = data.Email,
                        Email = data.Email,
                        Password = data.Password //DEFAULT_CUSTOMER_PASSWORD
                    };
                    var httpclient = _apiutil.GetHttpClient();
                    var request = _apiutil.BuildRequest(URL_CREATEUSER_API, HttpMethod.Post, usermodel);
                    var response = await httpclient.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var useropresult = JsonConvert.DeserializeObject<UserOpResult>(await response.Content.ReadAsStringAsync());
                    if (useropresult == null || useropresult.isSuccess == false)
                        throw new InvalidOperationException($"API CreateUser failed. Parameters: {JsonConvert.SerializeObject(usermodel)}");

                    //Retrieve newly created user and edit it using current information
                    User currentUser = await _dbcontext.User.FirstOrDefaultAsync(r => r.Email == data.Email);
                    if (currentUser == null)
                        throw new InvalidOperationException($"Failed to retrieve newly created User with ID {useropresult.UserId}");

                    _mapper.Map(data, currentUser);
                    _dbcontext.Update(currentUser);
                    await _dbcontext.SaveChangesAsync();

                    return currentUser;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to Add Customer");
                    return null;
                }
            }
        }

        public async Task<User?> EditUser(UserEditModel input)
        {
            using (_logger.BeginScope("Edit User"))
            {
                try
                {
                    if (input == null)
                        throw new ArgumentNullException("input");

                    var currUser = await GetClientById(input.Id);
                    if (currUser == null)
                    {
                        throw new ArgumentException($"Cannot find user to edit {input.Id}");
                    }

                    //check muhdo barcode. Do NOT update when isNewBarcode is false!
                    if (currUser.IsNewBarcode.HasValue && currUser.IsNewBarcode.Value == false)
                    {
                        //maintain the stored data when isNewBarcode is false.
                        //This means that the barcode is already registered with this user and must NOT be updated.
                        input.IsNewBarcode = currUser.IsNewBarcode;
                        input.BarcodeString = currUser.BarcodeString;
                    }
                    else
                    {
                        bool barcodeChanged = currUser.BarcodeString != input.BarcodeString;
                        input.IsNewBarcode = barcodeChanged;
                    }

                    _mapper.Map(input, currUser);
                    _dbcontext.Update(currUser);

                    await _dbcontext.SaveChangesAsync();

                    //Update IdentityDbContext Email 
                    var identityUser = await GetIdentityUserById(currUser.FkFederatedUser);
                    if (identityUser != null && identityUser.Email != currUser.Email)
                    {
                        identityUser.Email = currUser.Email;
                        identityUser.NormalizedEmail = currUser.Email.ToUpper();

                        _iddb.Update(identityUser);
                        await _iddb.SaveChangesAsync();
                    }

                    return currUser;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to edit user {id}", input.Id);
                    return null;
                }
            }
        }

        public async Task<List<UInternalNotes>> GetInternalNotesForUserId(long userId, int count = 5)
        {
            return await _dbcontext.UInternalNotes.Where(r => r.ForUser == userId).OrderByDescending(r => r.Date).Take(count)
                .Include(r => r.ByUserNavigation)
                .ToListAsync();
        }

        public async Task<bool> AddInternalNote(long forUserId, long byUserId, string note)
        {
            using (_logger.BeginScope("AddInternalNote"))
            {
                try
                {
                    var newNote = new UInternalNotes()
                    {
                        ByUser = byUserId,
                        ForUser = forUserId,
                        Note = note,
                        Date = DateTime.UtcNow
                    };

                    _dbcontext.Add(newNote);
                    await _dbcontext.SaveChangesAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add internal note.");
                    return false;
                }
            }
        }

        public async Task<bool> CallMuhdoRegisterApiCall(MuhdoRegisterModel input)
        {
            using (_logger.BeginScope("CallMuhdoRegisterApiCall"))
            {
                try
                {
                    SignUpRequest signuprequest = _mapper.Map<SignUpRequest>(input);

                    MuhdoMiddleware midware = new MuhdoMiddleware();
                    var isSuccess = await midware.SignUp(signuprequest);

                    //update the isNewBarcode field in the relevant user call to false if muhdo sign-up is successful
                    if (isSuccess)
                    {
                        var user = await GetClientById(input.Id);
                        if (user == null)
                        {
                            _logger.LogWarning("Cannot update isNewBarcode. Cannot find user {id}", input.Id);
                            return isSuccess;
                        }

                        user.IsNewBarcode = false;
                        _dbcontext.Update(user);
                        await _dbcontext.SaveChangesAsync();
                    }

                    return isSuccess;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to call muhdo register api {data}", JsonConvert.SerializeObject(input));
                    return false;
                }
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
                User? currUser = !string.IsNullOrEmpty(objectidentifier) ? await _dbcontext.User.FirstOrDefaultAsync(r => r.FkFederatedUser.ToLower() == objectidentifier.ToLower()) : null;
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
                _dbcontext.User.Add(currUser);
                await _dbcontext.SaveChangesAsync();

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
