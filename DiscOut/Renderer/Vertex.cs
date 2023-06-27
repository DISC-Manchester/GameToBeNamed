namespace DiscOut.Renderer
{
    internal struct Vertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float ColorID { get; set; }
        public float UvID { get; set; }
    }
    internal struct Aabb
    {
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }

        public Aabb(Vertex[] quad)
        {
            MinX = quad[3].X;
            MinY = quad[3].Y;
            MaxX = quad[0].X;
            MaxY = quad[0].Y;
        }
    }
}