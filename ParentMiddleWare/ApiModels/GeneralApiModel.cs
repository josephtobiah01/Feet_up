using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.ApiModels
{
    public  class GeneralApiModel
    {
        public string FkFederatedUser { get; set; }

        public GeneralApiModel()
        {
            FkFederatedUser = string.Empty;
            this.param1 = "";
            this.param2 = "";
            this.param3 = "";
            this.param4 = "";
            this.param5 = "";
            intparam1 = 0; ;
            intparam2 = 0;
            longparam1 = 0;
            longparam2 = 0;
            doubleparam1 = 0;
            doubleparam2 = 0;
            doubleparam3 = 0;
            floatparam1 = 0;
            floatparam2 = 0;
            boolparam1 = false;
            boolparam2 = false;
            stringcontentparam1 = new StringContent("");

        }
        public string param1 { get; set; }
        public string param2 { get; set; }
        public string param3 { get; set; }
        public string param4 { get; set; }
        public string param5 { get; set; }
        public int intparam1 { get; set; } 
        public int intparam2 { get; set; } 
        public long longparam1 { get; set; } 
        public long longparam2 { get; set; } 
        public double doubleparam1 { get; set; } 
        public double doubleparam2 { get; set; } 
        public double doubleparam3 { get; set; } 
        public float floatparam1 { get; set; } 
        public float floatparam2 { get; set; } 
        public bool boolparam1 { get; set; } 
        public bool boolparam2 { get; set; } 
        public StringContent stringcontentparam1 { get; set; } 
    }
}
