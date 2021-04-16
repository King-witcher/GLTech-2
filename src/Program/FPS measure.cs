using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    class CountFPS : Behaviour
    {
        int frames = 0;
        double rendertime = 0f;
        double frametime = 0f;

        protected internal override void Update()
        {
            Element.Rotate(Time.DeltaTime * 5);
            Element.Translate(Time.DeltaTime * Vector.Unit * 0.2f);

            frames++;
            rendertime += Time.RenderTime;
            frametime += Time.DeltaTime;

            if (frames == 500)
            {
                Console.WriteLine("Render fps: " + 500 / rendertime);
                Console.WriteLine("Actual fps: " + 500 / frametime);
                frames = 0;
                rendertime = 0;
                frametime = 0;
            }
        }

        protected internal override void Start()
        {
        }
    }
}
