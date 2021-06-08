using System;

namespace GLTech2
{
    internal static partial class Program
    {
        static void Main()
        {
            Console.WriteLine("Press any key to start.");
            Console.ReadKey();
            Console.Write("\b \b");

            GridExample();

            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
            Console.Write("\b \b");
            Console.WriteLine("Closing...");
        }
    }
}
