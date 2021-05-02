using System;

namespace GLTech2
{
    public static class Debug
    {
        public static bool DebugWarnings { get; set; } = true;

        internal static void LogWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (DebugWarnings)
                System.Console.WriteLine(warning);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Log(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
