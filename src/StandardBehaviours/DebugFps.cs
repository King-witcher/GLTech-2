﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.StandardBehaviours
{
    public class DebugFps : Behaviour
    {
        public float Interval { get; set; } = 1f;

        int frames;
        float frametime;
        double rendertime;

        void Update()
        {
            frames++;
            rendertime += Time.RenderTime;
            frametime += Time.DeltaTime;

            if (frametime >= Interval)
            {
                Debug.Log("Potential fps: \t" + frames / rendertime);
                Debug.Log("Actual fps: \t" + frames / frametime);
                frames = 0;
                rendertime = 0;
                frametime = 0;
            }
        }
    }
}
