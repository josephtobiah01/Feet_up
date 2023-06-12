using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApi.Net7.Models
{
    public class UserOpResult
    {
        [JsonConstructor]
        public UserOpResult()
        {
            Errors = new List<string>();
            UserName = string.Empty;
        }

        public bool isSuccess { get; set; }
        public List<string> Errors { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

    }
}
