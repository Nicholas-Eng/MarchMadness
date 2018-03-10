using System.Collections.Generic;
using System.Linq;
using MarchMadness.DataEntities;

namespace MarchMadness.Engines.Engine2018
{
    public class MatchEngine : IMatchEngine
    {
        private readonly Dictionary<int, List<MatchCompareResult>> _team1Results = new Dictionary<int, List<MatchCompareResult>>();
        private readonly Dictionary<int, List<MatchCompareResult>> _team2Results = new Dictionary<int, List<MatchCompareResult>>();

        public MatchResults Process(Team team1, Team team2)
        {
            var results = new MatchResults() 
            { 
                Team1Results = _team1Results, 
                Team2Results = _team2Results
            };

            if(team1 == null)
            {
                Logger.Error("Failed to process match: team1 is null");
                return null;
            }

            if(team2 == null)
            {
                Logger.Error("Failed to process match: team2 is null");
                return null;
            }

            IEnumerable<int> seasons = Constants.CSV_LIST_SEASONS.Split(',').Select(s => int.Parse(s));
            foreach(var season in seasons)
            {
                _team1Results.Add(season, new List<MatchCompareResult>());
                _team2Results.Add(season, new List<MatchCompareResult>());

                Season team1Season = team1.Seasons[season];
                Season team2Season = team2.Seasons[season];
                ProcessSeason(season, team1Season, team2Season);
            }

            return results;
        }

        private void ProcessSeason(int season, Season team1Season, Season team2Season) {
            CompareSeeds(season, team1Season.Seed, team2Season.Seed);
            CompareConferences(season, team1Season.Conference.ConferenceRanking, team2Season.Conference.ConferenceRanking);
        }

        // Reward the team with the better seed
        // Reward no one if equal seeds
        private void CompareSeeds(int season, int team1Seed, int team2Seed)
        {
            var result = new MatchCompareResult() { Category = "Seed" };

            if(team1Seed < team2Seed)
            {
                result.Value = team2Seed - team1Seed;
                _team1Results[season].Add(result);
            }
            else if(team2Seed < team1Seed)
            {
                result.Value = team1Seed - team2Seed;
                _team2Results[season].Add(result);
            }
        }

        // Reward the team with the better conference ranking
        // Reward no one if equal conference ranking
        private void CompareConferences(int season, int team1ConfRank, int team2ConfRank)
        {
            var result = new MatchCompareResult() { Category = "Conference Ranking" };

            if(team1ConfRank < team2ConfRank)
            {
                result.Value = team2ConfRank - team1ConfRank;
                _team1Results[season].Add(result);
            }
            else if(team2ConfRank < team1ConfRank)
            {
                result.Value = team1ConfRank - team2ConfRank;
                _team2Results[season].Add(result);
            }
        }
    }
}
