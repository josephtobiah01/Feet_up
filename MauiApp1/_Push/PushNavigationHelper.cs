using MauiApp1.Areas.Chat.Views;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Interfaces;
using MauiApp1.Pages;
using MauiApp1.Pages.Chat;
using MauiApp1.Services;
using Statics;

namespace MauiApp1._Push
{
    public class PushNavigationHelper
    {
        public static MauiApp1.Pages.Index RootPage { get; set; }
        public static async Task<bool> HandleNotificationTab(string aaction, string parameter)
        {
            try
            {
                NavigationIntercept intercept = new NavigationIntercept() { Action = aaction, Parameter1 = parameter };
                switch (aaction)
                {
                    case Strings.NOTIF_CHAT:
                        {
                            if (App.Current.MainPage.Navigation.NavigationStack.Last().GetType().Name != "ViewHybridChatContentPage")
                            {
                                ISelectedImageService selectedImageService = new SelectedImageService();
                                ViewHybridChatContentPage viewHybridChatContentPage = new ViewHybridChatContentPage(selectedImageService);
                                await Application.Current.MainPage.Navigation.PushAsync(viewHybridChatContentPage);
                            }
                            break;
                        }
                    case Strings.NOTIF_NUTRIENT:
                        {
                            HTMLBridge.RefreshData = null;

                            if (RootPage != null)
                            {
                         //       await App.Current.MainPage.Navigation.PopToRootAsync();
                                await Navigate_to_feed_item(intercept);
                            }
                            else
                            {
                                MauiApp1.Pages.Index.navigationIntercept = intercept;
                            }
                            break;
                        }
                    case Strings.NOTIF_SUPPLEMENT:
                        {
                            HTMLBridge.RefreshData = null;
                            if (RootPage != null)
                            {
                          //      await App.Current.MainPage.Navigation.PopToRootAsync();
                                await Navigate_to_feed_item(intercept);
                            }
                            else
                            {
                                MauiApp1.Pages.Index.navigationIntercept = intercept;
                            }
                            break;
                        }
                    case Strings.NOTIF_TRAINING:
                        {
                            HTMLBridge.RefreshData = null;

                            if (RootPage != null)
                            {
                           //     await App.Current.MainPage.Navigation.PopToRootAsync();
                                await Navigate_to_feed_item(intercept);
                            }
                            else
                            {
                                MauiApp1.Pages.Index.navigationIntercept = intercept;
                            }
                            break;
                        }
                    case Strings.NOTIF_TRANSCRIPT:
                        {
                            HTMLBridge.RefreshData = null;

                            if (RootPage != null)
                            {
                             //   await App.Current.MainPage.Navigation.PopToRootAsync();
                                await Navigate_to_feed_item(intercept);
                            }
                            else
                            {
                                MauiApp1.Pages.Index.navigationIntercept = intercept;
                            }
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static async Task DoIntercept(MauiApp1.Pages.NavigationIntercept intercept)
        {

        }
        // Open Chatpage 
        public static async Task Navigate_to_chat()
        {

        }

        // Navigate to the feeditem -- i want to give it a hidden long ID -- can taht be done?
        public static async Task Navigate_to_feed_item(MauiApp1.Pages.NavigationIntercept intercept)
        {
            try
            {
                if (RootPage != null)
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();

                    RootPage.LockScrollingToNow();
                    await RootPage.RefreshPage();
                    await RootPage.DoNavigationIntercept(intercept.Action, intercept.Parameter1);
                }
            }
            catch(Exception e)
            {
            
            }
        }
    }
}
