#pragma warning disable IDE0051

namespace GLTech2.StandardBehaviours
{
    public sealed class Move : Behaviour
    {
        public Vector Direction { get; set; } = Vector.Unit;
        public float Speed { get; set; } = 2f;

        void Update()
        {
            Element.Translate(Direction * Speed * Time.DeltaTime);
        }
    }
}
