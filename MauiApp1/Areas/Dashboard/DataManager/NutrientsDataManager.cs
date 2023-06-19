using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.DataManager
{
    public class NutrientsDataManager
    {

        public async Task<ApiResponse> GetProteinIntakeVer2(ProteinIntakeViewItem proteinIntakeViewItem)
        {
            ApiResponse ret = null;
            
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("proteinDataId", proteinIntakeViewItem.ProteinDataId.ToString());
                parameters.Add("proteinIntakeCount", proteinIntakeViewItem.ProteinIntakeCount.ToString());
                parameters.Add("transactionDate", proteinIntakeViewItem.TransactionDate.ToString());

                var result = 
            }
            
        }
        public List<ProteinIntakeViewItem> GetProteinIntake()
        {
            List<ProteinIntakeViewItem> proteinIntakeViewItems = new List<ProteinIntakeViewItem>();

            ProteinIntakeViewItem proteinIntakeViewItem = new ProteinIntakeViewItem();

            proteinIntakeViewItem = new ProteinIntakeViewItem();
            proteinIntakeViewItem.ProteinIntakeCount = 23;
            proteinIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 22, 0, 15, 0, TimeSpan.FromHours(8));
            proteinIntakeViewItems.Add(proteinIntakeViewItem);

            proteinIntakeViewItem = new ProteinIntakeViewItem();
            proteinIntakeViewItem.ProteinIntakeCount = 52;
            proteinIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 23, 0, 15, 0, TimeSpan.FromHours(8));
            proteinIntakeViewItems.Add(proteinIntakeViewItem);

            proteinIntakeViewItem = new ProteinIntakeViewItem();
            proteinIntakeViewItem.ProteinIntakeCount = 43;
            proteinIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 24, 0, 15, 0, TimeSpan.FromHours(8));
            proteinIntakeViewItems.Add(proteinIntakeViewItem);

            proteinIntakeViewItem = new ProteinIntakeViewItem();
            proteinIntakeViewItem.ProteinIntakeCount = 55;
            proteinIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 25, 0, 15, 0, TimeSpan.FromHours(8));
            proteinIntakeViewItems.Add(proteinIntakeViewItem);

            proteinIntakeViewItem = new ProteinIntakeViewItem();
            proteinIntakeViewItem.ProteinIntakeCount = 33;
            proteinIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 26, 0, 15, 0, TimeSpan.FromHours(8));
            proteinIntakeViewItems.Add(proteinIntakeViewItem);

            proteinIntakeViewItem = new ProteinIntakeViewItem();
            proteinIntakeViewItem.ProteinIntakeCount = 55;
            proteinIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 27, 0, 15, 0, TimeSpan.FromHours(8));
            proteinIntakeViewItems.Add(proteinIntakeViewItem);

            proteinIntakeViewItem = new ProteinIntakeViewItem();
            proteinIntakeViewItem.ProteinIntakeCount = 47;
            proteinIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 28, 0, 15, 0, TimeSpan.FromHours(8));
            proteinIntakeViewItems.Add(proteinIntakeViewItem);

            return proteinIntakeViewItems;
        }

        public List<CarbohydratesIntakeViewItem> GetCarbohydratesIntake()
        {
            List<CarbohydratesIntakeViewItem> carbohydratesIntakeViewItems = new List<CarbohydratesIntakeViewItem>();

            CarbohydratesIntakeViewItem carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();
            carbohydratesIntakeViewItem.CarbohydratesIntakeCount = 23;
            carbohydratesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 22, 0, 15, 0, TimeSpan.FromHours(8));
            carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();
            carbohydratesIntakeViewItem.CarbohydratesIntakeCount = 51;
            carbohydratesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 23, 0, 15, 0, TimeSpan.FromHours(8));
            carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();
            carbohydratesIntakeViewItem.CarbohydratesIntakeCount = 43;
            carbohydratesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 24, 0, 15, 0, TimeSpan.FromHours(8));
            carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();
            carbohydratesIntakeViewItem.CarbohydratesIntakeCount = 55;
            carbohydratesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 25, 0, 15, 0, TimeSpan.FromHours(8));
            carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();
            carbohydratesIntakeViewItem.CarbohydratesIntakeCount = 33;
            carbohydratesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 26, 0, 15, 0, TimeSpan.FromHours(8));
            carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();
            carbohydratesIntakeViewItem.CarbohydratesIntakeCount = 55;
            carbohydratesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 27, 0, 15, 0, TimeSpan.FromHours(8));
            carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);

            carbohydratesIntakeViewItem = new CarbohydratesIntakeViewItem();
            carbohydratesIntakeViewItem.CarbohydratesIntakeCount = 47;
            carbohydratesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 28, 0, 15, 0, TimeSpan.FromHours(8));
            carbohydratesIntakeViewItems.Add(carbohydratesIntakeViewItem);

            return carbohydratesIntakeViewItems;
        }

        public List<FatIntakeViewItem> GetFatIntake()
        {
            List<FatIntakeViewItem> fatIntakeViewItems = new List<FatIntakeViewItem>();

            FatIntakeViewItem fatIntakeViewItem = new FatIntakeViewItem();

            fatIntakeViewItem = new FatIntakeViewItem();
            fatIntakeViewItem.FatIntakeCount = 23;
            fatIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 22, 0, 15, 0, TimeSpan.FromHours(8));
            fatIntakeViewItems.Add(fatIntakeViewItem);

            fatIntakeViewItem = new FatIntakeViewItem();
            fatIntakeViewItem.FatIntakeCount = 51;
            fatIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 23, 0, 15, 0, TimeSpan.FromHours(8));
            fatIntakeViewItems.Add(fatIntakeViewItem);

            fatIntakeViewItem = new FatIntakeViewItem();
            fatIntakeViewItem.FatIntakeCount = 43;
            fatIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 24, 0, 15, 0, TimeSpan.FromHours(8));
            fatIntakeViewItems.Add(fatIntakeViewItem);

            fatIntakeViewItem = new FatIntakeViewItem();
            fatIntakeViewItem.FatIntakeCount = 55;
            fatIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 25, 0, 15, 0, TimeSpan.FromHours(8));
            fatIntakeViewItems.Add(fatIntakeViewItem);

            fatIntakeViewItem = new FatIntakeViewItem();
            fatIntakeViewItem.FatIntakeCount = 33;
            fatIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 26, 0, 15, 0, TimeSpan.FromHours(8));
            fatIntakeViewItems.Add(fatIntakeViewItem);

            fatIntakeViewItem = new FatIntakeViewItem();
            fatIntakeViewItem.FatIntakeCount = 55;
            fatIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 27, 0, 15, 0, TimeSpan.FromHours(8));
            fatIntakeViewItems.Add(fatIntakeViewItem);

            fatIntakeViewItem = new FatIntakeViewItem();
            fatIntakeViewItem.FatIntakeCount = 47;
            fatIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 28, 0, 15, 0, TimeSpan.FromHours(8));
            fatIntakeViewItems.Add(fatIntakeViewItem);

            return fatIntakeViewItems;
        }
    }
}
