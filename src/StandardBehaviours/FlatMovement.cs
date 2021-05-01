using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.StandardBehaviours
{
    public sealed class FlatMovement : Behaviour
    {
        public bool AlwaysRun { get; set; } = false;
        public float WalkSpeed { get; set; } = 0.75f;
        public float RunSpeed { get; set; } = 2.5f;
        public float TurnSpeed { get; set; } = 90f;
        public Key StepForward { get; set; } = Key.W;
        public Key StepBack { get; set; } = Key.S;
        public Key StepLeft { get; set; } = Key.A;
        public Key StepRight { get; set; } = Key.D;
        public Key TurnRight { get; set; } = Key.Right;
        public Key TurnLeft { get; set; } = Key.Left;
        public Key ChangeRun_Walk { get; set; } = Key.ShiftKey;

        void Update()
        {
            // Check speed
            bool run = AlwaysRun;
            if (Input.IsKeyDown(ChangeRun_Walk))
                run = !run;

            float speed;
            if (run)
                speed = RunSpeed;
            else speed = WalkSpeed;

            // Step
            if (Input.IsKeyDown(StepForward))
                Element.Translate(Vector.Forward * speed * Time.DeltaTime);
            if (Input.IsKeyDown(StepBack))
                Element.Translate(Vector.Backward * speed * Time.DeltaTime);
            if (Input.IsKeyDown(StepLeft))
                Element.Translate(Vector.Left * speed * Time.DeltaTime);
            if (Input.IsKeyDown(StepRight))
                Element.Translate(Vector.Right * speed * Time.DeltaTime);

            // Turn
            if (Input.IsKeyDown(TurnLeft))
                Element.Rotate(-TurnSpeed * Time.DeltaTime);
            if (Input.IsKeyDown(TurnRight))
                Element.Rotate(TurnSpeed * Time.DeltaTime);
        }
    }
}
