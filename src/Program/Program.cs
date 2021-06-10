using System;

namespace GLTech2
{
    internal static partial class Program
    {
        static void Main()
        {
            Vector a = (0, 10);
            a.Angle = 3f;

			Console.WriteLine(a);

            Console.WriteLine("Press any key to start.");
            Console.ReadKey();
            Console.Write("\b \b");

            GridExample();
            AnimatedExample();

            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
            Console.Write("\b \b");
            Console.WriteLine("Closing...");
        }
    }
}
