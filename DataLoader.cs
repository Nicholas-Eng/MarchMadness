using System;
using System.Collections.Generic;
using System.Linq;
using MarchMadness2018.CSVEntities;
using MarchMadness2018.DataEntities;
using MarchMadness2018.Mappers;

namespace MarchMadness2018
{
    public class DataLoader
    {
        private Team _team1;
        private Team _team2;

        public string Load(string team1Name, string team2Name)
        {
            string result = "";

            if(LoadTeams(team1Name, team2Name))
            {
                LoadSeasons();
                LoadSeeds();
                LoadRegularSeasons();
                LoadNCAATourneySeasons();
                LoadConferenceRankings();
                LoadRegularSeasonTeamBoxScores();
                LoadNCAATourneySeasonTeamBoxScores();
            }

            return result;
        }

        private bool LoadTeams(string team1Name, string team2Name)
        {
            bool result = true;

            var teams = CSVParser.ParseCsvFile<TeamEntity>(Constants.TEAMS_FILE);
            _team1 = teams.Select(x => TeamMapper.Map(x)).Where(t => t.TeamName.Equals(team1Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            _team2 = teams.Select(x => TeamMapper.Map(x)).Where(t => t.TeamName.Equals(team2Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if(_team1 == null) {
                Logger.Error("Failed to find team with name: " + team1Name);
                result = false;
            }

            if(_team2 == null) {
                Logger.Error("Failed to find team with name: " + team2Name);
                result = false;
            }

            return result;
        }

        private void LoadSeasons()
        {
            _team1.Seasons = new Dictionary<int, Season>();
            _team2.Seasons = new Dictionary<int, Season>();

            string[] seasonsToLoad = Constants.CSV_LIST_SEASONS.Split(',');

            foreach(string season in seasonsToLoad)
            {
                LoadSeason(_team1, season);
                LoadSeason(_team2, season);
            }
        }

        private void LoadSeason(Team team, string seasonYear)
        {
            int seasonId = -1;
            if(int.TryParse(seasonYear, out seasonId))
            {
                team.Seasons.Add(seasonId, new Season());
            }
            else
            {
                Logger.Error("Failed to load season: " + seasonYear);
            }
        }

        private void LoadSeeds()
        {
            var seeds = CSVParser.ParseCsvFile<NCAATourneySeedsEntity>(Constants.NCAA_TOURNEY_SEEDS_FILE);

            foreach(var season in _team1.Seasons)
            {
                LoadSeed(seeds, _team1, season.Key);
                LoadSeed(seeds, _team2, season.Key);
            }
        }

        //Return -1 if the team had no seed for that season
        private void LoadSeed(IEnumerable<NCAATourneySeedsEntity> seeds, Team team, int season)
        {
            NCAATourneySeedsEntity seedCsvEntity = seeds.Where(x => x.Season == season && x.TeamID == team.TeamId).FirstOrDefault();

            int seed = -1;
            if(seedCsvEntity != null)
            {           
                // Check if this was a tied seed for that region
                if(seedCsvEntity.Seed.Length > 3)
                {
                    seedCsvEntity.Seed = seedCsvEntity.Seed.Substring(0, 3);
                }

                // Remove out the region character in front and parse to int
                if(!int.TryParse(seedCsvEntity.Seed.Substring(1), out seed))
                {
                    Logger.Error($"Failed to load seed for team: {team.TeamName} and season: {season}");
                }
            }

            team.Seasons[season].Seed = seed;
        }

        private void LoadRegularSeasons()
        {
            var regularSeasons = CSVParser.ParseCsvFile<RegularSeasonCompactResultsEntity>(Constants.REGULAR_SEASON_COMPACT_RESULTS_FILE);

            foreach(var season in _team1.Seasons)
            {
                LoadRegularSeason(regularSeasons, _team1, season.Key);
                LoadRegularSeason(regularSeasons, _team2, season.Key);
            }
        }

        private void LoadRegularSeason(IEnumerable<RegularSeasonCompactResultsEntity> regularSeasons, Team team, int season)
        {
            var regularSeasonToLoad = new RegularSeason();

            regularSeasonToLoad.TotalRegularSeasonWins = regularSeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Count();
            if(regularSeasonToLoad.TotalRegularSeasonWins > 0)
            {
                regularSeasonToLoad.AverageRegularSeasonWinScore = regularSeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Average(w => w.WScore);
            }
            
            regularSeasonToLoad.TotalRegularSeasonLoses = regularSeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Count();
            if(regularSeasonToLoad.TotalRegularSeasonLoses > 0)
            {
                regularSeasonToLoad.AverageRegularSeasonLoseScore = regularSeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Average(w => w.LScore);
            }            

            team.Seasons[season].RegularSeason = regularSeasonToLoad;
        }

        private void LoadNCAATourneySeasons()
        {
            var ncaaTourneySeasons = CSVParser.ParseCsvFile<NCAATourneyCompactResultsEntity>(Constants.NCAA_TOURNEY_COMPACT_RESULTS_FILE);

            foreach(var season in _team1.Seasons)
            {
                LoadNCAATourneySeason(ncaaTourneySeasons, _team1, season.Key);
                LoadNCAATourneySeason(ncaaTourneySeasons, _team2, season.Key);
            }
        }

        private void LoadNCAATourneySeason(IEnumerable<NCAATourneyCompactResultsEntity> ncaaTourneySeasons, Team team, int season)
        {
            var ncaaSeasonToLoad = new NCAATourneySeason();

            ncaaSeasonToLoad.TotalNCAATourneySeasonWins = ncaaTourneySeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Count();
            if(ncaaSeasonToLoad.TotalNCAATourneySeasonWins > 0)
            {
                ncaaSeasonToLoad.AverageNCAATourneySeasonWinScore = ncaaTourneySeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Average(w => w.WScore);
            }

            ncaaSeasonToLoad.TotalNCAATourneySeasonLoses = ncaaTourneySeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Count();
            if(ncaaSeasonToLoad.TotalNCAATourneySeasonLoses > 0)
            {
                ncaaSeasonToLoad.AverageNCAATourneySeasonLoseScore = ncaaTourneySeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Average(w => w.LScore);
            }

            team.Seasons[season].WasInNCAATourneySeason = ncaaSeasonToLoad.TotalNCAATourneySeasonWins > 0 && ncaaSeasonToLoad.TotalNCAATourneySeasonLoses > 0;

            team.Seasons[season].NCAATourneySeason = ncaaSeasonToLoad;
        }

        private void LoadConferenceRankings()
        {
            var conferenceRankings = CSVParser.ParseCsvFile<ConferenceRankingsEntity>(Constants.CONFERENCE_RANKINGS_FILE);
            var teamConferences = CSVParser.ParseCsvFile<TeamConferencesEntity>(Constants.TEAM_CONFERENCES_FILE);

            foreach(var season in _team1.Seasons)
            {
                LoadConferenceRanking(conferenceRankings, teamConferences, _team1, season.Key);
                LoadConferenceRanking(conferenceRankings, teamConferences, _team2, season.Key);
            }
        }

        private void LoadConferenceRanking(IEnumerable<ConferenceRankingsEntity> conferenceRankings, IEnumerable<TeamConferencesEntity> teamConferences, Team team, int season)
        {
            var conferenceToLoad = new Conference();
            
            var conf = teamConferences.Where(x => x.TeamID == team.TeamId && x.Season == season).FirstOrDefault();
            conferenceToLoad.ConferenceAbbrev = conf != null ? conf.ConfAbbrev : null;

            if(conf != null)
            {
                var confRanking = conferenceRankings.Where(x => x.Season == season && x.ConfAbbrev == conferenceToLoad.ConferenceAbbrev).FirstOrDefault();
                conferenceToLoad.ConferenceRanking = confRanking != null ? confRanking.Ranking : -1;
            }
            else 
            {
                conferenceToLoad.ConferenceRanking = -1;
            }
            
            team.Seasons[season].Conference = conferenceToLoad;
        }

        private void LoadRegularSeasonTeamBoxScores()
        {
            var regSeasonDetails = CSVParser.ParseCsvFile<RegularSeasonDetailedResultsEntity>(Constants.REGULAR_SEASON_DETAILED_RESULTS_FILE);

            foreach(var season in _team1.Seasons)
            {
                LoadRegularSeasonWinTeamBoxScore(regSeasonDetails, _team1, season.Key);
                LoadRegularSeasonWinTeamBoxScore(regSeasonDetails, _team2, season.Key);
                LoadRegularSeasonLoseTeamBoxScore(regSeasonDetails, _team1, season.Key);
                LoadRegularSeasonLoseTeamBoxScore(regSeasonDetails, _team2, season.Key);
            }
        }

        private void LoadRegularSeasonWinTeamBoxScore(IEnumerable<RegularSeasonDetailedResultsEntity> regSeasonDetails, Team team, int season)
        {
            IEnumerable<RegularSeasonDetailedResultsEntity> teamRegSeasonDetails = regSeasonDetails.Where(x => x.WTeamID == team.TeamId &&  x.Season == season);
            RegularSeason regularSeasonToLoad = team.Seasons[season].RegularSeason;
            
            regularSeasonToLoad.AverageRegularSeasonWinFieldGoalsMade = teamRegSeasonDetails.Average(x => x.WFGM);
            regularSeasonToLoad.AverageRegularSeasonWinFieldGoalsAttempted = teamRegSeasonDetails.Average(x => x.WFGA);
            regularSeasonToLoad.AverageRegularSeasonWinThreePointersMade = teamRegSeasonDetails.Average(x => x.WFGM3);
            regularSeasonToLoad.AverageRegularSeasonWinThreePointersAttempted = teamRegSeasonDetails.Average(x => x.WFGA3);
            regularSeasonToLoad.AverageRegularSeasonWinFreeThrowsMade = teamRegSeasonDetails.Average(x => x.WFTM);
            regularSeasonToLoad.AverageRegularSeasonWinFreeThrowsAttempted = teamRegSeasonDetails.Average(x => x.WFTA);
            regularSeasonToLoad.AverageRegularSeasonWinOffensiveRebounds = teamRegSeasonDetails.Average(x => x.WOR);
            regularSeasonToLoad.AverageRegularSeasonWinDefensiveRebounds = teamRegSeasonDetails.Average(x => x.WDR);
            regularSeasonToLoad.AverageRegularSeasonWinAssists = teamRegSeasonDetails.Average(x => x.WAst);
            regularSeasonToLoad.AverageRegularSeasonWinTurnoversCommitted = teamRegSeasonDetails.Average(x => x.WTO);
            regularSeasonToLoad.AverageRegularSeasonWinSteals = teamRegSeasonDetails.Average(x => x.WStl);
            regularSeasonToLoad.AverageRegularSeasonWinBlocks = teamRegSeasonDetails.Average(x => x.WBlk);
            regularSeasonToLoad.AverageRegularSeasonWinPersonalFoulsCommitted = teamRegSeasonDetails.Average(x => x.WPF);
        }

        private void LoadRegularSeasonLoseTeamBoxScore(IEnumerable<RegularSeasonDetailedResultsEntity> regSeasonDetails, Team team, int season)
        {
            IEnumerable<RegularSeasonDetailedResultsEntity> teamRegSeasonDetails = regSeasonDetails.Where(x => x.LTeamID == team.TeamId &&  x.Season == season);
            RegularSeason regularSeasonToLoad = team.Seasons[season].RegularSeason;
            
            regularSeasonToLoad.AverageRegularSeasonLoseFieldGoalsMade = teamRegSeasonDetails.Average(x => x.LFGM);
            regularSeasonToLoad.AverageRegularSeasonLoseFieldGoalsAttempted = teamRegSeasonDetails.Average(x => x.LFGA);
            regularSeasonToLoad.AverageRegularSeasonLoseThreePointersMade = teamRegSeasonDetails.Average(x => x.LFGM3);
            regularSeasonToLoad.AverageRegularSeasonLoseThreePointersAttempted = teamRegSeasonDetails.Average(x => x.LFGA3);
            regularSeasonToLoad.AverageRegularSeasonLoseFreeThrowsMade = teamRegSeasonDetails.Average(x => x.LFTM);
            regularSeasonToLoad.AverageRegularSeasonLoseFreeThrowsAttempted = teamRegSeasonDetails.Average(x => x.LFTA);
            regularSeasonToLoad.AverageRegularSeasonLoseOffensiveRebounds = teamRegSeasonDetails.Average(x => x.LOR);
            regularSeasonToLoad.AverageRegularSeasonLoseDefensiveRebounds = teamRegSeasonDetails.Average(x => x.LDR);
            regularSeasonToLoad.AverageRegularSeasonLoseAssists = teamRegSeasonDetails.Average(x => x.LAst);
            regularSeasonToLoad.AverageRegularSeasonLoseTurnoversCommitted = teamRegSeasonDetails.Average(x => x.LTO);
            regularSeasonToLoad.AverageRegularSeasonLoseSteals = teamRegSeasonDetails.Average(x => x.LStl);
            regularSeasonToLoad.AverageRegularSeasonLoseBlocks = teamRegSeasonDetails.Average(x => x.LBlk);
            regularSeasonToLoad.AverageRegularSeasonLosePersonalFoulsCommitted = teamRegSeasonDetails.Average(x => x.LPF);
        }

        private void LoadNCAATourneySeasonTeamBoxScores()
        {
            var ncaaTourneySeasonDetails = CSVParser.ParseCsvFile<NCAATourneyDetailedResultsEntity>(Constants.NCAA_TOURNEY_DETAILED_RESULTS_FILE);

            foreach(var season in _team1.Seasons)
            {
                LoadNCAATourneySeasonWinTeamBoxScore(ncaaTourneySeasonDetails, _team1, season.Key);
                LoadNCAATourneySeasonWinTeamBoxScore(ncaaTourneySeasonDetails, _team2, season.Key);
                LoadNCAATourneySeasonLoseTeamBoxScore(ncaaTourneySeasonDetails, _team1, season.Key);
                LoadNCAATourneySeasonLoseTeamBoxScore(ncaaTourneySeasonDetails, _team2, season.Key);
            }
        }

        private void LoadNCAATourneySeasonWinTeamBoxScore(IEnumerable<NCAATourneyDetailedResultsEntity> ncaaTourneySeasonDetails, Team team, int season)
        {
            IEnumerable<NCAATourneyDetailedResultsEntity> teamNcaaTourneySeasonDetails = ncaaTourneySeasonDetails.Where(x => x.WTeamID == team.TeamId &&  x.Season == season);
            NCAATourneySeason ncaaTourneySeasonToLoad = team.Seasons[season].NCAATourneySeason;
            
            if(team.Seasons[season].WasInNCAATourneySeason)
            {
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinFieldGoalsMade = teamNcaaTourneySeasonDetails.Average(x => x.WFGM);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinFieldGoalsAttempted = teamNcaaTourneySeasonDetails.Average(x => x.WFGA);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinThreePointersMade = teamNcaaTourneySeasonDetails.Average(x => x.WFGM3);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinThreePointersAttempted = teamNcaaTourneySeasonDetails.Average(x => x.WFGA3);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinFreeThrowsMade = teamNcaaTourneySeasonDetails.Average(x => x.WFTM);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinFreeThrowsAttempted = teamNcaaTourneySeasonDetails.Average(x => x.WFTA);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinOffensiveRebounds = teamNcaaTourneySeasonDetails.Average(x => x.WOR);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinDefensiveRebounds = teamNcaaTourneySeasonDetails.Average(x => x.WDR);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinAssists = teamNcaaTourneySeasonDetails.Average(x => x.WAst);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinTurnoversCommitted = teamNcaaTourneySeasonDetails.Average(x => x.WTO);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinSteals = teamNcaaTourneySeasonDetails.Average(x => x.WStl);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinBlocks = teamNcaaTourneySeasonDetails.Average(x => x.WBlk);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonWinPersonalFoulsCommitted = teamNcaaTourneySeasonDetails.Average(x => x.WPF);
            }
        }

        private void LoadNCAATourneySeasonLoseTeamBoxScore(IEnumerable<NCAATourneyDetailedResultsEntity> ncaaTourneySeasonDetails, Team team, int season)
        {
            IEnumerable<NCAATourneyDetailedResultsEntity> teamNcaaTourneySeasonDetails = ncaaTourneySeasonDetails.Where(x => x.LTeamID == team.TeamId &&  x.Season == season);
            NCAATourneySeason ncaaTourneySeasonToLoad = team.Seasons[season].NCAATourneySeason;
            
            if(team.Seasons[season].WasInNCAATourneySeason)
            {
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseFieldGoalsMade = teamNcaaTourneySeasonDetails.Average(x => x.LFGM);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseFieldGoalsAttempted = teamNcaaTourneySeasonDetails.Average(x => x.LFGA);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseThreePointersMade = teamNcaaTourneySeasonDetails.Average(x => x.LFGM3);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseThreePointersAttempted = teamNcaaTourneySeasonDetails.Average(x => x.LFGA3);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseFreeThrowsMade = teamNcaaTourneySeasonDetails.Average(x => x.LFTM);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseFreeThrowsAttempted = teamNcaaTourneySeasonDetails.Average(x => x.LFTA);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseOffensiveRebounds = teamNcaaTourneySeasonDetails.Average(x => x.LOR);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseDefensiveRebounds = teamNcaaTourneySeasonDetails.Average(x => x.LDR);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseAssists = teamNcaaTourneySeasonDetails.Average(x => x.LAst);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseTurnoversCommitted = teamNcaaTourneySeasonDetails.Average(x => x.LTO);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseSteals = teamNcaaTourneySeasonDetails.Average(x => x.LStl);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLoseBlocks = teamNcaaTourneySeasonDetails.Average(x => x.LBlk);
                ncaaTourneySeasonToLoad.AverageNCAATourneySeasonLosePersonalFoulsCommitted = teamNcaaTourneySeasonDetails.Average(x => x.LPF);
            }
        }
    }
}