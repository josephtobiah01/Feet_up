using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO_layer.Enums
{
    public enum UserTiers
    {
        ERROR = -1,
        // 0 - 999 User Levels
        FORMER = 0,
        TRIAL = 10,
        BRONZE = 20,
        SILVER = 30,
        GOLD = 40,


        // 1000 - 99999  Trainers
        FIT_TRAINER = 1000,
        NUTRI_TRAINER = 2000,
        SUP_TRAINER = 3000,
        MC = 4000,


        // 100000+ Backoffice
        PRODUCT_MANAGER = 100000,
        FIN_MANAGER = 200000,
        SALES = 300000,

    }
}
