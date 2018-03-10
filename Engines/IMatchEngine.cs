using MarchMadness.DataEntities;

namespace MarchMadness.Engines
{
    public interface IMatchEngine
    {
         MatchResults Process(Team team1, Team team2);
    }
}