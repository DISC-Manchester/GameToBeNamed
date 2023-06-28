using Avalonia.Input;
using DiscOut.Avalonia;
using DiscOut.GameObjects.Static;
using DiscOut.GameObjects.World;
using DiscOut.Renderer;
using DiscOut.Util;
using System;
using System.Collections.Generic;
namespace DiscOut.GameObjects.Dynamic
{
    internal class Ball
    {
        private Random random = new Random();
        public int Score { get; set; } = 0;
        private int LastScore = 0;
        private Vertex[] Vertices = new Vertex[6];
        private float PositionX;
        private float PositionY = -140;
        private float VelocityX;
        private float VelocityY;
        private readonly byte LaunchSpeed;
        private bool Released = false;
        private byte CoolDown = 0;
        public Ball(byte speed)
        {
            LaunchSpeed = speed;
            PositionX = DiscWindow.Instance.Paddle.GetPositionX();
            VertexUtils.PreMakeQuad(0, 0, 0, 0, 0, ref Vertices);
        }
        public void ResetBall()
        {
            VelocityX = 0;
            VelocityY = 0;
            Released = false;
        }
        public int GetScore()
        {
            LastScore = Score;
            Score = 0;
            return LastScore;
        }
        public bool IsAlive() => Released;
        public void OnKeyDown(HashSet<Key> keys)
        {
            if (keys.Contains(Key.Space) && !Released)
            {
                SoundUtils.PlaySound(SoundUtils.CLICK_SOUND);
                DiscWindow.Instance.DisplayText.Text = "";
                VelocityY = LaunchSpeed;
                VelocityX = random.NextDouble() > 0.5 ? -LaunchSpeed : LaunchSpeed;
                Released = true;
            }
        }
        public void OnUpdate(Level level)
        {
            if (Released is false)
            {
                PositionX = DiscWindow.Instance.Paddle.GetPositionX() * 10;
                PositionY = -145;
                VertexUtils.UpdateQuad(PositionX, PositionY, 0.03f, 0.03f, ref Vertices);
                return;
            }
            PositionX = VertexUtils.Clamp(PositionX + VelocityX, -171, 171);
            PositionY = VertexUtils.Clamp(PositionY + VelocityY, -171, 171);
            if (DiscWindow.Instance.Paddle.DoseFullIntersects(Vertices))
            {
                SoundUtils.PlaySound(SoundUtils.BOUNCE_SOUND);
                VelocityY = Math.Abs(VelocityY);
            }
            else if (PositionY >= 170)
            {
                SoundUtils.PlaySound(SoundUtils.BOUNCE_SOUND);
                VelocityY = -Math.Abs(VelocityY);
            }
            else if (PositionX <= -170)
            {
                SoundUtils.PlaySound(SoundUtils.BOUNCE_SOUND);
                VelocityX = Math.Abs(VelocityX);
            }
            else if (PositionX >= 170)
            {
                SoundUtils.PlaySound(SoundUtils.BOUNCE_SOUND);
                VelocityX = -Math.Abs(VelocityX);
            }
            else
            {
                foreach (Brick brick in level.GetBricks())
                {
                    if (brick.DoseFullIntersects(Vertices))
                    {
                        SoundUtils.PlaySound(SoundUtils.BOUNCE_SOUND);
                        if (CoolDown == 0)
                        {
                            CoolDown = 5;
                            brick.Die();
                            Score++;
                            if (brick.GetBrickType() == BrickType.LIFE)
                                DiscWindow.Instance.Paddle.AddLife();
                            VelocityY = -(Math.Abs(VelocityY) * 1.02f);
                        }
                        else
                            VelocityY = -Math.Abs(VelocityY);

                        if (random.Next(12) > 7)
                        {
                            if (random.Next(12) > 7)
                                VelocityX -= random.Next(12) > 7 ? 0 : 3;
                            else
                                VelocityX += random.Next(12) > 7 ? 0 : 3;
                        }
                        else
                            VelocityX = random.Next(12) > 4 ? VelocityX * 1.02f : Math.Abs(VelocityX) * 1.02f;
                        break;
                    }
                }
            }
            if (CoolDown != 0)
                CoolDown--;
            if (VelocityY == 0)
                VelocityY += LaunchSpeed;
            VertexUtils.UpdateQuad(PositionX, PositionY, 0.03f, 0.03f, ref Vertices, 0);
            if (PositionY <= -170)
            {
                ResetBall();
                LastScore = Score;
                DiscWindow.Instance.Paddle.ResetPaddle();
            }
        }
        public void OnRendering(QuadBatchRenderer renderer)
        {
            DiscWindow.Instance.ScoreText.Text = "Score: " + Score.ToString();
            renderer.AddQuad(Vertices);
        }
    }
}