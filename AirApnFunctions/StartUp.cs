//using DAOLayer.Net7.User;
//using Microsoft.Azure.Functions.Extensions.DependencyInjection;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//[assembly: FunctionsStartup(typeof(AzureFunctionWithEF.StartUp))]

//namespace AzureFunctionWithEF
//{
//    public class StartUp : FunctionsStartup
//    {
//        public override void Configure(IFunctionsHostBuilder builder)
//        {
//            string connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
//            builder.Services.AddDbContext<UserContext>(
//              options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));
//        }
//    }
//}