using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SquareSmash.renderer;
using SquareSmash.Utils;

namespace SquareSmash.objects.components
{
    internal class Paddle : ColidableGameObject
    {
        private static readonly int Width = 160;
        private static readonly int Height = 20;
        private Vector2 Position = new(Client.Width / 2 - Width / 2, Client.Height - 20);
        private Vector2 Velocity;
        private int Lives = 3;

        public Paddle()
        {
        }
        public static int GetWidth() => Width;
        public Vector2 GetPosition() => Position;
        public Vector2 GetVelocity() => Velocity;

        public void ResetPaddle()
        {
            Lives -= 1;
            Velocity = Vector2.Zero;
            Position = new(Client.Width / 2 - Width / 2, Client.Height - 20);
        }

        public override void OnUpdate(object sender, long DeltaTime)
        {
            var client = (Client)sender;
            if (client.KeyboardState.IsKeyDown(Keys.A) && Position.X > 12)
                Velocity.X = -0.5f;
            else if (client.KeyboardState.IsKeyDown(Keys.D) && Position.X < Client.Width - 92)
                Velocity.X = 0.5f;
            else
                Velocity.X = 0;

            Position.X += Velocity.X * DeltaTime;
        }

        public override void OnRendering(object sender)
        {
            ((QuadBatchRenderer)sender).AddQuad(Position, new(Width, Height), Colours.DiscPaddle);
            /*float x = 20;
            for (uint i = 0; i < lives; i++)
            {
                ((QuadBatchRenderer)sender).AddQuad(new(x,40), new(40, 40), Color4.Coral);
                x += (Client.Width / 11.0f) - 40;
            }*/
        }
        public override bool DoseFullIntersects(Vector2 position, Vector2 size)
            => CollisionUtil.DoseFullIntersects(Position, new(Width, Height), position, size);
    }
}
