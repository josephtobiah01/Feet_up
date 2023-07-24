using ImageApi.Net7;
using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Interfaces;
using MauiApp1.Models;
using ParentMiddleWare.NutrientModels;

namespace MauiApp1.Services
{
    public class NutrientsIntakeService : INutrientsIntakeService
    {
        public async Task<List<NutrientsDataResponse>> GetAllData(DateTime selectedDate)
        {
            var model = await NutritionApi.GetNutrientsFirstPage(selectedDate.Date);
            if (model.TotalNutrientsModel != null && model.TotalNutrientsModel.Count > 0 && model.AvgTargetCalories <= 0)
            {
                model.AvgCurrentCalories = (int)(model.AvgCurrentCalories * 1.2);
            }
            
            List<NutrientsDataResponse> result = new List<NutrientsDataResponse>();
            result.Add(model);

            return result;



            //NutrientsDataResponse model = new NutrientsDataResponse();


            //model.ProteinModel = new List<Protein>();
            //model.CarbohydratesModel = new List<Carbohydrates>();
            //model.FatModel = new List<Fat>();
            //model.TotalNutrientsModel = new List<TotalNutrients>();

            //model.ProteinModel.Add(new Protein() { ProteinIntakeCount = 550, TransactionDate = selectedDate});

            //model.CarbohydratesModel.Add(new Carbohydrates() { CarbsIntakeCount = 450, TransactionDate = selectedDate });

            //model.FatModel.Add(new Fat() { FatIntakeCount = 350, TransactionDate = selectedDate });

            //model.TotalNutrientsModel.Add(new TotalNutrients { CaloriesTranscribedTotal = 2400 });

            //TotalNutrients totalNutrients = new TotalNutrients();

            //foreach (var protein in model.ProteinModel)
            //{
            //    totalNutrients.TotalProtein += protein.ProteinIntakeCount;
            //}

            //foreach (var carbohydrates in model.CarbohydratesModel)
            //{
            //    totalNutrients.TotalCarbs += carbohydrates.CarbsIntakeCount;
            //}

            //foreach (var fat in model.FatModel)
            //{
            //    totalNutrients.TotalFat += fat.FatIntakeCount;
            //}

            //foreach (var totalTranscribedCalorie in model.TotalNutrientsModel)
            //{
            //    totalNutrients.CaloriesTranscribedTotal = totalTranscribedCalorie.CaloriesTranscribedTotal;
            //}

            //model.TotalNutrientsModel.Add(totalNutrients);

            //List<NutrientsDataResponse> result = new List<NutrientsDataResponse>();
            //result.Add(model);

            //return result;
        }


        public async Task<List<NutrientsIntakeViewItem>> LoadTotalNutrientsBarChart(DateTime selectedDate)
        {
            List<NutrientsDataResponse> nutrientsDataResponse = await GetAllData(selectedDate);

            List<NutrientsIntakeViewItem> nutrientsIntakeViewItems = new List<NutrientsIntakeViewItem>();

            if (nutrientsDataResponse[0].TotalNutrientsModel.Count != 0) 
            {
                List<TotalNutrients> totalNutrientsModel = nutrientsDataResponse[0].TotalNutrientsModel;

                foreach (TotalNutrients totalNutrients in totalNutrientsModel)
                {

                    NutrientsIntakeViewItem nutrientsItem = new NutrientsIntakeViewItem();
                    nutrientsItem.CaloriesProtein = totalNutrients.TotalProtein * 4;
                    nutrientsItem.CaloriesFat = totalNutrients.TotalFat * 9;
                    nutrientsItem.CaloriesCarbs = totalNutrients.TotalCarbs * 4;
                    //    nutrientsItem.CaloriesCarbs += totalNutrients.AverageSugarIntake * 4;
                    nutrientsItem.TotalCalories = nutrientsItem.CaloriesProtein + nutrientsItem.CaloriesFat + nutrientsItem.CaloriesCarbs;

                    nutrientsItem.TranscribedCalories = totalNutrients.CaloriesTranscribedTotal;

                    if (totalNutrients.CaloriesTranscribedTotal != 0 )
                    {
                        double ratio = nutrientsItem.TotalCalories / totalNutrients.CaloriesTranscribedTotal;

                        nutrientsItem.ProteinToDisplay = (int)Math.Ceiling((decimal)(nutrientsItem.CaloriesProtein / ratio));
                        nutrientsItem.CarbohydratesToDisplay = (int)Math.Ceiling((decimal)(nutrientsItem.CaloriesCarbs / ratio));
                        nutrientsItem.FatToDisplay = (int)Math.Ceiling((decimal)(nutrientsItem.CaloriesFat / ratio));


                        //nutrientsItem.ProteinPercentage = nutrientsItem.CaloriesProtein / nutrientsItem.TotalCalories;
                        //nutrientsItem.CarbPercentage = nutrientsItem.CaloriesCarbs / nutrientsItem.TotalCalories;
                        //nutrientsItem.FatPercentage = nutrientsItem.CaloriesFat / nutrientsItem.TotalCalories;

                        //nutrientsItem.ProteinToDisplay = (int)Math.Ceiling((decimal)(nutrientsItem.ProteinPercentage * totalNutrients.CaloriesTranscribedTotal));
                        //nutrientsItem.CarbohydratesToDisplay = (int)Math.Ceiling((decimal)(nutrientsItem.CarbPercentage * totalNutrients.CaloriesTranscribedTotal));
                        //nutrientsItem.FatToDisplay = (int)Math.Ceiling((decimal)(nutrientsItem.FatPercentage * totalNutrients.CaloriesTranscribedTotal));

                    }
                    else
                    {
                        nutrientsItem.ProteinToDisplay = 0;
                        nutrientsItem.CarbohydratesToDisplay = 0;
                        nutrientsItem.FatToDisplay = 0;
                    }

                    if (nutrientsDataResponse[0].AvgTargetCalories != 0 && nutrientsDataResponse[0] != null)
                    {
                        nutrientsItem.TargetCalories = (int)nutrientsDataResponse[0].AvgTargetCalories;
                    }
                    else
                    {
                        nutrientsItem.TargetCalories = 0;
                    }
                    nutrientsIntakeViewItems.Add(nutrientsItem);
                }
            }
            else
            {
                NutrientsIntakeViewItem defaultItem = new NutrientsIntakeViewItem
                {
                    ProteinToDisplay = 0,
                    CarbohydratesToDisplay = 0,
                    FatToDisplay = 0,
                    TargetCalories = 0,
                    TotalCalories = 0
                };
                nutrientsIntakeViewItems.Add(defaultItem);
            }
            return nutrientsIntakeViewItems;
        }
    }
}
