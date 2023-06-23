using System;
using Avalonia.Input;
using SquareSmash.renderer;
using SquareSmash.utils;

namespace SquareSmash.objects.components
{
    public class Paddle
    {
        private Vertex[] Vertices;
        private float X;
        private float VelocityX;
        private int Lives = 3;
        public float GetPositionX() => X;
        public bool IsDead() => Lives < 0;
        public void AddLife() => Lives++;

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public Paddle()
        {
            Vertices = VertexUtils.PreMakeQuad( 0,0,0,0, 1f);
            ResetPaddle();
        }

        public void ResetPaddle()
        {
            Lives -= IsDead() ? -3 : 1;
            VelocityX = 0;
            VertexUtils.UpdateQuad(1.0f, -150.0f, 0.3f, 0.03f, ref Vertices,0);
        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.A && X < 14)
                VelocityX = 0.1f;
            else if (key == Key.D && X > -14)
                VelocityX = -0.1f;
        }

        public void OnUpdate(float DeltaTime)
        {
            X += VelocityX * DeltaTime;
            VelocityX = 0;
            X = Clamp(X, -15, 15);
            VertexUtils.UpdateQuad(X, -150.0f, 0.3f, 0.03f, ref Vertices,0);
        }

        public void OnRendering(object sender)
        {
            ((QuadBatchRenderer)sender).AddQuad(Vertices);
            float x = -33;
            for (uint i = 0; i <= Lives; i++)
            {
                ((QuadBatchRenderer)sender).AddQuad(x, 49f,0.1f, 0.1f, 2u);
                x -= 7f;
            }
            ((QuadBatchRenderer)sender).FlushAntiGhost();
        }
        public bool DoseFullIntersects(ref Vertex[] testing_quad)
            => CollisonUtil.TestAABB(new(ref Vertices), new(ref testing_quad));
    }
}
