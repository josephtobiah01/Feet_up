using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Models;
using ParentMiddleWare.NutrientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Interfaces
{
    public interface INutrientsIntakeService
    {
         Task<List<NutrientsDataResponse>> GetAllData(DateTime selectedDate);
         Task<List<NutrientsIntakeViewItem>> LoadTotalNutrientsBarChart(DateTime selectedDate);

    }
}
