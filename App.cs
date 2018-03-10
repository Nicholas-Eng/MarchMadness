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
            // TODO: Uncomment
            // Console.WriteLine("Please Enter the First Team's Name:");
            // string team1Name = Console.ReadLine();

            // Console.WriteLine("Please Enter the Second Team's Name:");
            // string team2Name = Console.ReadLine();

            // TODO: Remove stub code for debugging
            string team1Name = "North Carolina"; //"Kansas St";
            string team2Name = "Gonzaga";
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
                Console.WriteLine(teams[winnerResult]);
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
