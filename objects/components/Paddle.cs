using Avalonia.Input;
using OpenTK.Mathematics;
using SquareSmash.renderer;
using SquareSmash.utils;

namespace SquareSmash.objects.components
{
    public class Paddle
    {
        private readonly Vector2 Size = new(0.3f, 0.03f);
        private Vertex[] Vertices = new Vertex[1];
        private Vector2 Position = new(1.0f, -150.0f);
        private Vector2 Velocity;
        private int Lives = 3;
        public Vector2 GetPosition() => Position;
        public bool IsDead() => Lives < 0;
        public void AddLife() => Lives++;

        public Paddle()
        {
            Vertices = VertexUtils.PreMakeQuad( Vector2.Zero, Vector2.Zero, 1f);
            ResetPaddle();
        }

        public void ResetPaddle()
        {
            Lives -= IsDead() ? -3 : 1;
            Velocity = Vector2.Zero;
            Position = new(1.0f, -150.0f);
            VertexUtils.UpdateQuad(Position, Size, ref Vertices);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Lives = -1;
            if (e.Key == Key.A && Position.X < 14)
                Velocity.X = 0.1f;
            else if (e.Key == Key.D && Position.X > -14)
                Velocity.X = -0.1f;
        }

        public void OnUpdate(float DeltaTime)
        {
            Position.X += Velocity.X * DeltaTime;
            Velocity.X = 0;
            Position.X = MathHelper.Clamp(Position.X, -15, 15);
            VertexUtils.UpdateQuad(Position, Size, ref Vertices);
        }

        public void OnRendering(object sender)
        {
            ((QuadBatchRenderer)sender).AddQuad(Vertices);
            float x = -33;
            for (uint i = 0; i <= Lives; i++)
            {
                ((QuadBatchRenderer)sender).AddQuad(new(x, 49f), new(0.1f, 0.1f), 2u);
                x -= 7f;
            }
            ((QuadBatchRenderer)sender).FlushAntiGhost();
        }
        public bool DoseFullIntersects(Vertex[] testing_quad)
            => CollisonUtil.TestAABB(new(Vertices), new(testing_quad));
    }
}
