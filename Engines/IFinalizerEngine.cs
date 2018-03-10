using MarchMadness.DataEntities;

namespace MarchMadness.Engines
{
    public interface IFinalizerEngine
    {
         int ComputeWinner(MatchResults matchResults);
    }
}