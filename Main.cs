using System;

namespace ComponentSample
{
    public class CommonResource
    {
        public static string GetFHResourcePath()
        {
            return @"./res/images/FH3/";
        }
        public static string GetTVResourcePath()
        {
            return @"./res/images/VD/";
        }
    }

    public class Application
    {
        static void Main(string[] args)
        {
            new DaliDemo().Run(new string[] { "Dali-demo" });
        }
    }
}

