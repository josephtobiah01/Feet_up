using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.ApiModels
{
    public class MinUserModel
    {
        public string FkFederatedUser { get; set; } = string.Empty;
        public long ID { get; set; }
        public string NM { get; set; } = string.Empty;
    }
}
