using ImageApi.Net7;
using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Models;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.NutrientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.DataManager
{
    public class NutrientsDataManager
    {

        //public async Task<List<NutrientsDataResponse>> GetNutrients (DateTime startDate, DateTime endDate)
        //{
        //    NutrientsDataResponse ret = null;

        //    List<NutrientsDataResponse> nutrientsIntakeViewItems = new List<NutrientsDataResponse>();
        //    try
        //    {
        //        string baseUrl = "";
        //        var client = new HttpClient();

        //        var requestUrl = $"{baseUrl}{startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}";

        //        var message = new HttpRequestMessage(HttpMethod.Get, requestUrl);

        //        var response = await client.SendAsync(message);

        //        var responseString = await response.Content.ReadAsStringAsync();

        //        var data = JsonConvert.DeserializeObject<NutrientsDataResponse>(responseString);

        //        var nutrientsIntakeViewItem = new NutrientsDataResponse()
        //        {
        //            ProteinDataId = Guid.NewGuid(),
        //            ProteinIntakeCount = 5,
        //            TransactionDate = new DateTimeOffset(new DateTime(2023, 6, 21, 10, 30, 0)),

        //            AverageCarbsIntake = data.AverageCarbsIntake,
        //            AverageFatIntake = data.AverageFatIntake,
        //            AverageProteinIntake = 3,

        //            TotalCarbs = data.TotalCarbs,
        //            TotalFat = data.TotalFat,
        //            TotalProtein = 4,

        //            AvgCurrentCalories = data.AvgCurrentCalories,
        //            AvgTargetCalories = data.AvgTargetCalories

        //        };

        //        nutrientsIntakeViewItems.Add(nutrientsIntakeViewItem);

        //        return nutrientsIntakeViewItems;
        //    }

        //    catch (Exception nie)
        //    {
        //        Console.WriteLine(nie);
        //        return new List<NutrientsDataResponse>()
        //        {
        //            new NutrientsDataResponse()
        //            {
        //                Code = 0000,
        //                Message = "Please check your internet connection."
        //            }
                    
        //        };
        //    }
            
        //}
        public List<ProteinIntakeViewItem> GetProteinIntake(NutrientsDataResponse model)
        {
     
            List<ProteinIntakeViewItem> proteinIntakeViewItems = new List<ProteinIntakeViewItem>();

            ProteinIntakeViewItem proteinIntakeViewItem = new ProteinIntakeViewItem();

            proteinIntakeViewItem = new ProteinIntakeViewItem();

            foreach (var item in model.ProteinModel)
            {
                proteinIntakeViewItem.ProteinIntakeCount = item.ProteinIntakeCount;
                proteinIntakeViewItem.TransactionDate = item.TransactionDate;
                proteinIntakeViewItems.Add(proteinIntakeViewItem);
            }

            return proteinIntakeViewItems;
        }

        public List<CarbohydratesIntakeViewItem> GetCarbohydratesIntake(NutrientsDataResponse model)
        {
            List<CarbohydratesIntakeViewItem> carbohydratesIntakeViewItems = new List<CarbohydratesIntakeViewItem>();

            CarbohydratesIntakeViewItem carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();

            foreach (var item in model.CarbohydratesModel)
            {
                carbohydratesIntakeViewItem.CarbohydratesIntakeCount = item.CarbsIntakeCount;
                carbohydratesIntakeViewItem.TransactionDate = item.TransactionDate;
                carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);
            }

            return carbohydratesIntakeViewItems;
        }

        public List<FatIntakeViewItem> GetFatIntake(NutrientsDataResponse model)
        {
            List<FatIntakeViewItem> fatIntakeViewItems = new List<FatIntakeViewItem>();

            FatIntakeViewItem fatIntakeViewItem = new FatIntakeViewItem();

            fatIntakeViewItem = new FatIntakeViewItem();

            foreach (var item in model.FatModel)
            {
                fatIntakeViewItem.FatIntakeCount = item.FatIntakeCount;
                fatIntakeViewItem.TransactionDate = item.TransactionDate;
                fatIntakeViewItems.Add(fatIntakeViewItem);
            }

            return fatIntakeViewItems;

        }
    }
}
