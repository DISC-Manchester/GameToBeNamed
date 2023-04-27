namespace SquareSmash.client.objects
{
    internal abstract class GameObjects : IDisposable
    {
        public abstract void Dispose();
        public abstract void OnRendering(object sender,EventArgs e);
    }
}
