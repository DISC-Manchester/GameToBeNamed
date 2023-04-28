using SquareSmash.client.renderer;
using OpenTK.Mathematics;

namespace SquareSmash.client.objects.components
{
    internal class Ball
    : GameObject
    {
        private Vector2 Position;
        private Vector2 Velocity;
        private readonly int Width = 10;
        private readonly int Height = 10;

        public Ball()
        {
            Position = new Vector2
            {
                X = (Client.Width / 2 - Width / 2) - 15,
                Y = Client.Height - 50
            };
            Velocity.Y = -0.01f;
        }

        public void ResetBall()
        {
            Position = new Vector2
            {
                X = (Client.Width / 2 - Width / 2) - 15,
                Y = Client.Height - 50
            };
        }

        public override void OnUpdate(object sender, UpdateEventArgs e)
        {
            Tuple<Level, Client> senders = (Tuple<Level, Client>)sender;
            foreach (Brick brick in senders.Item1.GetBricks())
            {
                if (brick.IsActive() && brick.DoseIntersects(Position, new(Width, Height)))
                {
                    brick.Die();
                    Velocity.Y *= -1;
                    break;
                }
            }

            if(senders.Item2.paddle.DoseIntersects(Position,new(Width,Height)))
                Velocity.Y *= -1;

            Position = new Vector2
            {
                X = Position.X + Velocity.X,
                Y = Position.Y + Velocity.Y
            };

        }

        public override void OnRendering(object sender, EventArgs e)
        {
            ((QuadBatchRenderer)sender).AddQuad(Position, new(Width, Height), Color4.White);
        }

        public override void Dispose()
        {
        }
    }
}
