namespace Online_Judge.Models.ProblemSet
{
    public class PassRate
    {
        public long Submit { get; set; }

        public long Pass { get; set; }

        public double GetPassRate()
        {
            return (double)Pass / Submit;
        }

        public string ToUIString()
        {
            if (Pass == 0 || Submit == 0)
            {
                return "0/0 - 0%";
            }

            return Pass + "/" + Submit + " - " + (GetPassRate() * 100).ToString("0.00") + "%";
        }
    }
}