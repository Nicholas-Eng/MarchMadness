namespace MarchMadness2018.DataEntities
{
    public class Season
    {
        public int Seed { get; set; }

        public string Conference { get; set; }

        public int ConferenceRanking { get; set; }

        public int TotalRegularSeasonWins { get; set; }

        public double AverageRegularSeasonWinScore { get; set; }

        public int TotalRegularSeasonLoses { get; set; }

        public double AverageRegularSeasonLoseScore { get; set; }

        public int TotalNCAATourneySeasonWins { get; set; }

        public double AverageNCAATourneySeasonWinScore { get; set; }

        public int TotalNCAATourneySeasonLoses { get; set; }

        public double AverageNCAATourneySeasonLoseScore { get; set; }
    }
}