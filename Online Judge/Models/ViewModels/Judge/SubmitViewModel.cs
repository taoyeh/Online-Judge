using Online_Judge.Models.Judge;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ViewModels.Judge
{
    public class SubmitViewModel
    {
        [Required]
        public long Id { get; set; }

        public string OrderNumber { get; set; }


        [Required]
        public string Code { get; set; }

        [Required]
        public SupportProgrammingLanguage Language { get; set; }
    }
}
