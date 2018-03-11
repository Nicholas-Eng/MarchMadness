using MarchMadness.DataEntities;

namespace MarchMadness.Engines
{
    public interface IFinalizerEngine
    {
         FinalResult ComputeWinner(MatchResults matchResults);
    }
}