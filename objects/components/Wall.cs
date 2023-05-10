using OpenTK.Mathematics;
using SquareSmash.renderer;
using SquareSmash.Utils;

namespace SquareSmash.objects.components
{
    internal class Wall : ColidableRenderObject
    {
        private readonly Vector2 Position;
        private readonly int Width;
        private readonly int Height;
        public Wall(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }
        public override void OnRendering(object sender)
            => ((QuadBatchRenderer)sender).AddQuad(Position, new(Width, Height), Color4.LightGray);
        public override bool DoseFullIntersects(Vector2 position, Vector2 size)
            => CollisionUtil.DoseFullIntersects(Position, new(Width, Height), position, size);
    }
}
