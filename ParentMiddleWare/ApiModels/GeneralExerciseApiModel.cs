using ParentMiddleWare.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.ApiModels
{
    public class GeneralExerciseApiModel 
    {
        public long TrainingSessionId { get; set; }
        public string FkFederatedUser { get; set; }
        public GeneralExerciseApiModel()
        {
            this.FkFederatedUser = string.Empty;
            this.param1 = "";
            this.param2 = "";
            this.param3 = "";
            this.param4 = "";
            this.param5 = "";
            this.intparam1 = 0;
            this.intparam2 = 0;
            this.longparam1 = 0;
            this.longparam2 = 0;
            this.doubleparam1 = 0;
            this.doubleparam2 = 0;
            this.datetimeparam1 = DateTime.Now;
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
        public DateTime datetimeparam1 { get; set; }

    }
}
