using System.Collections.Generic;
using System.Linq;
using MarchMadness.DataEntities;

namespace MarchMadness.Engines.Engine2018
{
    public class FinalizerEngine : IFinalizerEngine
    {
        private readonly Dictionary<int, double> _team1SeasonResults = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _team2SeasonResults = new Dictionary<int, double>();

        public int ComputeWinner(MatchResults matchResults)
        {
            List<int> seasons = Constants.CSV_LIST_SEASONS.Split(',').Select(s => int.Parse(s)).OrderByDescending(x => x).ToList();

            int count = seasons.Count;
            foreach(var season in seasons)
            {
                CalculateSeasonResult(season, matchResults.Team1Results[season], matchResults.Team2Results[season], count / seasons.Count);

                count--;
            }

            return CalculateWinner();
        }

        private void CalculateSeasonResult(int season, List<MatchCompareResult> team1SeasonResults, List<MatchCompareResult> team2SeasonResults, double seasonMultiplier)
        {
            _team1SeasonResults.Add(season, team1SeasonResults.Sum(x => x.Value) * seasonMultiplier);
            _team2SeasonResults.Add(season, team2SeasonResults.Sum(x => x.Value) * seasonMultiplier);
        }

        private int CalculateWinner()
        {
            double team1Total = _team1SeasonResults.Sum(x => x.Value);
            double team2Total = _team2SeasonResults.Sum(x => x.Value);

            if(team1Total > team2Total)
            {
                return 0;
            }
            else if(team2Total > team1Total)
            {
                return 1;
            }
            else 
            {
                return -1;
            }
        }
    }
}