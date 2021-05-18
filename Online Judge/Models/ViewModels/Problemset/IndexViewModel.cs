using Online_Judge.Models.Label;
using Online_Judge.Models.ProblemSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ViewModels.Problemset
{
    public class IndexViewModel
    {
        public List<ProblemAndHisLabel> ProblemModels { get; set; }


        public List<LabelProfile> AllLables { get; set; }
        public int Page { get; set; }
    }
}
