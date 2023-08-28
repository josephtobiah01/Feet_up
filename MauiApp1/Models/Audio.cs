using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class Audio : INotifyPropertyChanged
    {
        #region Fields

        private bool _isPlayVisible;
        private bool _isPauseVisible;
        private string _currentAudioPostion;

        #endregion Fields

        #region Properties

        public string AudioFile { get; set; }

        public string AudioURL { get; set; }

        public bool IsPlayVisible
        {
            get { return _isPlayVisible;}
            set
            {
                _isPlayVisible = value;
                OnPropertyChanged(nameof(IsPlayVisible));
            }
        }

        public bool IsPauseVisible
        {
            get { return _isPauseVisible; }
            set
            {
                _isPauseVisible = value;
                OnPropertyChanged(nameof(IsPauseVisible));
            }
        }

        public string CurrentAudioPostion
        {
            get { return _currentAudioPostion; }
            set
            {
                _currentAudioPostion = value;
                OnPropertyChanged(nameof(CurrentAudioPostion));
            }
        }


        #endregion Properties

        #region Constructor

        public Audio()
        {
            IsPlayVisible = true;
        }
        #endregion Constructor

        #region Interface

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion Interface


      
    }
}
