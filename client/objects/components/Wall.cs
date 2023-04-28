using OpenTK.Mathematics;
using SquareSmash.client.renderer;
using SquareSmash.client.Utils;

namespace SquareSmash.client.objects.components
{
    internal class Brick
    : ColidableGameObject
    {
        private readonly Vector2 Position;
        private readonly Color4 Colour;
        private bool is_active = true;
        public static readonly int Width = 90;
        public static readonly int Height = 20;

        public Brick(Vector2 position,Color4 colour)
        {
            Position = position;
            Colour = colour;
        }

        public void Die() => is_active = false;
        public bool IsActive() => is_active;

        public override void OnUpdate(object sender, UpdateEventArgs e)
        {
            if (is_active is not true)
                return;
        }

        public override void OnRendering(object sender, EventArgs e)
        {
            if (is_active is not true)
                return;
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
