using System;

namespace gLTech2
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
