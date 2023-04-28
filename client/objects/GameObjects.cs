using OpenTK.Mathematics;

namespace SquareSmash.client.objects
{
    internal class UpdateEventArgs : EventArgs
    {
        public double DeltaTime { get; private set; }
        public UpdateEventArgs(double DeltaTime) 
        {
            this.DeltaTime = DeltaTime;
        }
    }
    internal abstract class GameObject : IDisposable
    {
        public abstract void Dispose();
        public abstract void OnUpdate(object sender, UpdateEventArgs e);
        public abstract void OnRendering(object sender,EventArgs e);
    }

    internal abstract class ColidableGameObject: GameObject
    {
        public abstract bool DoseIntersects(Vector2 position, Vector2 size);
    }
}
