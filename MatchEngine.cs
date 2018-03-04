using System;
using System.Linq;
using Entities;

namespace MarchMadness2018
{
    public class MatchEngine
    {
        private Team _team1;
        private Team _team2;

        public string Process(string team1Name, string team2Name) {
            string result = "";

            if(LoadTeams(team1Name, team2Name))
            {

            }

            return result;
        }

        private bool LoadTeams(string team1Name, string team2Name) {
            bool result = true;

            var teams = CSVParser.ParseCsvFile<Team>("Teams");
            _team1 = teams.Where(t => t.TeamName.Equals(team1Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            _team2 = teams.Where(t => t.TeamName.Equals(team2Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

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
    }
}
