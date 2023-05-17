using OpenTK.Mathematics;
using SquareSmash.renderer;
using SquareSmash.utils;

namespace SquareSmash.objects.components.bricks
{
    internal abstract class Brick
    {
        private readonly Vector2 Position;
        protected Colour3 Colour;
        private bool is_active = true;

        public delegate void OnBrickDeathEventHandler(object sender, EventArgs e);
        public event OnBrickDeathEventHandler OnBrickDeath;

        public abstract BrickType GetBrickType();
        public abstract void OnUpdate();
        protected Brick(Vector2 position, Colour3 colour, Level level)
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
        public void OnRendering(object sender)
        {
            if (is_active is not true)
                return;
            ((QuadBatchRenderer)sender).AddQuad(Position, new(90, 20), Colour);
        }
        public bool DoseFullIntersects(Vector2 position, Vector2 size)
        => CollisionUtil.DoseFullIntersects(Position, new(90, 20), position, size);
    }
}
