using OpenTK.Mathematics;
using SquareSmash.utils;
using System;

namespace SquareSmash.renderer
{
    public struct Vertex
    {
        public Vector2 Position { get; set; }
        public float Color { get; set; }
        public Vector2 Uvs { get; set; }
    }

    public struct Aabb
    {
        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }

        public Aabb(Vertex[] quad)
        {
            Min = (quad[3].Position + Vector2.One) * 1000 / 2;
            Min = new(MathF.Round(Min.X), MathF.Round(Min.Y));
            Max = (quad[0].Position + Vector2.One) * 1000 / 2;
            Max = new(MathF.Round(Max.X), MathF.Round(Max.Y));
        }
    }
}
