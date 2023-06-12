using System;
using System.Collections.Generic;

namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmEquipment
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

    }
}