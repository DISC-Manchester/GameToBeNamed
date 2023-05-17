using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SquareSmash.renderer;
using SquareSmash.utils;

namespace SquareSmash.objects.components
{
    internal class Paddle
    {
        private static readonly Colour3 DiscPaddle = new(157, 6, 241);
        private static readonly int Width = 160;
        private Vector2 Position = new(Client.Instance.Size.X / 2 - Width / 4, Client.Instance.Size.Y - 20);
        private Vector2 Velocity;
        private int Lives = 2;
        public static int GetWidth() => Width;
        public Vector2 GetPosition() => Position;
        public Vector2 GetVelocity() => Velocity;
        public static Vector2 GetSize() => new(Width, 20);
        public bool IsDead() => Lives < 0;

        public void AddLife() => Lives++;

        public void ResetPaddle()
        {
            Lives -= IsDead() ? -3 : 1;
            Velocity = Vector2.Zero;
            Position = new(Client.Instance.Size.X / 2 - Width / 4, Client.Instance.Size.Y - 20);
        }

        public void OnUpdate(object sender, float DeltaTime)
        {
            var client = (Client)sender;
            if (client.KeyboardState.IsKeyDown(Keys.F1))
                Lives = -1;
            if (client.KeyboardState.IsKeyDown(Keys.A) && Position.X > 12)
                Velocity.X = -0.5f;
            else if (client.KeyboardState.IsKeyDown(Keys.D) && Position.X < Client.Instance.Size.X - 92)
                Velocity.X = 0.5f;
            else
                Velocity.X = 0;

            Position.X += Velocity.X * DeltaTime;
        }

        public void OnRendering(object sender)
        {
            ((QuadBatchRenderer)sender).AddQuad(Position, new(Width, 20), DiscPaddle);
            ((QuadBatchRenderer)sender).Flush();
            float x = Client.Instance.Size.X - 35;
            for (uint i = 0; i <= Lives; i++)
            {
                ((QuadBatchRenderer)sender).AddQuad(new(x, 35), new(40, 40), new(byte.MaxValue, 127, 80));
                x -= (Client.Instance.Size.X / 11.0f) - 35;
            }
            ((QuadBatchRenderer)sender).FlushAntiGhost();
        }
        public bool DoseFullIntersects(Vector2 position, Vector2 size)
            => CollisionUtil.DoseFullIntersects(Position, new(Width, 20), position, size);
    }
}
