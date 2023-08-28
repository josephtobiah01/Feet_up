using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Interfaces
{
    public interface IRecordAudioService
    {
        void StartRecord();
        string StopRecord();
        void PauseRecord();
        void ResetRecord();
    }
}
