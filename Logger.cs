using System;

namespace MarchMadness
{
    public static class Logger
    {

        public static void Info(string message)
        {
            if(message == null)
            {
                message = "Empty messsage";
            }

            Console.WriteLine(">>>[INFO]: " + message);
        }


        public static void Debug(string message)
        {
            if(message == null)
            {
                message = "Empty messsage";
            }

            Console.WriteLine(">>>[DEBUG]: " + message);
        }

        public static void Warn(string message)
        {
            if(message == null)
            {
                message = "Empty messsage";
            }

            Console.WriteLine(">>>[WARN]: " + message);
        }

        public static void Error(string message)
        {
            if(message == null)
            {
                message = "Empty messsage";
            }

            Console.WriteLine(">>>[ERROR]: " + message);
        }
    }
}