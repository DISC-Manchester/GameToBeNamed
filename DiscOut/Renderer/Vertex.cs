namespace DiscOut.Renderer;
internal struct Vertex
{
    public float X { get; set; } = 0f;
    public float Y { get; set; } = 0f;
    public float ColorID { get; set; } = 0f;
    public float UvID { get; set; } = 0f;
    public Vertex() { }
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