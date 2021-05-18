using Online_Judge.Models.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ProblemSet
{
    public class ProblemAndHisLabel
    {
        public  Problem problem { get; set; }

        public bool IsSolved { get; set; }

        public List<LabelProfile>  labelProfiles { get; set; }
    }
}
