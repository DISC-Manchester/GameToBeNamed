using OpenTK.Mathematics;
namespace SquareSmash.objects
{
    internal abstract class GameObject
    {
        public abstract void OnRendering(object sender);
        public abstract void OnUpdate(object sender, long DeltaTime);
    }
    internal abstract class ColidableRenderObject
    {
        public abstract void OnRendering(object sender);
        public abstract bool DoseFullIntersects(Vector2 position, Vector2 size);
    }
    internal abstract class ColidableGameObject : GameObject
    {
        public abstract bool DoseFullIntersects(Vector2 position, Vector2 size);
    }
}