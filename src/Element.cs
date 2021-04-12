using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    internal abstract class Element
    {
        public abstract Vector Position { get; set; }
        public abstract Vector Rotation { get; set; }

        internal bool isInitialized;
        private List<BehaviorScript> scripts;

        public Element()
        {
            scripts = new List<BehaviorScript>();
        }

        public void AddScript(BehaviorScript script)
        {
            scripts.Add(script);
        }
    }
}
