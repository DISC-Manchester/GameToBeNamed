using Avalonia.Input;
using NAudio.Wave;
using OpenTK.Mathematics;
using SquareSmash.objects.components.bricks;
using SquareSmash.renderer;
using SquareSmash.renderer.Windows;
using SquareSmash.utils;
using System;
using System.Threading.Tasks;

namespace SquareSmash.objects.components
{
    public class Ball
    {
        private readonly Vector2 Size = new(0.03f, 0.03f);
        private int LastScore = 0;
        public int Score { get; set; }
        private Vector2 Position;
        private Vector2 Velocity;
        private Vertex[] Vertices;
        private float Speed;
        private readonly float LaunchSpeed;
        private bool Released = false;
        private int CoolDown = 0;
        public Ball(Paddle paddle, float speed)
        {
            LaunchSpeed = Speed = speed;
            Position = paddle.GetPosition();
            Position.X += 0;
            Position.Y -= 10;
            Vertices = VertexUtils.PreMakeQuad(Vector2.Zero, Vector2.Zero, 0f);
        }

        public void ResetBall()
        {
            Velocity = Vector2.Zero;
            Released = false;
        }

        public int GetScore()
        {
            LastScore = Score;
            Score = 0;
            return LastScore;
        }

        public bool IsAlive() => Released;

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !Released)
            {
                DiscWindow.Instance.DisplayText.Text = "";
                Velocity.Y = LaunchSpeed;
                Velocity.X = Random.Shared.NextSingle() > 0.5f ? -LaunchSpeed : LaunchSpeed;
                Released = true;
            }
        }
        private void Play()
        {
            WaveOutEvent beepPlayer = new();
            beepPlayer.Init(new WaveFileReader(AssetUtil.OpenEmbeddedFile("assets.sounds.bounce.wav")));
            beepPlayer.Play();
        }

        public void OnUpdate(Level level, float DeltaTime)
        {
            if (Released is false)
            {
                Position = DiscWindow.Instance.Paddle.GetPosition();
                Position.X *= 10;
                Position.Y += 5;
                VertexUtils.UpdateQuad(Position, Size, ref Vertices);
                return;
            }

            Position += Velocity * DeltaTime;
            Position.X = Math.Clamp(Position.X, -171, 171);
            Position.Y = Math.Clamp(Position.Y, -171, 171);

            if (DiscWindow.Instance.Paddle.DoseFullIntersects(Vertices))
            {
                _ = Task.Run(Play);
                Velocity.Y = Math.Abs(Velocity.Y);
            }
            else if (Position.Y >= 170)
            {
                _ = Task.Run(Play);
                Velocity.Y = -Math.Abs(Velocity.Y);
            }
            else if (Position.X <= -170)
            {
                _ = Task.Run(Play);
                Velocity.X = Math.Abs(Velocity.X);
            }
            else if (Position.X >= 170)
            {
                _ = Task.Run(Play);
                Velocity.X = -Math.Abs(Velocity.X);
            }
            else
            {
                foreach (Brick brick in level.GetBricks())
                {
                    if (brick.IsActive() && brick.DoseFullIntersects(Vertices))
                    {
                        _ = Task.Run(Play);
                        if (CoolDown == 0)
                        {
                            CoolDown = 5;
                            brick.Die();
                            Speed *= 1.0000001f;
                            Score++;
                            if (brick.GetBrickType() == BrickType.LIFE)
                                DiscWindow.Instance.Paddle.AddLife();
                            Velocity.Y = -(Math.Abs(Velocity.Y) + Speed);
                        }
                        else
                            Velocity.Y = -Math.Abs(Velocity.Y);

                        if (Random.Shared.NextSingle() <= 0.2f)
                        {
                            if (Random.Shared.NextSingle() <= 0.5f)
                                Velocity.X -= Random.Shared.NextSingle() > 0.5f ? 0 : Speed;
                            else
                                Velocity.X += Random.Shared.NextSingle() > 0.5f ? 0 : Speed;
                        }
                        else
                            Velocity.X = Random.Shared.NextSingle() > 0.25f ? Velocity.X + Speed : Math.Abs(Velocity.X) + Speed;

                        Velocity.X = Math.Clamp(Velocity.X, -LaunchSpeed * 2, LaunchSpeed * 2);
                        Velocity.Y = Math.Clamp(Velocity.Y, -LaunchSpeed * 2, LaunchSpeed * 2);
                        break;
                    }
                }
            }

            if (CoolDown != 0)
                CoolDown--;

            if (Velocity.Y == 0)
                Velocity.Y += LaunchSpeed;

            VertexUtils.UpdateQuad(Position, Size, ref Vertices);

            if (Position.Y <= -170)
            {
                ResetBall();
                LastScore = Score;
                Speed = LaunchSpeed;
                DiscWindow.Instance.Paddle.ResetPaddle();
            }
        }
        public void OnRendering(object sender)
        {
            DiscWindow.Instance.ScoreText.Text = "Score: " + Score.ToString();
            ((QuadBatchRenderer)sender).AddQuad(Vertices);
        }
    }
}
