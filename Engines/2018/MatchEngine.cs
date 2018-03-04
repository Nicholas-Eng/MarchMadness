using System;
using System.Collections.Generic;
using System.Linq;
using MarchMadness2018.CSVEntities;
using MarchMadness2018.DataEntities;
using MarchMadness2018.Mappers;

namespace MarchMadness2018.Engines
{
    public class MatchEngine
    {
        private Team _team1;
        private Team _team2;

        public string Process(string team1Name, string team2Name)
        {
            string result = "";

            if(LoadTeams(team1Name, team2Name))
            {
                LoadSeasons();
                LoadSeeds();
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
    }
}
