using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.Judge
{
    public class Track
    {
        public long Id { get; set; }

        public long ProblemId { get; set; }

        public string AuthorId { get; set; }

        public DateTime CreateTime { get; set; }

        public JudgeStatus Status { get; set; }

        public string PointStatus { get; set; }

        public string CodeEncoded { get; set; }

        public SupportProgrammingLanguage Language { get; set; }

        public string SubmitterId { get; set; }


        public void SetPointStatus(List<Point> vs)
        {
            PointStatus = System.Text.Json.JsonSerializer.Serialize(vs);
        }

        public List<Point> GetPointStatus()
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<Point>>(PointStatus);
        }
    }
}
