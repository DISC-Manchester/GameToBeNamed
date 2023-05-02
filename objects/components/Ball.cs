using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SquareSmash.renderer;
namespace SquareSmash.objects.components
{
    internal class Ball : GameObject
    {
        private static readonly int Size = 20;

        private Vector2 Position = new(Client.Width / 2 - Size / 2 - 15, Client.Height - 50);
        private Vector2 Velocity;
        private float Speed = 0.001f;
        private bool Released = false;
        public void ResetBall()
        {
            Velocity = Vector2.Zero;
            Released = false;
        }
        public override void OnUpdate(object sender, long DeltaTime)
        {
            Tuple<Level, Client> senders = (Tuple<Level, Client>)sender;
            if (senders.Item2.KeyboardState.IsKeyPressed(Keys.Space) && Released is false)
            {
                Velocity.Y = Random.Shared.NextSingle() > 0.5f ? -(0.01f + Speed) : 0.01f + Speed;
                Velocity.X = Random.Shared.NextSingle() > 0.5f ? -(0.01f + Speed) : 0.01f + Speed;
                Released = true;
            }
            else if (Released is false)
            {
                Position = senders.Item2.paddle.GetPosition();
                Position.X += (float)Paddle.GetWidth() / 4 - (float)Size / 2;
                Position.Y -= (float)Size / 2;
                Position = new(Position.X + Velocity.X * (DeltaTime * 15.0f), Position.Y + Velocity.Y * (DeltaTime * 15.0f));
                return;
            }

            if (senders.Item2.paddle.DoseFullIntersects(Position, new(Size)))
            {
                Velocity.Y = -Velocity.Y;
                if (senders.Item2.paddle.GetVelocity().X != 0)
                    Velocity.X = -Velocity.X;
                else
                    Velocity.X += Math.Clamp(senders.Item2.paddle.GetVelocity().X,-0.001f,0.001f);
                Position.Y -= 1;
                Position.X += Velocity.X < 0 ? -1 : 1;
            }
            else if (senders.Item1.GetWallRight().DoseFullIntersects(Position, new(Size)))
            {
                Velocity.X = -Velocity.X;
                Position.X -= 1;
            }
            else if (senders.Item1.GetWallTop().DoseFullIntersects(Position, new(Size)))
            {
                Velocity.Y = -Velocity.Y;
                Position.Y += 1;
            }
            else if (senders.Item1.GetWallLeft().DoseFullIntersects(Position, new(Size)))
            {
                Velocity.X = -Velocity.X;
                Position.X += 1;
            }
            else
            {
                foreach (Brick brick in senders.Item1.GetBricks())
                {
                    if (brick.IsActive() && brick.DoseFullIntersects(Position, new(Size)))
                    {
                        brick.Die();
                        Speed += 0.00001f;
                        Velocity.Y = -Velocity.Y;

                        Position.X += Velocity.X < 0 ? -1 : 1;
                        Position.Y += Velocity.Y < 0 ? -1 : 1;

                        Velocity.X += Velocity.X < 0 ? -Speed : Speed;
                        Velocity.Y += Velocity.Y < 0 ? -Speed : Speed;

                        break;
                    }
                }
            }

            Position = new(Position.X + Velocity.X * (DeltaTime * 15.0f), Position.Y + Velocity.Y * (DeltaTime * 15.0f));
            if (Position.Y > Client.Height)
            {
                ResetBall();
                senders.Item2.paddle.ResetPaddle();
            }
        }
        public override void OnRendering(object sender)
            => ((QuadBatchRenderer)sender).AddQuad(Position, new(Size), Color4.White);
    }
}
