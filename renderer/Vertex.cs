using OpenTK.Mathematics;
using SquareSmash.utils;


namespace SquareSmash.renderer
{
    internal struct Vertex
    {
#pragma warning disable S4487 // Unread "private" fields should be removed
        public Vector2 Position;
        public Colour3 Color;
        public Vector2 Uvs;
#pragma warning restore S4487 // Unread "private" fields should be removed
    }
}
