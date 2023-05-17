using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SquareSmash.objects.components.bricks;
using SquareSmash.renderer;
namespace SquareSmash.objects.components
{
    internal class Ball
    {
        private static readonly int Size = 20;
        private int LastScore = 0;
        private int Score = 0;
        private Vector2 Position;
        private Vector2 Velocity;
        private float Speed;
        private readonly float LaunchSpeed;
        private bool Released = false;

        public static Vector2 GetSize() => new(Size, Size);
        public Ball(Paddle paddle, float speed)
        {
            LaunchSpeed = Speed = speed;
            Position = paddle.GetPosition();
            Position.X += (float)Paddle.GetWidth() / 4 - Size;
            Position.Y -= (float)Size / 2;
        }


        public void ResetBall()
        {
            Velocity = Vector2.Zero;
            Released = false;
        }

        public int GetScore()
        {
            Score = 0;
            return LastScore;
        }

        public bool IsAlive() => Released;
        public void OnUpdate(object sender, float DeltaTime)
        {
            Tuple<Level, Client> senders = (Tuple<Level, Client>)sender;
            if (senders.Item2.KeyboardState.IsKeyPressed(Keys.Space) && Released is false)
            {
                Velocity.Y = LaunchSpeed;
                Velocity.X = Random.Shared.NextSingle() > 0.5f ? -LaunchSpeed : LaunchSpeed;
                Released = true;
            }
            else if (Released is false)
            {
                Position = senders.Item2.Paddle.GetPosition();
                Position.X += (float)Paddle.GetWidth() / 4 - (float)Size / 2;
                Position.Y -= (float)Size / 2;
                return;
            }

            Position += Velocity * DeltaTime;

            if (senders.Item2.Paddle.DoseFullIntersects(Position, new Vector2(Size)))
            {
                Velocity.Y = -Math.Abs(Velocity.Y);
            }
            else if (Position.X <= 10)
                Velocity.X = Math.Abs(Velocity.X);
            else if (Position.X  > Client.Instance.Size.X - 20)
                Velocity.X = -Math.Abs(Velocity.X);
            else if (Position.Y <= 20)
                Velocity.Y = Math.Abs(Velocity.Y);
            else
            {
                foreach (Brick brick in senders.Item1.GetBricks())
                {
                    break;
                    if (brick.IsActive() && brick.DoseFullIntersects(Position, new Vector2(Size)))
                    {
                        brick.Die();
                        Speed *= 1.0001f;
                        Score++;
                        if (brick.GetBrickType() == BrickType.LIFE)
                            senders.Item2.Paddle.AddLife();
                        Velocity.Y = Math.Abs(Velocity.Y) + Speed;
                        if (Random.Shared.NextSingle() <= 0.2f)
                        {
                            if (Random.Shared.NextSingle() <= 0.5f)
                                Velocity.X -= Random.Shared.NextSingle() > 0.5f ? 0 : Speed;
                            else
                                Velocity.X += Random.Shared.NextSingle() > 0.5f ? 0 : Speed;
                        }
                        else
                            Velocity.X = Random.Shared.NextSingle() > 0.25f ? Velocity.X + Speed : Math.Abs(Velocity.X) + Speed;

                        Velocity.X = Math.Clamp(Velocity.X, -LaunchSpeed * 3, LaunchSpeed * 3);
                        Velocity.Y = Math.Clamp(Velocity.Y, -LaunchSpeed * 3, LaunchSpeed * 3);
                        break;
                    }
                }
            }

            if (Velocity.Y == 0)
                Velocity.Y += LaunchSpeed;

            if (Position.Y > Client.Instance.Size.Y)
            {
                ResetBall();
                LastScore = Score;
                Speed = LaunchSpeed;
                senders.Item2.Paddle.ResetPaddle();
            }
        }
        public void OnRendering(object sender)
        {
            ImGui.SetNextWindowPos(new(80, 20), ImGuiCond.Always, new(0.5f, 0.5f));
            bool temp = false;
            ImGui.Begin("ScoreBoard", ref temp, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus);
            ImGui.SetWindowFontScale(1.5f);
            ImGui.SetWindowSize(new(150, ImGui.GetTextLineHeightWithSpacing()));
            ImGui.Text("Score: " + Convert.ToString(Score));
            ImGui.End();
            ImGui.Begin("debug", ref temp, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings);
            ImGui.SetWindowSize(new(175, ImGui.GetTextLineHeightWithSpacing() * 3));
            ImGui.Text("pos: " + Position.ToString());
            ImGui.End();
            ((QuadBatchRenderer)sender).AddQuad(Position, new(Size), new(byte.MaxValue, byte.MaxValue, byte.MaxValue));
        }
    }
}
