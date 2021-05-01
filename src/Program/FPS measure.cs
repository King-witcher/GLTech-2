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

        void Update()
        {
            if (Time.Elapsed < 50)
            {
                Element.Rotate(Time.DeltaTime * 6);
                Element.Translate(Time.DeltaTime * Vector.Forward * 0.08f);
            }

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

        void Start()
        {
        }
    }
}
