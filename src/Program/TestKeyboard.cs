using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    class TestKeyboard : Behaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(Input.Keys.W))
                Element.Translate(Vector.Unit * Time.DeltaTime);
        }

        void Start()
        {
        }
    }
}
