using SquareSmash.client.renderer;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SquareSmash.client.Utils;

namespace SquareSmash.client.objects.components
{
    internal class Paddle 
    : ColidableGameObject
    {
        private Vector2 Position;
        private readonly int Width;
        private readonly int Height;

        public Paddle()
        {
            Width = 80;
            Height = 20;

            Position = new Vector2
            {
                X = Client.Width / 2 - Width / 2,
                Y = Client.Height - 20
            };
        }

        public void ResetPaddle()
        {
            Position = new Vector2
            {
                X = Client.Width / 2 - Width / 2,
                Y = Client.Height - 20
            };
        }

        public override void OnUpdate(object sender, UpdateEventArgs e)
        {
            var client = (Client)sender;
            float MoveDistance = Math.Abs(0.001f * (float)e.DeltaTime) * Client.Width;
            if (client.KeyboardState.IsKeyDown(Keys.A) && Position.X > 20)
                Position.X -= MoveDistance;
            else if(client.KeyboardState.IsKeyDown(Keys.D) && Position.X < Client.Width - 60)
                Position.X += MoveDistance;
        }

        public override void OnRendering(object sender, EventArgs e)
        {
            ((QuadBatchRenderer)sender).AddQuad(Position,new(Width,Height), Color4.Purple);
        }

        public override void Dispose()
        {
        }

        public override bool DoseIntersects(Vector2 position, Vector2 size)
        {
            return CollisionUtil.DoseIntersects(Position, new(Width / 2, Height / 2), position, size);
        }
    }
}
