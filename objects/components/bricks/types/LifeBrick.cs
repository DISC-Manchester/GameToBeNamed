using OpenTK.Mathematics;

namespace SquareSmash.objects.components.bricks.types
{
    internal class LifeBrick : Brick
    {
        private int wait = 0;
        public LifeBrick(Vector2 position, Level level) : base(position, new(byte.MaxValue, byte.MaxValue, byte.MaxValue), level)
        {
        }

        public override BrickType GetBrickType() => BrickType.LIFE;

        public override void OnUpdate()
        {
            if (wait < 10)
            {
                wait++;
                return;
            }
            wait = 0;
            Colour = new(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle());
        }
    }
}
