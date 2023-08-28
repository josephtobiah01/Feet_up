using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Exceptions
{
    public class DeviceStorageException : Exception
    {
        public DeviceStorageException()
        {
        }

        public DeviceStorageException(string message)
            : base(message)
        {
        }

        public DeviceStorageException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
