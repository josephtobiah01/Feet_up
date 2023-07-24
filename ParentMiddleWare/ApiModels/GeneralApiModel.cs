using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.ApiModels
{
    public  class GeneralApiModel
    {
        public long UserId { get; set; }
        public string param1 { get; set; }

        public GeneralApiModel()
        {
            this.param1 = "";
            this.param2 = "";
            this.param3 = "";
            this.param4 = "";
            this.param5 = "";

        }

        public string param2 { get; set; }
        public string param3 { get; set; }
        public string param4 { get; set; }
        public string param5 { get; set; }
    }
}
