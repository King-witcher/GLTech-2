using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GLTech2
{
    public static class Input
    {
        static LinkedList<Key> Keys = new LinkedList<Key>();

        public static bool IsKeyDown(Key key) => Keys.Contains(key);
        public static event Action<Key> OnKeyDown;
        public static event Action<Key> OnKeyUp;

        internal static void KeyDown(Key key)
        {
            OnKeyDown?.Invoke(key);
            Keys.AddFirst(key);
        }
        internal static void KeyUp(Key key)
        {
            OnKeyUp?.Invoke(key);
            Keys.Remove(key);
        }
    }
}
