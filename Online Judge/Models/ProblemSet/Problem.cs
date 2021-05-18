using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ProblemSet
{
    public class Problem
    {
      
        public long Id { get; set; }

        public string OrderNumber { get; set; }


        public bool  IsDelete { get; set; }
        public string Title { get; set; }

        public string AuthorId { get; set; }

        public string Description { get; set; }

        [Description("发布时间")]
        public DateTime PublishTime { get; set; }

        public string ExampleData { get; set; }
        [Description("约束条件")]
        public string JudgeProfile { get; set; }

        [Description("通过数量")]
        public string PassRate { get; set; }


        [Description("获得案例数据")]
        public TestData GetExampleData()
        {
            if (!string.IsNullOrWhiteSpace(ExampleData))
            {
                return System.Text.Json.JsonSerializer.Deserialize<TestData>(ExampleData);
            }

            return new TestData();
        }
        [Description("获得通过数据")]
        public PassRate GetPassRate()
        {
            if (!string.IsNullOrWhiteSpace(PassRate))
            {
                return System.Text.Json.JsonSerializer.Deserialize<PassRate>(PassRate);
            }

            return new PassRate();
        }
        [Description("获得约束数据")]
        public JudgeProfile GetJudgeProfile()
        {
            if (!string.IsNullOrWhiteSpace(JudgeProfile))
            {
                return System.Text.Json.JsonSerializer.Deserialize<JudgeProfile>(JudgeProfile);
            }

            return new JudgeProfile();
        }

        //设置数据
        public void SetExampleData(TestData exampleData)
        {
            ExampleData = System.Text.Json.JsonSerializer.Serialize(exampleData);
        }

        public void SetJudgeProfile(JudgeProfile judgeProfile)
        {
            JudgeProfile = System.Text.Json.JsonSerializer.Serialize(judgeProfile);
        }
        public void SetPassRate(PassRate passRate)
        {
            PassRate = System.Text.Json.JsonSerializer.Serialize(passRate);
        }

    }

}
