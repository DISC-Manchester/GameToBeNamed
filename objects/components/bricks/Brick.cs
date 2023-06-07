using OpenTK.Mathematics;
using SquareSmash.renderer;
using SquareSmash.utils;
using System;
namespace SquareSmash.objects.components.bricks
{
    public abstract class Brick
    {
        private readonly Vertex[] Vertices;
        private bool is_active = true;
        public delegate void OnBrickDeathEventHandler(object sender, EventArgs e);
        public event OnBrickDeathEventHandler OnBrickDeath;
        public abstract BrickType GetBrickType();
        public abstract void OnUpdate();
        protected Brick(Vector2 position, Colour3 colour, Level level)
        {
            Vertices = QuadBatchRenderer.PreMakeQuad(position, new(0.1225f, 0.04f), colour);
            OnBrickDeath += level.OnBrickDeath;
        }

        public void Die()
        {
            is_active = false;
            OnBrickDeath(this, EventArgs.Empty);
        }
        public bool IsActive() => is_active;

        protected void UpdateColour(Colour3 colour) 
        {
            for(int i = 0; i < Vertices.Length; i++)
                Vertices[i].Color = colour;
        }
        public void OnRendering(object sender)
        {
            if (is_active is not true)
                return;
            ((QuadBatchRenderer)sender).AddQuad(Vertices);
        }
        public bool DoseFullIntersects(Vertex[] testing_quad)
            => CollisonUtil.TestAABB(new(Vertices), new(testing_quad));
    }
}
