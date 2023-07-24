using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApi.Net7.Models
{
    public class LoginUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTimeOffset Offset { get; set; }
    }

    public class SetOffsetModel
    {
        public DateTimeOffset Offset { get; set; }
        public long UserId { get; set; }
    }
}
