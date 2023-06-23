using System;
using SquareSmash.renderer;
using SquareSmash.utils;
namespace SquareSmash.objects.components.bricks
{
    public abstract class ChangeableBrick : Brick
    {
        public abstract void OnUpdate();
        protected void UpdateColour(float colour)
        {
            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i].ColorID = colour;
        }

        protected ChangeableBrick(float x, float y, float colour, Level level) : base(x, y, colour, level)
        {
        }
    }
}