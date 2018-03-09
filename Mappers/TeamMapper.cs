using MarchMadness;
using MarchMadness.CSVEntities;
using MarchMadness.DataEntities;

namespace MarchMadness.Mappers
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