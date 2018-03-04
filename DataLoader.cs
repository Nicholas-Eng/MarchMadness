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
            team.Seasons[season].TotalRegularSeasonWins = regularSeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Count();
            if(team.Seasons[season].TotalRegularSeasonWins > 0)
            {
                team.Seasons[season].AverageRegularSeasonWinScore = regularSeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Average(w => w.WScore);
            }
            
            team.Seasons[season].TotalRegularSeasonLoses = regularSeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Count();
            if(team.Seasons[season].TotalRegularSeasonLoses > 0)
            {
                team.Seasons[season].AverageRegularSeasonLoseScore = regularSeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Average(w => w.LScore);
            }            
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
            team.Seasons[season].TotalNCAATourneySeasonWins = ncaaTourneySeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Count();
            if(team.Seasons[season].TotalNCAATourneySeasonWins > 0)
            {
                team.Seasons[season].AverageNCAATourneySeasonWinScore = ncaaTourneySeasons.Where(x => x.Season == season && x.WTeamID == team.TeamId).Average(w => w.WScore);
            }

            team.Seasons[season].TotalNCAATourneySeasonLoses = ncaaTourneySeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Count();
            if(team.Seasons[season].TotalNCAATourneySeasonLoses > 0)
            {
                team.Seasons[season].AverageRegularSeasonLoseScore = ncaaTourneySeasons.Where(x => x.Season == season && x.LTeamID == team.TeamId).Average(w => w.LScore);
            }
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
            var conf = teamConferences.Where(x => x.TeamID == team.TeamId && x.Season == season).FirstOrDefault();
            team.Seasons[season].Conference = conf != null ? conf.ConfAbbrev : null;

            if(conf != null)
            {
                var confRanking = conferenceRankings.Where(x => x.Season == season && x.ConfAbbrev == team.Seasons[season].Conference).FirstOrDefault();
                team.Seasons[season].ConferenceRanking = confRanking != null ? confRanking.Ranking : -1;
            }
            else 
            {
                team.Seasons[season].ConferenceRanking = -1;
            }
        }
    }
}