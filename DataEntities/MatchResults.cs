using System.Collections.Generic;

namespace MarchMadness.DataEntities
{
    public class MatchResults
    {
        public Dictionary<int, List<MatchCompareResult>> Team1Results { get; set; }
        public Dictionary<int, List<MatchCompareResult>> Team2Results { get; set; }
    }
}