using FeedApi.Net7.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Supplement.ViewModels
{
    public class SupplementFeedItemViewModel : INotifyPropertyChanged
    {
        public List<SupplementPageViewModel> supplements { get; set; }
        public List<SupplementPageViewModel> allSupplements { get; set; }

        private bool isSupplementToTake;
        public bool IsSupplementToTake
        {
            get { return isSupplementToTake; }
            set
            {
                isSupplementToTake = value;
                OnPropertyChanged();
            }
        }

        private bool isAllSupplement;
        public bool IsAllSupplement
        {
            get { return isAllSupplement; }
            set
            {
                isAllSupplement = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
