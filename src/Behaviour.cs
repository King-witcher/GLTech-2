using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GLTech2
{
    public abstract class Behaviour
    {
        internal Element element;

        protected internal Behaviour() { } // Makes possible the creation of derived classes
        protected internal Element Element { get => element; }
        protected internal Scene Scene { get => element.scene; }

        private Action startMethod = null;
        internal Action StartMethod
        {
            get
            {
                if (startMethod is null)
                {
                    MethodInfo startInfo = GetType().GetMethod("Start",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null);

                    startMethod = () => startInfo?.Invoke(this, null);
                }

                return startMethod;
            }
        }

        private Action updateMethod = null;
        internal Action UpdateMethod
        {
            get
            {
                if (updateMethod is null)
                {
                    MethodInfo startInfo = GetType().GetMethod("Update",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null);

                    updateMethod = () => startInfo?.Invoke(this, null);
                }

                return updateMethod;
            }
        }

        // Métodos são captados via reflection.
        // Start()
        // Update()
        // Activate()
        // BeginCollide(Element collisor)
        // EndCollide(Element collisor)
    }
}
