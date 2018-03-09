namespace MarchMadness.CSVEntities
{
    public class RegularSeasonDetailedResultsEntity
    {
        public int Season { get; set; }

        public int DayNum { get; set;}

        public int WTeamID { get; set; }

        public int WScore { get; set; }

        public int LTeamID { get; set;}

        public int LScore { get; set; }

        public char WLoc { get; set; }

        public int NumOT { get; set; }

        // Win field goals made
        public int WFGM { get; set; }

        // Win field goals attempted
        public int WFGA { get; set; }

        // Win three pointers made
        public int WFGM3 { get; set; }

        // Win three pointers attempted
        public int WFGA3 { get; set; }

        // Win free throws made
        public int WFTM { get; set; }

        // Win free throws attempted
        public int WFTA { get; set; }

        // Win offensive rebounds
        public int WOR { get; set; }

        // Win defensive rebounds
        public int WDR { get; set; }

        // Win assists
        public int WAst { get; set; }

        // Win turnovers committed
        public int WTO { get; set; }

        // Win steals
        public int WStl { get; set; }

        // Win blocks
        public int WBlk { get; set; }

        // Win personal fouls committed
        public int WPF { get; set; }

        // Lose field goals made
        public int LFGM { get; set; }

        // Lose field goals attempted
        public int LFGA { get; set; }

        // Lose three pointers made
        public int LFGM3 { get; set; }

        // Lose three pointers attempted
        public int LFGA3 { get; set; }

        // Lose free throws made
        public int LFTM { get; set; }

        // Lose free throws attempted
        public int LFTA { get; set; }

        // Lose offensive rebounds
        public int LOR { get; set; }

        // Lose defensive rebounds
        public int LDR { get; set; }

        // Lose assists
        public int LAst { get; set; }

        // Lose turnovers committed
        public int LTO { get; set; }

        // Lose steals
        public int LStl { get; set; }

        // Lose blocks
        public int LBlk { get; set; }

        // Lose personal fouls committed
        public int LPF { get; set; }
    }
}