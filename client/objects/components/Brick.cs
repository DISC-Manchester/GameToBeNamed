using OpenTK.Mathematics;
using SquareSmash.client.renderer;
using SquareSmash.client.Utils;

namespace SquareSmash.client.objects.components
{
    internal class Wall
    : ColidableGameObject
    {
        private readonly Vector2 Position;
        private readonly Color4 Colour = Color4.LightGray;
        private int Width;
        private int Height;
        public Wall(Vector2 position,int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public void resizeWidth(int width)
        {
            Width = width;
        }

        public void resizeHeight(int height)
        {
            Height = height;
        }

        public override void OnUpdate(object sender, UpdateEventArgs e)
        {
        }

        public override void OnRendering(object sender, EventArgs e)
        {
            ((QuadBatchRenderer)sender).AddQuad(Position, new(Width, Height), Colour);
        }

        public override void Dispose()
        {
        }

        public override bool DoseIntersects(Vector2 position,Vector2 size)
        {
            return CollisionUtil.DoseIntersects(Position,new(Width / 2, Height / 2), position,size);
        }
    }
}
