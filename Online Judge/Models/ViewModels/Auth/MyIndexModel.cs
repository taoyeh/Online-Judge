using Online_Judge.Models.Auth;
using Online_Judge.Models.Judge;
using Online_Judge.Models.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ViewModels.Auth
{
    public class MyIndexModel
    {
        public UserProfileModel User { get; set; }

        public List<string> HaveSolvedProblem { get; set; }

        public List<LabelProfile> HaveLabel { get; set; }
        public List<LabelProfile> NotHaveLabel { get; set; }

    }
}
