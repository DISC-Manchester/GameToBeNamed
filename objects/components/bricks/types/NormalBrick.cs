using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareSmash.objects.components.bricks.types
{
    internal class NormalBrick : Brick
    {
        public NormalBrick(Vector2 position, Color4 colour, Level level) : base(position, colour, level)
        {
        }

        public override BrickType GetBrickType() => BrickType.NORMAL;
    }
}
