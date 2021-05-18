using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.Auth
{
    public class UserLabel
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string LabelId { get; set; }

        public int Weight { get; set; }

    }
}
