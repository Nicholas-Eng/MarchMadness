using System.Collections.Generic;

namespace MarchMadness2018.DataEntities
{
    public class Team
    { 
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public int FirstD1Season { get; set; }

        public int LastD1Season { get; set; }

        public Dictionary<int, Season> Seasons { get; set; }
    }
}