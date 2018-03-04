using System;

namespace MarchMadness2018
{
    public class App
    {
        public void DisplayWelcomeMessage() {
            Console.WriteLine("**********************************************");
            Console.WriteLine("**********************************************");
            Console.WriteLine("***** Sila Solutions Group March Madness *****");
            Console.WriteLine("***** Author: Nicholas Eng               *****");
            Console.WriteLine("***** Year: 2018                         *****");
            Console.WriteLine("**********************************************");
            Console.WriteLine("**********************************************");        
            Console.WriteLine("");    
        }

        public void Start() {           
            Console.WriteLine("Please Enter the First Team's Name:");
            string team1Name = Console.ReadLine();

            Console.WriteLine("Please Enter the Second Team's Name:");
            string team2Name = Console.ReadLine();

            string engineResult = new MatchEngine().Process(team1Name, team2Name);

            if(!string.IsNullOrWhiteSpace(engineResult)) {
                Console.WriteLine(engineResult);
            }
            else {
                Logger.Error("Match Engine Failed");
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
