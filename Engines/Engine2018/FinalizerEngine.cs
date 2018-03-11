using System;
using System.Collections.Generic;
using System.Linq;
using MarchMadness.DataEntities;

namespace MarchMadness.Engines.Engine2018
{
    public class FinalizerEngine : IFinalizerEngine
    {
        private readonly FinalResult _finalResult;

        private readonly Dictionary<int, double> _team1SeasonResults = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _team2SeasonResults = new Dictionary<int, double>();

        private readonly Dictionary<int, double> _team1SeasonScoreResults = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _team2SeasonScoreResults = new Dictionary<int, double>();

        public FinalizerEngine(string team1Name, string team2Name)
        {
            if(string.IsNullOrWhiteSpace(team1Name))
            {
                Logger.Error("FinalizerEngine team 1 name invalid.");
            }

            if(string.IsNullOrWhiteSpace(team2Name))
            {
                Logger.Error("FinalizerEngine team 2 name invalid.");
            }

            _finalResult = new FinalResult() 
            {
                Team1 = team1Name,
                Team2 = team2Name
            };
        }

        public FinalResult ComputeWinner(MatchResults matchResults)
        {
            List<int> seasons = Constants.CSV_LIST_SEASONS.Split(',').Select(s => int.Parse(s)).OrderByDescending(x => x).ToList();

            int count = seasons.Count;
            foreach(var season in seasons)
            {
                CalculateSeasonResult(season, matchResults.Team1Results[season], matchResults.Team2Results[season], Math.Exp(count));
                CalculateSeasonScoreResult(season, matchResults.Team1ScoreResults[season], matchResults.Team2ScoreResults[season], Math.Exp(count));
                count--;
            }

            _finalResult.Winner = CalculateWinner();

            var finalScores = CalculateFinalScores();
            _finalResult.Team1Score = finalScores.team1Score;
            _finalResult.Team2Score = finalScores.team2Score;
            
            return _finalResult;
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

        private void CalculateSeasonScoreResult(int season, List<MatchCompareResult> team1SeasonScoreResults, List<MatchCompareResult> team2SeasonScoreResults, double seasonMultiplier)
        {
            _team1SeasonScoreResults.Add(season, team1SeasonScoreResults.Average(x => x.Value));
            _team2SeasonScoreResults.Add(season, team2SeasonScoreResults.Average(x => x.Value));
        }

        private (int team1Score, int team2Score) CalculateFinalScores()
        {
            int team1TotalScore = Convert.ToInt32(_team1SeasonScoreResults.Average(x => x.Value));
            int team2TotalScore = Convert.ToInt32(_team2SeasonScoreResults.Average(x => x.Value));
            return (team1TotalScore, team2TotalScore);
        }
    }
}