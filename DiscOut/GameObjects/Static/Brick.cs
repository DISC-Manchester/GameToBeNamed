using DiscOut.GameObjects.World;
using DiscOut.Renderer;
using DiscOut.Util;
using System;
namespace DiscOut.GameObjects.Static
{
    internal enum BrickType
    {
        NORMAL,
        LIFE
    }
    internal abstract class Brick
    {
        protected Vertex[] Vertices = new Vertex[6];
        private Aabb hitbox;
        public delegate void OnBrickDeathEventHandler(object sender, EventArgs e);
        public event OnBrickDeathEventHandler OnBrickDeath;
        public abstract BrickType GetBrickType();
        protected Brick(int x, int y, byte colour, Level level)
        {
            VertexUtils.PreMakeQuad(x, y, 0.1225f, 0.04f, colour, ref Vertices);
            hitbox = new Aabb(Vertices);
            OnBrickDeath += level.OnBrickDeath;
        }
        public void Die() => OnBrickDeath(this, EventArgs.Empty);
        public virtual void OnRendering(QuadBatchRenderer renderer) => renderer.AddQuad(Vertices);
        public bool DoseFullIntersects(Vertex[] testing_quad) => CollisonUtil.TestAABB(hitbox, new Aabb(testing_quad));
    }
    internal class NormalBrick : Brick
    {
        public NormalBrick(int x, int y, byte colour, Level level) : base(x, y, colour, level)
        {
        }
        public override BrickType GetBrickType() => BrickType.NORMAL;
    }
    internal abstract class ChangeableBrick : Brick
    {
        public abstract void OnUpdate();
        protected void UpdateColour(byte colour)
        {
            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i].ColorID = colour;
        }
        protected ChangeableBrick(int x, int y, byte colour, Level level) : base(x, y, colour, level)
        {
        }
    }
    internal class LifeBrick : ChangeableBrick
    {
        private static readonly Random random = new Random();
        private int wait = 0;
        public LifeBrick(int x, int y, Level level) : base(x, y, 0, level)
        {
        }
        public override BrickType GetBrickType() => BrickType.LIFE;
        public override void OnRendering(QuadBatchRenderer renderer)
        {
            OnUpdate();
            base.OnRendering(renderer);
        }
        public override void OnUpdate()
        {
            if (wait > 10)
            {
                wait = 0;
                UpdateColour((byte)random.Next(5));
            }
            else
                wait++;
        }
    }
}