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
            CompareRegularSeason(season, team1Season.RegularSeason, team2Season.RegularSeason);
            CompareNCAATourneySeason(season, team1Season.NCAATourneySeason, team2Season.NCAATourneySeason);
        }

        // Reward the team with the better seed
        // Reward no one if equal seeds
        private void CompareSeeds(int season, int team1Seed, int team2Seed)
        {
            LowerStatComparer(season, "Seed", team1Seed, team2Seed);
        }

        // Reward the team with the better conference ranking
        // Reward no one if equal conference ranking
        private void CompareConferences(int season, int team1ConfRank, int team2ConfRank)
        {
            LowerStatComparer(season, "Conference Ranking", team1ConfRank, team2ConfRank, 10);
        }

        private void CompareRegularSeason(int season, RegularSeason t1RegSeason, RegularSeason t2RegSeason)
        {
            HigherStatComparer(season, "Total Regular Season Win/Lose Ratio",
                t1RegSeason.TotalRegularSeasonLoses > 0 ? t1RegSeason.TotalRegularSeasonWins / t1RegSeason.TotalRegularSeasonLoses : t1RegSeason.TotalRegularSeasonWins,
                t2RegSeason.TotalRegularSeasonLoses > 0 ? t2RegSeason.TotalRegularSeasonWins / t2RegSeason.TotalRegularSeasonLoses : t2RegSeason.TotalRegularSeasonWins, 10);

            CompareRegularSeasonWinStats(season, t1RegSeason, t2RegSeason);
            CompareRegularSeasonLoseStats(season, t1RegSeason, t2RegSeason);
        }

        private void CompareRegularSeasonWinStats(int season, RegularSeason t1RegSeason, RegularSeason t2RegSeason)
        {
            HigherStatComparer(season, "Average Regular Season Win Field Goals Ratio",
                t1RegSeason.AverageRegularSeasonWinFieldGoalsMade / t1RegSeason.AverageRegularSeasonWinFieldGoalsAttempted,
                t2RegSeason.AverageRegularSeasonWinFieldGoalsMade / t2RegSeason.AverageRegularSeasonWinFieldGoalsAttempted, 10);
            HigherStatComparer(season, "Average Regular Season Win Three Pointers Ratio",
                t1RegSeason.AverageRegularSeasonWinThreePointersMade / t1RegSeason.AverageRegularSeasonWinThreePointersAttempted,
                t2RegSeason.AverageRegularSeasonWinThreePointersMade / t2RegSeason.AverageRegularSeasonWinThreePointersAttempted, 10);
            HigherStatComparer(season, "Average Regular Season Win Free Throws Ratio",
                t1RegSeason.AverageRegularSeasonWinFreeThrowsMade / t1RegSeason.AverageRegularSeasonWinFreeThrowsAttempted,
                t2RegSeason.AverageRegularSeasonWinFreeThrowsMade / t2RegSeason.AverageRegularSeasonWinFreeThrowsAttempted, 10);
            HigherStatComparer(season, "Average Regular Season Win Offensive Rebounds", t1RegSeason.AverageRegularSeasonWinOffensiveRebounds, t2RegSeason.AverageRegularSeasonWinOffensiveRebounds, 10);
            HigherStatComparer(season, "Average Regular Season Win Defensive Rebounds", t1RegSeason.AverageRegularSeasonWinDefensiveRebounds, t2RegSeason.AverageRegularSeasonWinDefensiveRebounds, 10);
            HigherStatComparer(season, "Average Regular Season Win Assists", t1RegSeason.AverageRegularSeasonWinAssists, t2RegSeason.AverageRegularSeasonWinAssists, 10);
            LowerStatComparer(season, "Average Regular Season Win Turnovers", t1RegSeason.AverageRegularSeasonWinTurnoversCommitted, t2RegSeason.AverageRegularSeasonWinTurnoversCommitted, 10);
            HigherStatComparer(season, "Average Regular Season Win Steals", t1RegSeason.AverageRegularSeasonWinSteals, t2RegSeason.AverageRegularSeasonWinSteals, 10);
            HigherStatComparer(season, "Average Regular Season Win Blocks", t1RegSeason.AverageRegularSeasonWinBlocks, t2RegSeason.AverageRegularSeasonWinBlocks, 10);
            LowerStatComparer(season, "Average Regular Season Win Fouls", t1RegSeason.AverageRegularSeasonWinPersonalFoulsCommitted, t2RegSeason.AverageRegularSeasonWinPersonalFoulsCommitted, 10);
        }

        private void CompareRegularSeasonLoseStats(int season, RegularSeason t1RegSeason, RegularSeason t2RegSeason)
        {
            HigherStatComparer(season, "Average Regular Season Lose Field Goals Ratio",
                t1RegSeason.AverageRegularSeasonLoseFieldGoalsMade / t1RegSeason.AverageRegularSeasonLoseFieldGoalsAttempted,
                t2RegSeason.AverageRegularSeasonLoseFieldGoalsMade / t2RegSeason.AverageRegularSeasonLoseFieldGoalsAttempted, 10);
            HigherStatComparer(season, "Average Regular Season Lose Three Pointers Ratio",
                t1RegSeason.AverageRegularSeasonLoseThreePointersMade / t1RegSeason.AverageRegularSeasonLoseThreePointersAttempted,
                t2RegSeason.AverageRegularSeasonLoseThreePointersMade / t2RegSeason.AverageRegularSeasonLoseThreePointersAttempted, 10);
            HigherStatComparer(season, "Average Regular Season Lose Free Throws Ratio",
                t1RegSeason.AverageRegularSeasonLoseFreeThrowsMade / t1RegSeason.AverageRegularSeasonLoseFreeThrowsAttempted,
                t2RegSeason.AverageRegularSeasonLoseFreeThrowsMade / t2RegSeason.AverageRegularSeasonLoseFreeThrowsAttempted, 10);
            HigherStatComparer(season, "Average Regular Season Lose Offensive Rebounds", t1RegSeason.AverageRegularSeasonLoseOffensiveRebounds, t2RegSeason.AverageRegularSeasonLoseOffensiveRebounds, 10);
            HigherStatComparer(season, "Average Regular Season Lose Defensive Rebounds", t1RegSeason.AverageRegularSeasonLoseDefensiveRebounds, t2RegSeason.AverageRegularSeasonLoseDefensiveRebounds, 10);
            HigherStatComparer(season, "Average Regular Season Lose Assists", t1RegSeason.AverageRegularSeasonLoseAssists, t2RegSeason.AverageRegularSeasonLoseAssists, 10);
            LowerStatComparer(season, "Average Regular Season Lose Turnovers", t1RegSeason.AverageRegularSeasonLoseTurnoversCommitted, t2RegSeason.AverageRegularSeasonLoseTurnoversCommitted, 10);
            HigherStatComparer(season, "Average Regular Season Lose Steals", t1RegSeason.AverageRegularSeasonLoseSteals, t2RegSeason.AverageRegularSeasonLoseSteals, 10);
            HigherStatComparer(season, "Average Regular Season Lose Blocks", t1RegSeason.AverageRegularSeasonLoseBlocks, t2RegSeason.AverageRegularSeasonLoseBlocks, 10);
            LowerStatComparer(season, "Average Regular Season Lose Fouls", t1RegSeason.AverageRegularSeasonLosePersonalFoulsCommitted, t2RegSeason.AverageRegularSeasonLosePersonalFoulsCommitted, 10);
        }

        private void CompareNCAATourneySeason(int season, NCAATourneySeason t1TourneySeason, NCAATourneySeason t2TourneySeason)
        {
            HigherStatComparer(season, "Total Tourney Season Win/Lose Ratio",
                t1TourneySeason.TotalNCAATourneySeasonLoses > 0 ? t1TourneySeason.TotalNCAATourneySeasonWins / t1TourneySeason.TotalNCAATourneySeasonLoses : t1TourneySeason.TotalNCAATourneySeasonWins,
                t2TourneySeason.TotalNCAATourneySeasonLoses > 0 ? t2TourneySeason.TotalNCAATourneySeasonWins / t2TourneySeason.TotalNCAATourneySeasonLoses : t2TourneySeason.TotalNCAATourneySeasonWins, 3);

            CompareTourneySeasonWinStats(season, t1TourneySeason, t2TourneySeason);
            CompareTourneySeasonLoseStats(season, t1TourneySeason, t2TourneySeason);
        }

        private void CompareTourneySeasonWinStats(int season, NCAATourneySeason t1TourneySeason, NCAATourneySeason t2TourneySeason)
        {
            HigherStatComparer(season, "Average NCAATourney Season Win Field Goals Ratio",
                t1TourneySeason.AverageNCAATourneySeasonWinFieldGoalsMade / t1TourneySeason.AverageNCAATourneySeasonWinFieldGoalsAttempted,
                t2TourneySeason.AverageNCAATourneySeasonWinFieldGoalsMade / t2TourneySeason.AverageNCAATourneySeasonWinFieldGoalsAttempted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Win Three Pointers Ratio",
                t1TourneySeason.AverageNCAATourneySeasonWinThreePointersMade / t1TourneySeason.AverageNCAATourneySeasonWinThreePointersAttempted,
                t2TourneySeason.AverageNCAATourneySeasonWinThreePointersMade / t2TourneySeason.AverageNCAATourneySeasonWinThreePointersAttempted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Win Free Throws Ratio",
                t1TourneySeason.AverageNCAATourneySeasonWinFreeThrowsMade / t1TourneySeason.AverageNCAATourneySeasonWinFreeThrowsAttempted,
                t2TourneySeason.AverageNCAATourneySeasonWinFreeThrowsMade / t2TourneySeason.AverageNCAATourneySeasonWinFreeThrowsAttempted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Win Offensive Rebounds", t1TourneySeason.AverageNCAATourneySeasonWinOffensiveRebounds, t2TourneySeason.AverageNCAATourneySeasonWinOffensiveRebounds, 10);
            HigherStatComparer(season, "Average NCAATourney Season Win Defensive Rebounds", t1TourneySeason.AverageNCAATourneySeasonWinDefensiveRebounds, t2TourneySeason.AverageNCAATourneySeasonWinDefensiveRebounds, 10);
            HigherStatComparer(season, "Average NCAATourney Season Win Assists", t1TourneySeason.AverageNCAATourneySeasonWinAssists, t2TourneySeason.AverageNCAATourneySeasonWinAssists, 10);
            LowerStatComparer(season, "Average NCAATourney Season Win Turnovers", t1TourneySeason.AverageNCAATourneySeasonWinTurnoversCommitted, t2TourneySeason.AverageNCAATourneySeasonWinTurnoversCommitted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Win Steals", t1TourneySeason.AverageNCAATourneySeasonWinSteals, t2TourneySeason.AverageNCAATourneySeasonWinSteals, 10);
            HigherStatComparer(season, "Average NCAATourney Season Win Blocks", t1TourneySeason.AverageNCAATourneySeasonWinBlocks, t2TourneySeason.AverageNCAATourneySeasonWinBlocks, 10);
            LowerStatComparer(season, "Average NCAATourney Season Win Fouls", t1TourneySeason.AverageNCAATourneySeasonWinPersonalFoulsCommitted, t2TourneySeason.AverageNCAATourneySeasonWinPersonalFoulsCommitted, 10);
        }

        private void CompareTourneySeasonLoseStats(int season, NCAATourneySeason t1TourneySeason, NCAATourneySeason t2TourneySeason)
        {
            HigherStatComparer(season, "Average NCAATourney Season Lose Field Goals Ratio",
                t1TourneySeason.AverageNCAATourneySeasonLoseFieldGoalsMade / t1TourneySeason.AverageNCAATourneySeasonLoseFieldGoalsAttempted,
                t2TourneySeason.AverageNCAATourneySeasonLoseFieldGoalsMade / t2TourneySeason.AverageNCAATourneySeasonLoseFieldGoalsAttempted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Lose Three Pointers Ratio",
                t1TourneySeason.AverageNCAATourneySeasonLoseThreePointersMade / t1TourneySeason.AverageNCAATourneySeasonLoseThreePointersAttempted,
                t2TourneySeason.AverageNCAATourneySeasonLoseThreePointersMade / t2TourneySeason.AverageNCAATourneySeasonLoseThreePointersAttempted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Lose Free Throws Ratio",
                t1TourneySeason.AverageNCAATourneySeasonLoseFreeThrowsMade / t1TourneySeason.AverageNCAATourneySeasonLoseFreeThrowsAttempted,
                t2TourneySeason.AverageNCAATourneySeasonLoseFreeThrowsMade / t2TourneySeason.AverageNCAATourneySeasonLoseFreeThrowsAttempted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Lose Offensive Rebounds", t1TourneySeason.AverageNCAATourneySeasonLoseOffensiveRebounds, t2TourneySeason.AverageNCAATourneySeasonLoseOffensiveRebounds, 10);
            HigherStatComparer(season, "Average NCAATourney Season Lose Defensive Rebounds", t1TourneySeason.AverageNCAATourneySeasonLoseDefensiveRebounds, t2TourneySeason.AverageNCAATourneySeasonLoseDefensiveRebounds, 10);
            HigherStatComparer(season, "Average NCAATourney Season Lose Assists", t1TourneySeason.AverageNCAATourneySeasonLoseAssists, t2TourneySeason.AverageNCAATourneySeasonLoseAssists, 10);
            LowerStatComparer(season, "Average NCAATourney Season Lose Turnovers", t1TourneySeason.AverageNCAATourneySeasonLoseTurnoversCommitted, t2TourneySeason.AverageNCAATourneySeasonLoseTurnoversCommitted, 10);
            HigherStatComparer(season, "Average NCAATourney Season Lose Steals", t1TourneySeason.AverageNCAATourneySeasonLoseSteals, t2TourneySeason.AverageNCAATourneySeasonLoseSteals, 10);
            HigherStatComparer(season, "Average NCAATourney Season Lose Blocks", t1TourneySeason.AverageNCAATourneySeasonLoseBlocks, t2TourneySeason.AverageNCAATourneySeasonLoseBlocks, 10);
            LowerStatComparer(season, "Average NCAATourney Season Lose Fouls", t1TourneySeason.AverageNCAATourneySeasonLosePersonalFoulsCommitted, t2TourneySeason.AverageNCAATourneySeasonLosePersonalFoulsCommitted, 10);
        }

        private void LowerStatComparer(int season, string category, double t1Value, double t2Value, double spread = 0)
        {
            var winnerCompareResult = new MatchCompareResult() { Category = category, Value = 1 };
            var loserCompareResult = new MatchCompareResult() { Category = category, Value = 0 };

            if(t1Value < t2Value)
            {
                if(spread > 0) {
                    winnerCompareResult.Value = winnerCompareResult.Value + ((t2Value - t1Value) / spread);
                }

                _team1Results[season].Add(winnerCompareResult);
                _team2Results[season].Add(loserCompareResult);
            }
            else if(t2Value < t1Value)
            {
                if(spread > 0) {
                    winnerCompareResult.Value = winnerCompareResult.Value + ((t1Value - t2Value) / spread);
                }

                _team2Results[season].Add(winnerCompareResult);
                _team1Results[season].Add(loserCompareResult);
            }
            else
            {
                _team1Results[season].Add(loserCompareResult);
                _team2Results[season].Add(loserCompareResult);
            }
        }

        private void HigherStatComparer(int season, string category, double t1Value, double t2Value, double spread = 0)
        {
            var winnerCompareResult = new MatchCompareResult() { Category = category, Value = 1 };
            var loserCompareResult = new MatchCompareResult() { Category = category, Value = 0 };

            if(t1Value > t2Value)
            {
                if(spread > 0) {
                    winnerCompareResult.Value = winnerCompareResult.Value + ((t1Value - t2Value) / spread);
                }
                
                _team1Results[season].Add(winnerCompareResult);
                _team2Results[season].Add(loserCompareResult);
            }
            else if(t2Value > t1Value)
            {
                if(spread > 0) {
                    winnerCompareResult.Value = winnerCompareResult.Value + ((t2Value - t1Value) / spread);
                }
                
                _team2Results[season].Add(winnerCompareResult);
                _team1Results[season].Add(loserCompareResult);
            }
            else
            {
                _team1Results[season].Add(loserCompareResult);
                _team2Results[season].Add(loserCompareResult);
            }
        }
    }
}
