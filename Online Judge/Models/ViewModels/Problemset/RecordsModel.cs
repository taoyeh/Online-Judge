using Online_Judge.Models.Judge;
using Online_Judge.Models.ProblemSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Models.ViewModels.Problemset
{
    public class RecordsModel : Track
    {
        public RecordsModel(Track track, string author)
        {
            Id = track.Id;
            PointStatus = track.PointStatus;
            ProblemId = track.ProblemId;
            Status = track.Status;
            AuthorId = track.AuthorId;
            CodeEncoded = track.CodeEncoded;
            CreateTime = track.CreateTime;

            Author = author;
        }

        public string Author { get; set; }

        public long GetPassedCount()
        {
            return GetPointStatus().Where(p => p.Status == Models.Judge.PointStatus.Accepted).LongCount();
        }

        public string GetPassRate()
        {
            return new PassRate { Pass = GetPassedCount(), Submit = GetPointStatus().Count }.ToUIString();
        }
    }
}
