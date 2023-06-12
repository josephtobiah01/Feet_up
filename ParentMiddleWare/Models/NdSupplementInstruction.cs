using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NdSupplementInstruction
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public bool RequiresSourceOfFat { get; set; }

        public bool TakeAfterMeal { get; set; }

        public bool TakeBeforeSleep { get; set; }

        public bool TakeOnEmptyStomach { get; set; }
    }
}
