using MauiApp1.Areas.Dashboard.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.DataManager
{
    public class NutrientsDataManager
    {
        public List<ProteinIntakeViewItem> GetProteinIntake()
        {
            List<ProteinIntakeViewItem> caloriesIntakeViewItems = new List<ProteinIntakeViewItem>();

            ProteinIntakeViewItem caloriesIntakeViewItem = new ProteinIntakeViewItem();

            caloriesIntakeViewItem = new ProteinIntakeViewItem();
            caloriesIntakeViewItem.ProteinIntakeCount = 2000;
            caloriesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 22, 0, 15, 0, TimeSpan.FromHours(8));
            caloriesIntakeViewItems.Add(caloriesIntakeViewItem);

            caloriesIntakeViewItem = new ProteinIntakeViewItem();
            caloriesIntakeViewItem.ProteinIntakeCount = 2100;
            caloriesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 23, 0, 15, 0, TimeSpan.FromHours(8));
            caloriesIntakeViewItems.Add(caloriesIntakeViewItem);

            caloriesIntakeViewItem = new ProteinIntakeViewItem();
            caloriesIntakeViewItem.ProteinIntakeCount = 1000;
            caloriesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 24, 0, 15, 0, TimeSpan.FromHours(8));
            caloriesIntakeViewItems.Add(caloriesIntakeViewItem);

            caloriesIntakeViewItem = new ProteinIntakeViewItem();
            caloriesIntakeViewItem.ProteinIntakeCount = 1900;
            caloriesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 25, 0, 15, 0, TimeSpan.FromHours(8));
            caloriesIntakeViewItems.Add(caloriesIntakeViewItem);

            caloriesIntakeViewItem = new ProteinIntakeViewItem();
            caloriesIntakeViewItem.ProteinIntakeCount = 1200;
            caloriesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 26, 0, 15, 0, TimeSpan.FromHours(8));
            caloriesIntakeViewItems.Add(caloriesIntakeViewItem);

            caloriesIntakeViewItem = new ProteinIntakeViewItem();
            caloriesIntakeViewItem.ProteinIntakeCount = 1500;
            caloriesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 27, 0, 15, 0, TimeSpan.FromHours(8));
            caloriesIntakeViewItems.Add(caloriesIntakeViewItem);

            caloriesIntakeViewItem = new ProteinIntakeViewItem();
            caloriesIntakeViewItem.ProteinIntakeCount = 2000;
            caloriesIntakeViewItem.TransactionDate = new DateTimeOffset(2023, 05, 28, 0, 15, 0, TimeSpan.FromHours(8));
            caloriesIntakeViewItems.Add(caloriesIntakeViewItem);

            return caloriesIntakeViewItems;
        }

    }
}
