using Online_Judge.Models.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ViewModels.Label
{
    public class IndexViewModel
    {
        public List<LabelProfile> LabelProfileModels { get; set; }
        public int Page { get; set; }
    }
}
