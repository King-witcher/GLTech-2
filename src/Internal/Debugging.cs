using System;

namespace GLTech2
{
    internal static class Debugging
    {
        public static void Debug(object o)
        {
#if DEBUG
            Console.WriteLine(o.ToString());
#endif
        }
    }
}
