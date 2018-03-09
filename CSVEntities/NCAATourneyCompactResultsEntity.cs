namespace MarchMadness.CSVEntities
{
    public class NCAATourneyCompactResultsEntity
    {
        public int Season { get; set; }

        public int DayNum { get; set;}

        public int WTeamID { get; set; }

        public int WScore { get; set; }

        public int LTeamID { get; set;}

        public int LScore { get; set; }

        public char WLoc { get; set; }

        public int NumOT { get; set; }
    }
}