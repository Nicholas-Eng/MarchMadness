using MarchMadness2018;
using MarchMadness2018.CSVEntities;
using MarchMadness2018.DataEntities;

namespace MarchMadness2018.Mappers
{
    public static class TeamMapper
    {
        public static Team Map(TeamEntity teamEntity)
        {
            if(teamEntity == null)
            {
                Logger.Error("Failed to map team from team entity");
            }

            return new Team()
            {
                TeamId = teamEntity.TeamID,
                TeamName = teamEntity.TeamName,
                FirstD1Season = teamEntity.FirstD1Season,
                LastD1Season = teamEntity.LastD1Season
            };
        }
    }
}