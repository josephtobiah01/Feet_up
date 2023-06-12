using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NdUnitMetric
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsWeight { get; set; }

        public bool IsCount { get; set; }

        public bool IsVolume { get; set; }
    }
}
