using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Online_Judge.Models.Auth;
using Online_Judge.Models.Judge;
using Online_Judge.Models.Label;
using Online_Judge.Models.ProblemSet;

namespace Online_Judge.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }
        // 题目信息
        public DbSet<Problem> Problems { get; set; }

        // 判题情况
        public DbSet<Track> Tracks { get; set; }
        
        //用户信息
        public DbSet<UserProfileModel> UserProfileModels { get; set; }

        //标签信息
        public DbSet<LabelProfile> LabelProfiles { get; set; }

        //用户标签
        public DbSet<UserLabel> UserLabels { get; set; }

        //题目标签
        public DbSet<ProblemLabel> ProblemLabels { get; set; }
    }
}
