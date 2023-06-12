using FeedApi.Net7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Data.FeedManagement
{
    public static class FeedItemManager
    {
        private static List<FeedItem> _feedItems;

        private static List<FeedItem> PopulateTrainingSessionFeedItems()
        {
            _feedItems = new List<FeedItem>();

            FeedItem feeditem = new FeedItem();
            TrainingSessionFeedItem trainingSessionFeedItem = new TrainingSessionFeedItem();
            feeditem.Title = "Full body workout";

            feeditem.Text = new List<Tuple<TextCategory, string>>();
            feeditem.Text.Add(Tuple.Create(TextCategory.Estimated_Time, "1 Hour"));
            feeditem.Text.Add(Tuple.Create(TextCategory.Target_Muscles, "Chest, Back, Biceps, Neck, Leg"));
            feeditem.Date = DateTime.UtcNow.AddMinutes(15);
            trainingSessionFeedItem.TraningSession = new ParentMiddleWare.Models.EmTrainingSession();   // pop more if needed
            feeditem.Status = FeedItemStatus.Scheduled;

            feeditem.ItemType = FeedItemType.TrainingSessionFeedItem;


            _feedItems.Add(feeditem);


            trainingSessionFeedItem = new TrainingSessionFeedItem();
            trainingSessionFeedItem.Title = "Breakfast";

            trainingSessionFeedItem.Text = new List<Tuple<TextCategory, string>>();
            trainingSessionFeedItem.Text.Add(Tuple.Create(TextCategory.Description, "Log your breakfast to keep log of your nutrients.\r\nTarget calories intake: ~780kCal"));
            trainingSessionFeedItem.Date = DateTime.UtcNow.AddHours(1);
            trainingSessionFeedItem.TraningSession = null;
            trainingSessionFeedItem.Status = FeedItemStatus.Scheduled;


            _feedItems.Add(trainingSessionFeedItem);

            trainingSessionFeedItem = new TrainingSessionFeedItem();
            trainingSessionFeedItem.Title = "Take your supplements";

            trainingSessionFeedItem.Text = new List<Tuple<TextCategory, string>>();
            trainingSessionFeedItem.Text.Add(Tuple.Create(TextCategory.Description, "Take your King Herbal supplements\r\n \r\nAfter meal"));
            trainingSessionFeedItem.Date = DateTime.UtcNow.AddHours(2);
            trainingSessionFeedItem.TraningSession = null;
            trainingSessionFeedItem.Status = FeedItemStatus.Scheduled;

            _feedItems.Add(trainingSessionFeedItem);

            trainingSessionFeedItem = new TrainingSessionFeedItem();
            trainingSessionFeedItem.Title = "Log your lunch";

            trainingSessionFeedItem.Text = new List<Tuple<TextCategory, string>>();
            trainingSessionFeedItem.Text.Add(Tuple.Create(TextCategory.Description, "Log your lunch to keep log of your nutrients.\r\nTarget calories intake: ~780kCal"));
            trainingSessionFeedItem.Date = DateTime.UtcNow.AddHours(3);
            trainingSessionFeedItem.TraningSession = null;
            trainingSessionFeedItem.Status = FeedItemStatus.Scheduled;

            _feedItems.Add(trainingSessionFeedItem);


            trainingSessionFeedItem = new TrainingSessionFeedItem();
            trainingSessionFeedItem.Title = "Full body workout";

            trainingSessionFeedItem.Text = new List<Tuple<TextCategory, string>>();
            trainingSessionFeedItem.Text.Add(Tuple.Create(TextCategory.Estimated_Time, "1 Hour"));
            trainingSessionFeedItem.Text.Add(Tuple.Create(TextCategory.Target_Muscles, "Chest, Back, Biceps, Neck, Leg"));
            trainingSessionFeedItem.Date = DateTime.UtcNow.AddDays(1).AddMinutes(15);
            trainingSessionFeedItem.TraningSession = null;
            trainingSessionFeedItem.Status = FeedItemStatus.Scheduled;


            _feedItems.Add(trainingSessionFeedItem);


            return _feedItems;
        }

        public static List<TrainingSessionFeedItem> GetTrainingSessionFeedItems()
        {
            if(_feedItems == null)
            {
                _feedItems = PopulateTrainingSessionFeedItems();
            }
            

            return _feedItems;

        }
    }
}
