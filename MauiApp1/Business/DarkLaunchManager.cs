using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Business
{
    public static class DarkLaunchManager
    {
        public static bool DarkLaunched(string userName)
        {
            switch (userName)
            {
                case "Thomastest":
                case "kieltest":
                case "Joseph":
                case "Joseph306":
                case "gerlyntest":
                case "petertest":
                case "DominikTest1":
                case "DominikTest2":

                    return true;
                    break;

                default:
                    return false;
                    break;
            }
        }
    }
}
