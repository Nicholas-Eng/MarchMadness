namespace MarchMadness2018.DataEntities
{
    public class Season
    {
        public int Seed { get; set; }

        public Conference Conference { get; set; }

        public RegularSeason RegularSeason { get; set; }

        public bool WasInNCAATourneySeason { get; set; }

        public NCAATourneySeason NCAATourneySeason { get; set; }
    }
}