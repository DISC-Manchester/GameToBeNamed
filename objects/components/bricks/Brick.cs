using OpenTK.Mathematics;
using SquareSmash.renderer;
using SquareSmash.Utils;

namespace SquareSmash.objects.components.bricks
{
    internal abstract class Brick : ColidableRenderObject
    {
        public static readonly int Width = 90;
        public static readonly int Height = 20;
        private readonly Vector2 Position;
        private readonly Color4 Colour;
        private bool is_active = true;

        public delegate void OnBrickDeathEventHandler(object sender, EventArgs e);
        public event OnBrickDeathEventHandler OnBrickDeath;

        public abstract BrickType GetBrickType();
        public Brick(Vector2 position, Color4 colour, Level level)
        {
            Position = position;
            Colour = colour;
            OnBrickDeath += level.OnBrickDeath;
        }

        public void Die()
        {
            is_active = false;
            OnBrickDeath(this, EventArgs.Empty);
        }
        public bool IsActive() => is_active;
        public override void OnRendering(object sender)
        {
            if (is_active is not true)
                return;
            ((QuadBatchRenderer)sender).AddQuad(Position, new(Width, Height), Colour);
        }
        public override bool DoseFullIntersects(Vector2 position, Vector2 size)
        => CollisionUtil.DoseFullIntersects(Position, new(Width, Height), position, size);
    }
}
