﻿namespace SquareSmash.objects.components.bricks.types
{
    public class NormalBrick : Brick
    {
        public NormalBrick(float x, float y, float colour, Level level) : base(x,y, colour, level)
        {
        }
        public override BrickType GetBrickType() => BrickType.NORMAL;

    }
}
