using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.Auth
{
    public class UserProfileModel
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastLoginTime { get; set; }

        public int Role { get; set; }
        public int TotalSubmit { get; set; }
        public int TotalAccepted { get; set; }

    }
}
