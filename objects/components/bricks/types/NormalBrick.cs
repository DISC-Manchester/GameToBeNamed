using OpenTK.Mathematics;
using SquareSmash.utils;

namespace SquareSmash.objects.components.bricks.types
{
    public class NormalBrick : Brick
    {
        public NormalBrick(Vector2 position, float colour, Level level) : base(position, colour, level)
        {
        }

        public override BrickType GetBrickType() => BrickType.NORMAL;

        public override void OnUpdate() { }
    }
}
