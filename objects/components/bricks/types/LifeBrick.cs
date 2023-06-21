using OpenTK.Mathematics;
using System;

namespace SquareSmash.objects.components.bricks.types
{
    public class LifeBrick : Brick
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
            UpdateColour(new(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle()));
        }
    }
}
