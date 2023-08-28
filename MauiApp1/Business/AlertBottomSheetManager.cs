using MauiApp1.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Business
{
    public class AlertBottomSheetManager
    {
        #region [Fields]

        public event EventHandler<AlertMessageEventArgs> OpenConfirmationBottomSheet;
        public event EventHandler AcceptButtonConfirmationBottomSheet;
        public event EventHandler CancelButtonConfirmationBottomSheet;
        public event EventHandler<AlertMessageEventArgs> OpenAlertBottomSheet;
        public event EventHandler OpenInternalServerErrorBottomSheet;

        #endregion

        #region [Static Public Methods :: Tasks :: Confirmation Bottom Sheet]

        public void ShowConfirmationMessage(string title, string message, string acceptMessage, string cancelMessage)
        {
            AlertMessageEventArgs confirmed = null;

            AlertMessageEventArgs alertMessageEventArgs = new AlertMessageEventArgs();
            alertMessageEventArgs.Title = title;
            alertMessageEventArgs.Message = message;
            alertMessageEventArgs.AcceptMessage = acceptMessage;
            alertMessageEventArgs.CancelMessage = cancelMessage;

            if (OpenAlertBottomSheet != null)
            {
                OpenConfirmationBottomSheet.Invoke(this, alertMessageEventArgs);
            }
            else if (App.Current.MainPage != null)
            {
                App.Current.MainPage.DisplayAlert(title, message, cancelMessage);
            }
            else
            {

            }
        }

        public void AcceptConfirmationBottomSheet_Invoke()
        {
            if (AcceptButtonConfirmationBottomSheet != null)
            {
                AcceptButtonConfirmationBottomSheet.Invoke(this, null);
            }
        }

        public void CancelConfirmationBottomSheet_Invoke()
        {
            if (CancelButtonConfirmationBottomSheet != null)
            {
                CancelButtonConfirmationBottomSheet.Invoke(this, null);
            }
        }

        public void ClearEventSubscriptionOpenConfirmationBottomSheet()
        {
            OpenConfirmationBottomSheet = null;
        }

        public void ClearConfirmationBottomSheetActionEvents()
        {
            //if(AcceptButtonConfirmationBottomSheet != null)
            //{
            //    foreach (Delegate subscription in AcceptButtonConfirmationBottomSheet.GetInvocationList())
            //    {
            //        AcceptButtonConfirmationBottomSheet -= (EventHandler)subscription;
            //    }
            //}

            //if (CancelButtonConfirmationBottomSheet != null)
            //{
            //    foreach (Delegate subscription in CancelButtonConfirmationBottomSheet.GetInvocationList())
            //    {
            //        CancelButtonConfirmationBottomSheet -= (EventHandler)subscription;
            //    }
            //}


            AcceptButtonConfirmationBottomSheet = null;
            CancelButtonConfirmationBottomSheet = null;
        }


        #endregion

        #region [Static Public Methods :: Tasks :: Alert Bottom Sheet]

        public void ShowAlertMessage(string title, string message, string cancelMessage)
        {
            AlertMessageEventArgs alertMessageEventArgs = new AlertMessageEventArgs();
            alertMessageEventArgs.Title = title;
            alertMessageEventArgs.Message = message;
            alertMessageEventArgs.AcceptMessage = string.Empty;
            alertMessageEventArgs.CancelMessage = cancelMessage;

            if (OpenAlertBottomSheet != null)
            {
                OpenAlertBottomSheet.Invoke(this, alertMessageEventArgs);
            }
            else if (App.Current.MainPage != null)
            {
                App.Current.MainPage.DisplayAlert(title, message, cancelMessage);
            }
            else
            {

            }
        }

        public void ClearEventSubscriptionOpenAlertBottomSheet()
        {
            //if(OpenAlertBottomSheet != null)
            //{
            //    foreach (Delegate subscription in OpenAlertBottomSheet.GetInvocationList())
            //    {
            //        OpenAlertBottomSheet -= (EventHandler<AlertMessageEventArgs>)subscription;
            //    }
            //}
            OpenAlertBottomSheet = null;
        }

        #endregion

        #region [Static Public Methods :: Tasks :: Internal Server Error Bottom Sheet]

        public void ShowInternalServerErrorMessage(string title, string message, string cancelMessage)
        {
            if (OpenInternalServerErrorBottomSheet != null)
            {
                OpenInternalServerErrorBottomSheet.Invoke(this, new EventArgs());
            }
            else if (App.Current.MainPage != null)
            {
                App.Current.MainPage.DisplayAlert(title, message, cancelMessage);
            }
            else
            {

            }
        }

        public void ClearEventSubscriptionOpenInternalServerErrorBottomSheet()
        {
            //if (OpenInternalServerErrorBottomSheet != null)
            //{
            //    foreach (Delegate subscription in OpenInternalServerErrorBottomSheet.GetInvocationList())
            //    {
            //        OpenInternalServerErrorBottomSheet -= (EventHandler)subscription;
            //    }
            //}

            OpenInternalServerErrorBottomSheet = null;
        }

        #endregion
    }
}
