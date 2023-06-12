using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using Android.App;
using AndroidX.Core.App;
//using Xamarin.Android.ShortcutBadger;
#elif IOS
using UIKit;
using UserNotifications;
#endif

namespace MauiApp1.Areas.Badges.Models
{
    /* to call, use something like this:
     *var count = 1;
     *NotificationCounter.Default.SetNotificationCount(count);
     */
    public interface INotificationCounter
    {
        void SetNotificationCount(int count);
    }

    public static class NotificationCounter
    {
        static INotificationCounter? defaultImplementation;

        public static void SetNotificationCount(int count)
        {
            Default.SetNotificationCount(count);
        }

        public static INotificationCounter Default =>
            defaultImplementation ??= new NotificationCounterImplementation();

        internal static void SetDefault(INotificationCounter? implementation) =>
            defaultImplementation = implementation;
    }
#if ANDROID
    public class NotificationCounterImplementation : INotificationCounter
    {
	    public void SetNotificationCount(int count)
	    {
		   //ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(global::Android.App.Application.Context, count);
        }
    }
#elif IOS
    public class NotificationCounterImplementation : INotificationCounter
    {
        public void SetNotificationCount(int count)
        {
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Badge, (r, e) =>
            {
            });
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = count;
        }
    }
#else
    public class NotificationCounterImplementation : INotificationCounter
    {
	    public void SetNotificationCount(int count)
	    {
		    //var badgeUpdater =	BadgeUpdateManager.CreateBadgeUpdaterForApplication();
		    if (count <= 0)
		    {
			  //  badgeUpdater.Clear();
		    }
		    else
		    {
			  //  var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

			  //  var badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
			  //  badgeElement?.SetAttribute("value", count.ToString());

			 //   var badge = new BadgeNotification(badgeXml);
			 //   badgeUpdater.Update(badge);
		    }
	    }
    }
#endif
}