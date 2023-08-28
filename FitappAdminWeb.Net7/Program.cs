using FitappAdminWeb.Net7.Classes.AutoMapperProfiles;
using FitappAdminWeb.Net7.Classes.Converters;
using FitappAdminWeb.Net7.Classes.Repositories;
using DAOLayer.Net7.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DAOLayer.Net7.Supplement;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web.UI;
using FitappAdminWeb.Net7.Hubs;
using DAOLayer.Net7.Nutrition;
using DAOLayer.Net7.Chat;
using DAOLayer.Net7.User;
using DAOLayer.Net7.Logs;
using System.Net;
using FitappAdminWeb.Net7.Classes.Utilities;

System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews(options => {
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddMicrosoftIdentityUI()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
    //options.JsonSerializerOptions.Converters.Add(new NullableTimeSpanJsonConverter());
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ExerciseContext>(options =>
    options.UseSqlServer(connectionString, sqlopt => sqlopt.EnableRetryOnFailure())
);
builder.Services.AddDbContext<SupplementContext>(options =>
    options.UseSqlServer(connectionString, sqlopt => sqlopt.EnableRetryOnFailure())
);
builder.Services.AddDbContext<NutritionContext>(options =>
    options.UseSqlServer(connectionString, sqlopt => sqlopt.EnableRetryOnFailure())
);
builder.Services.AddDbContext<ChatContext>(options =>
    options.UseSqlServer(connectionString, sqlopt => sqlopt.EnableRetryOnFailure())
);
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(connectionString, sqlopt => sqlopt.EnableRetryOnFailure())
);
builder.Services.AddDbContext<LogsContext>(options =>
    options.UseSqlServer(connectionString, sqlopt => sqlopt.EnableRetryOnFailure())
);
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString, sqlopt => sqlopt.EnableRetryOnFailure())
);

builder.Services.AddAzureClients(clientBuilder =>
{
    string blobconnstring = builder.Configuration.GetValue<string>("BlobStorageAccount_ConnectionString");

    clientBuilder.ConfigureDefaults(builder.Configuration.GetSection("AzureDefaults"));
    clientBuilder.AddBlobServiceClient(blobconnstring);
});

builder.Services.AddScoped<TrainingRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<LookupRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<SupplementRepository>();
builder.Services.AddScoped<NutritionRepository>();
builder.Services.AddScoped<BlobStorageRepository>();
builder.Services.AddScoped<PromotionRepository>();
builder.Services.AddScoped<LogRepository>();

builder.Services.AddScoped<FitAppAPIUtil>();

builder.Services.AddAutoMapper(typeof(TrainingProfile));
builder.Services.AddAutoMapper(typeof(SupplementProfile));
builder.Services.AddAutoMapper(typeof(NutritionProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.HandleSameSiteCookieCompatibility();
});
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options => builder.Configuration.Bind("AzureAd", options));

builder.Services.AddSignalR();
builder.Services.AddHttpClient();

builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapHub<ChatHub>("/chathub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
