using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.Label
{
    public class LabelProfile
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Introduction { get; set; }

        public bool IsDelete { get; set; }
    }
}
