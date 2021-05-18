using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ViewModels.Label
{
    public class NewViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Introduction { get; set; }
    }
}
