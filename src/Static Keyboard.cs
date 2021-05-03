using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GLTech2
{
    public static class Keyboard
    {
        static LinkedList<Key> Keys = new LinkedList<Key>();

        public static bool IsKeyDown(Key key) => Keys.Contains(key);
        public static event Action<Key> OnKeyDown;
        public static event Action<Key> OnKeyUp;

        internal static void KeyDown(Key key)
        {
            Keys.AddFirst(key);
            OnKeyDown?.Invoke(key);
        }
        internal static void KeyUp(Key key)
        {
            while (Keys.Contains(key))  // Suboptimal
                Keys.Remove(key);
            OnKeyUp?.Invoke(key);
        }
    }
}
