using System;
using MarchMadness.DataEntities;
using MarchMadness.Engines.Engine2018;

namespace MarchMadness
{
    public class App
    {
        public void DisplayWelcomeMessage() {
            Console.WriteLine("**********************************************");
            Console.WriteLine("**********************************************");
            Console.WriteLine("***** Sila Solutions Group March Madness *****");
            Console.WriteLine("***** Author: Nicholas Eng               *****");
            Console.WriteLine("**********************************************");
            Console.WriteLine("**********************************************");        
            Console.WriteLine("");    
        }

        public void Start() {         
            Console.WriteLine("Please Enter the First Team's Name:");
            string team1Name = Console.ReadLine();

            Console.WriteLine("Please Enter the Second Team's Name:");
            string team2Name = Console.ReadLine();

            // ********** FOR TESTING USE **********
            // Console.WriteLine("Please Enter test case:");
            // string team1Name = "";
            // string team2Name = "";
            // switch(Console.ReadLine())
            // {
            //     case "0":
            //         team1Name = "North Carolina";
            //         team2Name = "Gonzaga";
            //         break;
            //     case "1":
            //         team1Name = "SMU";
            //         team2Name = "USC";
            //         break;
            //     case "2":
            //         team1Name = "Wisconsin";
            //         team2Name = "Virginia Tech";
            //         break;
            //     case "3":
            //         team1Name = "Creighton";
            //         team2Name = "Rhode Island";
            //         break;
            //     case "4":
            //         team1Name = "Northwestern";
            //         team2Name = "Vanderbilt";
            //         break;
            //     case "5":
            //         team1Name = "Michigan";
            //         team2Name = "Oklahoma St";
            //         break;
            //     case "6":
            //         team1Name = "Minnesota";
            //         team2Name = "MTSU";
            //         break;
            // }

            var teams = new string[] {team1Name, team2Name};

            int winnerResult = -1;
            var dataLoaderEngine = new DataLoaderEngine();
            var matchEngine = new MatchEngine();
            var finalizerEngine = new FinalizerEngine();
            if(dataLoaderEngine.Load(team1Name, team2Name))
            {
                MatchResults engineResult = matchEngine.Process(dataLoaderEngine.Team1, dataLoaderEngine.Team2);
                winnerResult = finalizerEngine.ComputeWinner(engineResult);
            }
            
            if(winnerResult < 0 || winnerResult > 1)
            {
                Logger.Error("Failed to decided a winner");
                
            }
            else
            {
                Console.WriteLine("The Winner is: " + teams[winnerResult]);
            }


            ProcessAnotherMatch();
        }

        private void ProcessAnotherMatch() {
             Console.WriteLine("Process Another Match? (Y/N)");
            
            switch(Console.ReadLine().ToUpper())
            {
                case "Y":
                    Start();
                    break;
                case "N":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid Response. Please Try Again.");
                    ProcessAnotherMatch();
                    break;
            }
        }
    }
}
