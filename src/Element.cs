using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public abstract class Element
    {
        public abstract Vector Position { get; set; }
        public abstract Vector Rotation { get; set; }

        internal bool isInitialized;
        private List<Behavior> scripts;

        public Element()
        {
            scripts = new List<Behavior>();
        }

        public void AddScript(Behavior script)
        {
            scripts.Add(script);
        }
    }
}
