using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ProblemSet
{
    public class ProblemLabel
    {
        public long Id { get; set; }
        public string ProblemId { get; set; }
        public string LabelId { get; set; }
        public int Weight { get; set; }
    }
}
