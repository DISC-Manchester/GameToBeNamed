using System.Collections.Generic;
using Avalonia.Input;
using DiscOut.Renderer;
using DiscOut.Util;
namespace DiscOut.GameObjects.Dynamic
{
    internal class Paddle
    {
        private Vertex[] Vertices = new Vertex[6];
        private short X;
        private short VelocityX;
        private sbyte Lives = 3;
        public short GetPositionX() => X;
        public bool IsDead() => Lives < 0;
        public void AddLife() => Lives++;

        public Paddle()
        {
            VertexUtils.PreMakeQuad(0, 0, 0, 0, 1f, ref Vertices);
            ResetPaddle();
        }

        public void ResetPaddle()
        {
            Lives -= (sbyte)(IsDead() ? -3 : 1);
            VelocityX = 0;
            VertexUtils.UpdateQuad(1.0f, -150.0f, 0.3f, 0.03f, ref Vertices);
        }

        public void OnKeyDown(HashSet<Key> keys)
        {
            if (keys.Contains(Key.A) || keys.Contains(Key.Left) && X < 14)
                VelocityX = 1;
            else if (keys.Contains(Key.D) || keys.Contains(Key.Right) && X > -14)
                VelocityX = -1;
        }

        public void OnUpdate()
        {
            X += VelocityX;
            VelocityX = 0;
            X = VertexUtils.Clamp(X, (short)-15, (short)15);
            VertexUtils.UpdateQuad(X, -150.0f, 0.3f, 0.03f, ref Vertices);
        }

        public void OnRendering(QuadBatchRenderer renderer)
        {
            renderer.AddQuad(Vertices);
            short x = -33;
            for (uint i = 0; i <= Lives; i++)
            {
                renderer.AddQuad(x, 49f, 0.1f, 0.1f, 2u);
                x -= 7;
            }
            renderer.FlushAntiGhost();
        }
        public bool DoseFullIntersects(Vertex[] testing_quad)
            => CollisonUtil.TestAABB(new Aabb(Vertices), new Aabb(testing_quad));
    }
}