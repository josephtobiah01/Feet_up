using DAOLayer.Net7.Chat;
using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Logs;
using DAOLayer.Net7.Nutrition;
using DAOLayer.Net7.Supplement;
using DAOLayer.Net7.User;
using FitappApi.Net7;
using FitappApi.Net7.IdentityContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ExerciseContext>(options =>
{
    options.UseSqlServer(connectionString);
  //  options.UseLazyLoadingProxies(true);
 //   options.UseMemoryCache()
    });

builder.Services.AddDbContext<SupplementContext>(options =>
{
    options.UseSqlServer(connectionString);
   // options.EnableSensitiveDataLogging();
    //  options.UseLazyLoadingProxies(true);
    //options.UseMemoryCache = ;
});

builder.Services.AddDbContext<NutritionContext>(options =>
{
    options.UseSqlServer(connectionString);
    //  options.UseLazyLoadingProxies(true);
    //options.UseMemoryCache = ;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
   // options.UseLazyLoadingProxies(true);
    //options.UseMemoryCache = ;
});

builder.Services.AddDbContext<ChatContext>(options =>
{
    options.UseSqlServer(connectionString);
    // options.UseLazyLoadingProxies(true);
    //options.UseMemoryCache = ;
});

builder.Services.AddDbContext<UserContext>(options =>
{
    options.UseSqlServer(connectionString);
    // options.UseLazyLoadingProxies(true);
    //options.UseMemoryCache = ;
});

builder.Services.AddDbContext<LogsContext>(options =>
{
    options.UseSqlServer(connectionString);
    // options.UseLazyLoadingProxies(true);
    //options.UseMemoryCache = ;
});

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders();


builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});



builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});


//builder.Services.AddMvc()
//.AddJsonOptions(
//    options =>
//    {
//        // options.JsonSerializerOptions.ReferenceHandler = .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//        options.JsonSerializerOptions.MaxDepth = 0;
//        options.JsonSerializerOptions.IgnoreNullValues = true;
//        options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;


//    });

//builder.Services.AddControllers(options =>
//{
//    options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
//    options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new JsonSerializerOptions(JsonSerializerDefaults.Web)
//    {
//        ReferenceHandler = ReferenceHandler.IgnoreCycles,

//        MaxDepth = 0,
//        IgnoreNullValues = true,
//        IgnoreReadOnlyProperties = true
//    }));
//});

//builder.Services.AddMvc().AddNewtonsoftJson(o =>
//{
//   o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//});

//builder.Services.AddControllers().AddNewtonsoftJson(o =>
//{
//    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
//        options => builder.Configuration.Bind("JwtSettings", options))
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
//        options => builder.Configuration.Bind("CookieSettings", options));

builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName.ToString());
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddApiVersioning(options => {
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
});
// Add ApiExplorer to discover versions
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});
var app = builder.Build();

app.UseResponseCompression();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

//Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
