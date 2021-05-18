using Microsoft.AspNetCore.Http;
using Online_Judge.Models.Label;
using Online_Judge.Models.ProblemSet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ViewModels.Problemset
{
    public class NewViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        // [Required]
        public TestData ExampleData { get; set; }

        [Required]
        public double MemoryLimit { get; set; }

        [Required]
        public double TimeLimit { get; set; }

        [Required]
        public IFormFile TestDatas { get; set; }

        // 题目所归属的标签
        public List<LabelProfile> Labels { get; set; }

    }
}
