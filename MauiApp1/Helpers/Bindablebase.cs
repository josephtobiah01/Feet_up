using MauiApp1.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Helpers
{
    public class Bindablebase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected bool SetPropertyValue<T>(ref T field, T value, string propertyName)
        {
            if (Equals(field, value))
            {
                return false;
            }
            else
            {
                field = value;
                return true;
            }
        }

        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
