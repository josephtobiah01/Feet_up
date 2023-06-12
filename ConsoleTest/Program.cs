using DAOLayer.Net7.Nutrition;
using DAOLayer.Net7.Supplement;
using FeedApi.Net7.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {



        // https://fitapp-mainapi-test.azurewebsites.net
       FeedApi.Net7.FeedApi.BaseUrl = "https://localhost:7174";

      //    FeedApi.Net7.FeedApi.BaseUrl = "https://fitapp-mainapi-test.azurewebsites.net";
        FeedApi.Net7.FeedApi.UserID = 10;

        List<FeedItem> feedItems = await FeedApi.Net7.FeedApi.GetDailyFeedAsync(DateTime.Now);

        //   var _emSet = await  ExerciseApi.Net7.ExerciseApi.GetSetById(179);


        // Create User: (Note: this user will be duplicate, change name)
        //   var result = await UserApi.Net7.UserMiddleware.CreateUser("thomasempl", "thomasempl@live.com", "Aa12345!");

        //await UserApi.Net7.UserMiddleware.CreateUser("Wolframtest", "none@none.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("Mariotest", "none@none.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("Anikettest", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("Karltest", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("Heinrichtest", "none@live.com", "Aa12345!");



        //await UserApi.Net7.UserMiddleware.CreateUser("Thomastest", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("Dominiktest", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("Garytest", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user1test", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user2test", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user3test", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user4test", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user5test", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user1dev", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user2dev", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user3dev", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user4dev", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user5dev", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user6dev", "none@live.com", "Aa12345!");
        //await UserApi.Net7.UserMiddleware.CreateUser("user7dev", "none@live.com", "Aa12345!");



        DateTime utcnow = DateTime.Now;

       var users =  await UserApi.Net7.UserMiddleware.GetAllUsers();

        foreach (var u in users)
        {
            if (u.UserName != "100") continue;

            for (int i = 0; i < 40; i++)
            {


                FnsNutritionActualDay nday = new FnsNutritionActualDay();
                nday.AlcoholGramsTarget = 120;
                nday.SugarGramsTarget = 90;
                nday.FiberGramsTarget = 250;
                nday.UnsaturatedFatGramsTarget = 10;
                nday.FatGramsTarget = 125;
                nday.CrabsGramsTarget = 515;
                nday.Date = utcnow.AddDays(i);
                nday.DayCalorieTarget = 2500;
                nday.DayCalorieTargetMax = 3500;
                nday.DayCalorieTargetMin = 2000;
                nday.FkUserId = u.UserId;
        

                FnsNutritionActualMeal mmeal1 = new FnsNutritionActualMeal();
                FnsNutritionActualMeal mmeal2 = new FnsNutritionActualMeal();
                FnsNutritionActualMeal mmeal3 = new FnsNutritionActualMeal();
                mmeal1.AlcoholGramsTarget = 100;
                mmeal1.SugarGramsTarget = 110;
                mmeal1.FiberGramsTarget = 120;
                mmeal1.UnsaturatedFatGramsTarget = 130;
                mmeal1.FatGramsTarget = 140;
                mmeal1.CrabsGramsTarget = 150;
                mmeal1.ProteinGramsTarget = 160;
                mmeal1.MealCalorieTarget = 1100;
                mmeal1.MealCalorieMin = 1200;
                mmeal1.MealCalorieMax = 1300;
                mmeal1.HasTarget = true;
                mmeal1.MealTypeId = 1;
                mmeal1.FkNutritionActualDay = nday;

                mmeal2.AlcoholGramsTarget = 200;
                mmeal2.SugarGramsTarget = 210;
                mmeal2.FiberGramsTarget = 220;
                mmeal2.UnsaturatedFatGramsTarget = 230;
                mmeal2.FatGramsTarget = 240;
                mmeal2.CrabsGramsTarget = 250;
                mmeal2.ProteinGramsTarget = 260;
                mmeal2.MealCalorieTarget = 2100;
                mmeal2.MealCalorieMin = 2200;
                mmeal2.MealCalorieMax = 2300;
                mmeal2.HasTarget = true;
                mmeal2.MealTypeId = 2;
                mmeal2.FkNutritionActualDay = nday;

                mmeal3.AlcoholGramsTarget = 300;
                mmeal3.SugarGramsTarget = 310;
                mmeal3.FiberGramsTarget = 320;
                mmeal3.UnsaturatedFatGramsTarget = 330;
                mmeal3.FatGramsTarget = 340;
                mmeal3.CrabsGramsTarget = 350;
                mmeal3.ProteinGramsTarget = 360;
                mmeal3.MealCalorieTarget = 3100;
                mmeal3.MealCalorieMin = 3200;
                mmeal3.MealCalorieMax = 3300;
                mmeal3.HasTarget = true;
                mmeal3.MealTypeId = 3;
                mmeal3.FkNutritionActualDay = nday;
            }
        }
        // Login User

        //   var result = await UserApi.Net7.UserMiddleware.LoginUser("Dominiktest",  "Aa12345!");



        // Feed 

        //   var k2 = await FeedApi.Net7.FeedApi.GetDailyFeedAsync();

        //var k3 = await ExerciseApi.Net7.ExerciseApi.UpdateSet(1, 100);
        //  var k3 = await ExerciseApi.Net7.ExerciseApi.AddNewSet(53, 79);

        //var k = await FeedApi.Net7.FeedApi.GetDailyFeedAsync(DateTime.UtcNow);


        //     var k2 = await FeedApi.Net7.FeedApi.GetDailyFeedAsync(DateTime.UtcNow.AddDays(-1));


        //  var k2 = await FeedApi.Net7.FeedApi.GetDailyFeedAsync(DateTime.UtcNow.AddDays(-1));
        //   await ExerciseApi.Net7.ExerciseApi.RescheduleTrainingSession(1, 10, "fsdfsdfsd");

        var sessionn = await ExerciseApi.Net7.ExerciseApi.GetTrainingSession(37);


        //// get all eqipments
        //var equipments = await ExerciseApi.Net7.ExerciseApi.GetEquipments();

        //// get all mainmuscles
        //var mainmuscles = await ExerciseApi.Net7.ExerciseApi.GetMainMuscleWorked();

        //// get all exercises
        //var exercisetypes = await ExerciseApi.Net7.ExerciseApi.GetExerciseTypes();

        //// filter list of exercises by ones that have that equipment
        //var m1 = ExerciseApi.Net7.ExerciseApi.SortExerciseTypeByEquipment(exercisetypes, "Barbell");

        ////filter list of exercises by ones that have that mannmuscle worked
        //var m2 = ExerciseApi.Net7.ExerciseApi.SortExerciseTypeMuscleWorked(exercisetypes, "Chest");



        //   var pp = await ExerciseApi.Net7.ExerciseApi.GetDefaultExerciseFromExerciseType(13, 25);

        bool stop = true;
    }

  

}