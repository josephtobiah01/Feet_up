using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.EventArguments
{
    public class AlertMessageEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string AcceptMessage { get; set; }
        public string CancelMessage { get; set; }

        public bool Confirmed { get; set; }
    }
}
