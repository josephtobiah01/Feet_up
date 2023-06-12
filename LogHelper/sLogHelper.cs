using DAOLayer.Net7.Logs;

namespace LogHelper
{
    public class sLogHelper
    {
        public static Logs Log(string component, string type, string subtype,  int severity, string message, string innermessage = null )
        {
            return new Logs()
            {
                Component = component,
                Type = type,
                Subtype = subtype,
                Loglevel = (short)severity,
                Message = message,
                Innermessage = innermessage,
                Date = DateTime.UtcNow
            };
        }
    }


    public class Component
    {
        public static string API = "API";
        public static string Mobile = "Mobile";
        public static string Device = "Device";
    }


    public enum Severity
    {
        Critical = 0,
        Error = 1,
        Warning = 2,
        Information = 3,
    }
}