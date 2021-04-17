#pragma warning disable IDE0051

namespace GLTech2.StandardBehaviours
{
    public sealed class Rotate : Behaviour
    {
        float Speed { get; set; } = 30f;
        void Update()
        {
            Element.Rotate(Speed * Time.DeltaTime);
        }
    }
}
